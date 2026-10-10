using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bidding_Core.Helpers;
using Bidding_Core.Services.Interfaces;
using Bidding_DLL;
using Bidding_DLL.Services.Interfaces;
using Bidding_Models.DatabaseModel;
using Bidding_Models.DatabaseModel.Employment;
using Bidding_Models.DatabaseModel.SystemCatalog;
using Bidding_Models.Jobseekers;
using ConstructionHub_Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Bidding_Core.Services.Implementations
{
    public class NotificationDispatchService : INotificationDispatchService
    {
        private readonly INotificationTemplateRenderer _templateRenderer;
        private readonly CHDBContext _context;
        private readonly EmailSettingsVerifications _emailSettings;
        private readonly SMSSettings _smsSettings;
        private readonly IConfiguration _configuration;

        public NotificationDispatchService(
            INotificationTemplateRenderer templateRenderer,
            CHDBContext context,
            EmailSettingsVerifications emailSettings,
            SMSSettings smsSettings,
            IConfiguration configuration)
        {
            _templateRenderer = templateRenderer;
            _context = context;
            _emailSettings = emailSettings;
            _smsSettings = smsSettings;
            _configuration = configuration;
        }

        public async Task<bool> SendRegistrationEmailVerificationAsync(AppUser user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                return false;
            }

            var verifyUrl = BuildEmailVerificationUrl(user.Email, user.VerificationToken);
            var locale = ResolvePreferredLocale(user.Profile?.PreferredLanguage);
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["jobseeker_name"] = ResolveRegistrationDisplayName(user, locale),
                ["verification_link"] = verifyUrl,
            };

            var rendered = await _templateRenderer.RenderAsync(
                NotificationTemplateActionCodes.RegistrationEmailVerification,
                locale,
                values);

            if (rendered == null || !rendered.IsActive || rendered.Email == null)
            {
                return false;
            }

            var emailClient = new Email(_emailSettings);
            var emailTo = NotificationTestOverrides.ResolveEmail(user.Email.Trim());
            var emailLog = await SendEmailAndLogAsync(
                emailClient,
                emailTo,
                rendered.Email.Subject ?? string.Empty,
                rendered.Email.Body ?? string.Empty,
                locale,
                NotificationTemplateActionCodes.RegistrationEmailVerification,
                user.ProfileId);

            if (rendered.Sms != null && !string.IsNullOrWhiteSpace(rendered.Sms.Body))
            {
                await SendSmsAndLogAsync(
                    user.PhoneNumber,
                    rendered.Sms.Body,
                    locale,
                    NotificationTemplateActionCodes.RegistrationEmailVerification,
                    user.ProfileId);
            }

            await _context.SaveChangesAsync();
            return emailLog?.IsSuccess == true;
        }

        public async Task<NotificationTransactionLogRowDto> ResendFromLogAsync(long logId)
        {
            var source = await _context.NotificationTransactionLogs.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == logId);
            if (source == null)
            {
                return null;
            }

            NotificationTransactionLog created;
            var channel = (source.ChannelType ?? string.Empty).Trim();
            if (string.Equals(channel, "Email", StringComparison.OrdinalIgnoreCase))
            {
                var emailClient = new Email(_emailSettings);
                created = await SendEmailAndLogAsync(
                    emailClient,
                    source.Receiver,
                    source.Subject ?? string.Empty,
                    source.MessageBody ?? string.Empty,
                    source.PreferredLanguage,
                    source.ActionCode,
                    source.ProfileId);
            }
            else if (string.Equals(channel, "SMS", StringComparison.OrdinalIgnoreCase))
            {
                // Resend the logged payload as-is (do not reroute to test overrides).
                created = await SendSmsAndLogAsync(
                    source.Receiver,
                    source.MessageBody ?? string.Empty,
                    source.PreferredLanguage,
                    source.ActionCode,
                    source.ProfileId,
                    applyTestOverrides: false);
            }
            else
            {
                throw new InvalidOperationException("Unsupported channel type: " + channel);
            }

            await _context.SaveChangesAsync();
            return created == null ? null : MapTransactionLog(created);
        }

        internal static NotificationTransactionLogRowDto MapTransactionLog(NotificationTransactionLog row)
        {
            return new NotificationTransactionLogRowDto
            {
                Id = row.Id,
                ChannelType = row.ChannelType,
                Receiver = row.Receiver,
                Subject = row.Subject,
                MessageBody = row.MessageBody,
                PreferredLanguage = row.PreferredLanguage,
                ActionCode = row.ActionCode,
                ProfileId = row.ProfileId,
                IsSuccess = row.IsSuccess,
                FailureReason = row.FailureReason,
                CreatedAtUtc = row.CreatedAtUtc.ToUniversalTime().ToString("o"),
            };
        }

        private string BuildEmailVerificationUrl(string email, string token)
        {
            var domain = (_configuration["VerifyDomain"] ?? _emailSettings.VerifyDomain ?? string.Empty).Trim().TrimEnd('/');
            var encodedEmail = Uri.EscapeDataString(email ?? string.Empty);
            var encodedToken = Uri.EscapeDataString(token ?? string.Empty);
            return $"{domain}/api/v2/auth/verify-email?email={encodedEmail}&token={encodedToken}";
        }

        private static string ResolveRegistrationDisplayName(AppUser user, string locale)
        {
            var profile = user?.Profile;
            var isArabic = string.Equals(locale, "ar", StringComparison.OrdinalIgnoreCase);
            if (isArabic)
            {
                return FirstNonEmpty(profile?.NameAr, profile?.Name, user?.Email);
            }

            return FirstNonEmpty(profile?.Name, profile?.NameAr, user?.Email);
        }

        public async Task<bool> SendCustomEmailAsync(
            string receiver,
            string subject,
            string htmlBody,
            string locale,
            int? profileId,
            long customNotificationId)
        {
            var log = await SendEmailAndLogAsync(
                new Email(_emailSettings),
                NotificationTestOverrides.ResolveEmail((receiver ?? string.Empty).Trim()),
                subject ?? string.Empty,
                htmlBody ?? string.Empty,
                locale,
                NotificationTemplateActionCodes.CustomNotification,
                profileId,
                customNotificationId);
            return log?.IsSuccess == true;
        }

        public async Task<bool> SendCustomSmsAsync(
            string phone,
            string body,
            string locale,
            int? profileId,
            long customNotificationId)
        {
            var log = await SendSmsAndLogAsync(
                phone,
                body,
                locale,
                NotificationTemplateActionCodes.CustomNotification,
                profileId,
                applyTestOverrides: true,
                customNotificationId: customNotificationId);
            return log?.IsSuccess == true;
        }

        public void LogCustomSkipped(
            string channelType,
            string receiver,
            string locale,
            int? profileId,
            long customNotificationId,
            string reason)
        {
            AddTransactionLog(
                channelType: channelType,
                receiver: string.IsNullOrWhiteSpace(receiver) ? "-" : receiver,
                subject: null,
                messageBody: null,
                preferredLanguage: locale,
                actionCode: NotificationTemplateActionCodes.CustomNotification,
                profileId: profileId,
                isSuccess: false,
                failureReason: reason,
                customNotificationId: customNotificationId);
        }

        private async Task<NotificationTransactionLog> SendEmailAndLogAsync(
            Email emailClient,
            string receiver,
            string subject,
            string body,
            string locale,
            string actionCode,
            int? profileId,
            long? customNotificationId = null)
        {
            var success = false;
            string failure = null;
            try
            {
                success = await emailClient.SendEmailAsync(subject, body, receiver);
                if (!success)
                {
                    failure = "Email send returned false.";
                }
            }
            catch (Exception ex)
            {
                failure = ex.Message;
            }

            return AddTransactionLog(
                channelType: "Email",
                receiver: receiver,
                subject: subject,
                messageBody: body,
                preferredLanguage: locale,
                actionCode: actionCode,
                profileId: profileId,
                isSuccess: success,
                failureReason: failure,
                customNotificationId: customNotificationId);
        }

        private Task<NotificationTransactionLog> SendSmsAndLogAsync(
            string phone,
            string body,
            string locale,
            string actionCode,
            int? profileId,
            bool applyTestOverrides = true,
            long? customNotificationId = null)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return Task.FromResult<NotificationTransactionLog>(null);
            }

            var destination = applyTestOverrides
                ? NotificationTestOverrides.ResolveMobile(phone ?? string.Empty).Trim()
                : (phone ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(destination))
            {
                return Task.FromResult<NotificationTransactionLog>(null);
            }

            var success = false;
            string failure = null;
            var loggedReceiver = destination;
            try
            {
                var smsApi = new NewSmscapi(_smsSettings, _configuration);
                var smsNotification = new SMSEmailNotification();
                success = smsApi.SendSms(
                    smsNotification,
                    _smsSettings.SMSUserName,
                    _smsSettings.SMSPassword,
                    destination,
                    body.Trim(),
                    _smsSettings.SID);
                loggedReceiver = ExtractMsisdnFromReason(smsNotification.Reason) ?? destination;
                if (!success)
                {
                    failure = string.IsNullOrWhiteSpace(smsNotification.Reason)
                        ? "SMS send returned false."
                        : smsNotification.Reason;
                }
            }
            catch (Exception ex)
            {
                failure = ex.Message;
            }

            var log = AddTransactionLog(
                channelType: "SMS",
                receiver: loggedReceiver,
                subject: null,
                messageBody: body.Trim(),
                preferredLanguage: locale,
                actionCode: actionCode,
                profileId: profileId,
                isSuccess: success,
                failureReason: failure,
                customNotificationId: customNotificationId);

            return Task.FromResult(log);
        }

        private NotificationTransactionLog AddTransactionLog(
            string channelType,
            string receiver,
            string subject,
            string messageBody,
            string preferredLanguage,
            string actionCode,
            int? profileId,
            bool isSuccess,
            string failureReason,
            long? customNotificationId = null)
        {
            var row = new NotificationTransactionLog
            {
                ChannelType = channelType,
                Receiver = (receiver ?? string.Empty).Trim(),
                Subject = subject,
                MessageBody = messageBody,
                PreferredLanguage = ResolvePreferredLocale(preferredLanguage),
                ActionCode = actionCode,
                ProfileId = profileId,
                CustomNotificationId = customNotificationId,
                IsSuccess = isSuccess,
                FailureReason = failureReason,
                CreatedAtUtc = DateTime.UtcNow,
            };
            _context.NotificationTransactionLogs.Add(row);
            return row;
        }

        /// <summary>Parses <c>MSISDN=…; …</c> from <see cref="NewSmscapi"/> diagnostics.</summary>
        private static string ExtractMsisdnFromReason(string reason)
        {
            var raw = (reason ?? string.Empty).Trim();
            if (!raw.StartsWith("MSISDN=", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var value = raw.Substring("MSISDN=".Length);
            var sep = value.IndexOf(';');
            if (sep >= 0)
            {
                value = value.Substring(0, sep);
            }

            value = value.Trim();
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private static string ResolvePreferredLocale(string preferredLanguage)
        {
            return string.Equals((preferredLanguage ?? string.Empty).Trim(), "ar", StringComparison.OrdinalIgnoreCase)
                ? "ar"
                : "en";
        }

        private static string FirstNonEmpty(params string[] values)
        {
            foreach (var value in values ?? Array.Empty<string>())
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            return string.Empty;
        }
    }
}

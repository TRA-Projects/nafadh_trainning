using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Bidding_Models.DatabaseModel.Notifications;
using Bidding_Models.Jobseekers;

namespace Bidding_Core.Helpers
{
    /// <summary>Pure rules for admin-authored custom notifications (validation, edit lock, recipient planning).</summary>
    public static class CustomNotificationRules
    {
        public const int TitleMaxLength = 256;
        public const int SmsBodyMaxLength = 1000;

        /// <summary>Tolerance so a schedule picked "now" in the browser is not rejected by clock drift.</summary>
        private static readonly TimeSpan PastScheduleTolerance = TimeSpan.FromMinutes(2);

        public const string SkipNoEmail = "Skipped: no email address.";
        public const string SkipNoPhone = "Skipped: no phone number.";
        public const string SkipNoAccount = "Skipped: organization has no registered account.";
        public const string SkipNoJobSeekerAccount = "Skipped: job seeker has not registered yet.";

        public sealed class RecipientUser
        {
            public string UserId { get; set; }
            public int? ProfileId { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string PreferredLanguage { get; set; }
        }

        public sealed class RecipientOrganization
        {
            public int OrganizationId { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
        }

        /// <summary>Imported job seeker with no login: Arabic email / SMS only.</summary>
        public sealed class RecipientUnregisteredJobSeeker
        {
            public int ProfileId { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Name { get; set; }
        }

        public sealed class PlannedDelivery
        {
            public string Channel { get; set; }
            public string Receiver { get; set; }
            public string Locale { get; set; }
            public string UserId { get; set; }
            public int? ProfileId { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public bool Skipped { get; set; }
            public string SkipReason { get; set; }
        }

        public sealed class DeliveryPlan
        {
            public int TotalRecipients { get; set; }
            public List<PlannedDelivery> Deliveries { get; set; } = new List<PlannedDelivery>();
        }

        public static bool CanEdit(string status)
        {
            return string.Equals(status, CustomNotificationStatuses.Scheduled, StringComparison.OrdinalIgnoreCase);
        }

        public static bool CanUserAccessInboxRow(string rowUserId, string callerUserId)
        {
            return !string.IsNullOrWhiteSpace(callerUserId)
                && string.Equals(rowUserId, callerUserId, StringComparison.Ordinal);
        }

        public static string ResolveLocale(string preferredLanguage)
        {
            return string.Equals((preferredLanguage ?? string.Empty).Trim(), "ar", StringComparison.OrdinalIgnoreCase)
                ? "ar"
                : "en";
        }

        /// <summary>Returns an error message, or null when the request is valid. Resolves the UTC send time.</summary>
        public static string Validate(UpsertCustomNotificationRequestDto request, DateTime utcNow, out DateTime scheduledAtUtc)
        {
            scheduledAtUtc = utcNow;
            if (request == null)
            {
                return "Request body is required.";
            }

            if (string.IsNullOrWhiteSpace(request.Title_en) || string.IsNullOrWhiteSpace(request.Title_ar))
            {
                return "Title is required in both English and Arabic.";
            }

            if (request.Title_en.Trim().Length > TitleMaxLength || request.Title_ar.Trim().Length > TitleMaxLength)
            {
                return $"Title must be at most {TitleMaxLength} characters.";
            }

            if (string.IsNullOrWhiteSpace(request.Body_en) || string.IsNullOrWhiteSpace(request.Body_ar))
            {
                return "Message is required in both English and Arabic.";
            }

            if (!request.SendEmail && !request.SendSms && !request.SendSystem)
            {
                return "Select at least one channel (email, SMS or system).";
            }

            if (request.SendSms
                && (request.Body_en.Trim().Length > SmsBodyMaxLength || request.Body_ar.Trim().Length > SmsBodyMaxLength))
            {
                return $"Message must be at most {SmsBodyMaxLength} characters when SMS is selected.";
            }

            var targets = NormalizeTargets(request.Targets);
            if (targets.Count == 0)
            {
                return "Select at least one target (user, group or organization).";
            }

            if (!request.SendNow)
            {
                if (!DateTime.TryParse(
                        (request.ScheduledAtUtc ?? string.Empty).Trim(),
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                        out var parsed))
                {
                    return "A valid schedule date and time is required.";
                }

                if (parsed < utcNow - PastScheduleTolerance)
                {
                    return "The schedule time cannot be in the past.";
                }

                scheduledAtUtc = parsed < utcNow ? utcNow : parsed;
            }

            return null;
        }

        /// <summary>Drops invalid rows and duplicates; "all organizations" replaces individual organization picks.</summary>
        public static List<CustomNotificationTargetDto> NormalizeTargets(IEnumerable<CustomNotificationTargetDto> targets)
        {
            var list = (targets ?? Enumerable.Empty<CustomNotificationTargetDto>())
                .Where(t => t != null)
                .ToList();

            // Individual organization picks are redundant only when "all organizations" is unfiltered.
            var hasAllOrganizations = list.Any(t => IsType(t, CustomNotificationTargetTypes.AllOrganizations)
                && CustomNotificationRegistrationFilters.Normalize(t.RegistrationFilter) == CustomNotificationRegistrationFilters.All);
            var result = new List<CustomNotificationTargetDto>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var t in list)
            {
                string key;
                CustomNotificationTargetDto normalized;
                if (IsType(t, CustomNotificationTargetTypes.User) && !string.IsNullOrWhiteSpace(t.UserId))
                {
                    key = "U:" + t.UserId.Trim();
                    normalized = new CustomNotificationTargetDto
                    {
                        TargetType = CustomNotificationTargetTypes.User,
                        UserId = t.UserId.Trim(),
                        Label = t.Label?.Trim(),
                    };
                }
                else if (IsType(t, CustomNotificationTargetTypes.UserGroup) && t.UserGroupId > 0)
                {
                    key = "G:" + t.UserGroupId.Value;
                    normalized = new CustomNotificationTargetDto
                    {
                        TargetType = CustomNotificationTargetTypes.UserGroup,
                        UserGroupId = t.UserGroupId,
                        Label = t.Label?.Trim(),
                    };
                }
                else if (IsType(t, CustomNotificationTargetTypes.Organization) && t.OrganizationId > 0 && !hasAllOrganizations)
                {
                    key = "O:" + t.OrganizationId.Value;
                    normalized = new CustomNotificationTargetDto
                    {
                        TargetType = CustomNotificationTargetTypes.Organization,
                        OrganizationId = t.OrganizationId,
                        Label = t.Label?.Trim(),
                    };
                }
                else if (IsType(t, CustomNotificationTargetTypes.AllOrganizations))
                {
                    key = "ALL_ORGS";
                    normalized = new CustomNotificationTargetDto
                    {
                        TargetType = CustomNotificationTargetTypes.AllOrganizations,
                        RegistrationFilter = CustomNotificationRegistrationFilters.Normalize(t.RegistrationFilter),
                        Label = t.Label?.Trim(),
                    };
                }
                else if (IsType(t, CustomNotificationTargetTypes.JobSeekers))
                {
                    key = "ALL_JOBSEEKERS";
                    normalized = new CustomNotificationTargetDto
                    {
                        TargetType = CustomNotificationTargetTypes.JobSeekers,
                        RegistrationFilter = CustomNotificationRegistrationFilters.Normalize(t.RegistrationFilter),
                        Label = t.Label?.Trim(),
                    };
                }
                else
                {
                    continue;
                }

                if (seen.Add(key))
                {
                    result.Add(normalized);
                }
            }

            return result;
        }

        /// <summary>
        /// Without registered users the audience has no inbox: unregistered organizations need email,
        /// unregistered job seekers need email or SMS.
        /// </summary>
        public static string ValidateReachability(CustomNotificationRecipientPreviewDto preview, bool sendEmail, bool sendSms = false)
        {
            if (preview == null || preview.RegisteredUsers > 0)
            {
                return null;
            }

            var reachable = (preview.EmailOnlyOrganizations > 0 && sendEmail)
                || (preview.UnregisteredJobSeekers > 0 && (sendEmail || sendSms));
            if (reachable)
            {
                return null;
            }

            if (preview.UnregisteredJobSeekers > 0)
            {
                return "The selected recipients are not registered and can only be reached by email or SMS. Select the Email or SMS channel.";
            }

            if (preview.EmailOnlyOrganizations > 0)
            {
                return "The selected organizations are not registered and can only be reached by email. Select the Email channel.";
            }

            return null;
        }

        public static DeliveryPlan Plan(
            CustomNotification notification,
            IEnumerable<RecipientUser> users,
            IEnumerable<RecipientOrganization> unregisteredOrganizations,
            IEnumerable<RecipientUnregisteredJobSeeker> unregisteredJobSeekers = null)
        {
            var plan = new DeliveryPlan();
            if (notification == null)
            {
                return plan;
            }

            var emailSeen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var phoneSeen = new HashSet<string>(StringComparer.Ordinal);
            var userSeen = new HashSet<string>(StringComparer.Ordinal);

            foreach (var user in users ?? Enumerable.Empty<RecipientUser>())
            {
                if (user == null || string.IsNullOrWhiteSpace(user.UserId) || !userSeen.Add(user.UserId))
                {
                    continue;
                }

                plan.TotalRecipients++;
                var locale = ResolveLocale(user.PreferredLanguage);
                var title = PickByLocale(locale, notification.Title_en, notification.Title_ar);
                var body = PickByLocale(locale, notification.Body_en, notification.Body_ar);
                var email = (user.Email ?? string.Empty).Trim();
                var phone = (user.Phone ?? string.Empty).Trim();

                if (notification.SendEmail)
                {
                    if (email.Length == 0)
                    {
                        plan.Deliveries.Add(Skip(CustomNotificationChannels.Email, user.UserId, locale, user, SkipNoEmail));
                    }
                    else if (emailSeen.Add(email))
                    {
                        plan.Deliveries.Add(new PlannedDelivery
                        {
                            Channel = CustomNotificationChannels.Email,
                            Receiver = email,
                            Locale = locale,
                            UserId = user.UserId,
                            ProfileId = user.ProfileId,
                            Subject = title,
                            Body = RenderEmailHtml(title, body, locale),
                        });
                    }
                }

                if (notification.SendSms)
                {
                    var phoneKey = NormalizePhone(phone);
                    if (phoneKey.Length == 0)
                    {
                        plan.Deliveries.Add(Skip(CustomNotificationChannels.Sms, email.Length > 0 ? email : user.UserId, locale, user, SkipNoPhone));
                    }
                    else if (phoneSeen.Add(phoneKey))
                    {
                        plan.Deliveries.Add(new PlannedDelivery
                        {
                            Channel = CustomNotificationChannels.Sms,
                            Receiver = phone,
                            Locale = locale,
                            UserId = user.UserId,
                            ProfileId = user.ProfileId,
                            Body = (title + "\n" + body).Trim(),
                        });
                    }
                }

                if (notification.SendSystem)
                {
                    plan.Deliveries.Add(new PlannedDelivery
                    {
                        Channel = CustomNotificationChannels.System,
                        Receiver = user.UserId,
                        Locale = locale,
                        UserId = user.UserId,
                        ProfileId = user.ProfileId,
                        Subject = title,
                        Body = body,
                    });
                }
            }

            var orgSeen = new HashSet<int>();
            foreach (var org in unregisteredOrganizations ?? Enumerable.Empty<RecipientOrganization>())
            {
                if (org == null || !orgSeen.Add(org.OrganizationId))
                {
                    continue;
                }

                var email = (org.Email ?? string.Empty).Trim();
                if (email.Length > 0 && emailSeen.Contains(email))
                {
                    continue;
                }

                plan.TotalRecipients++;
                var receiver = email.Length > 0 ? email : (org.Name ?? ("Organization " + org.OrganizationId));

                if (notification.SendEmail)
                {
                    if (email.Length == 0)
                    {
                        plan.Deliveries.Add(SkipOrg(CustomNotificationChannels.Email, receiver, SkipNoEmail));
                    }
                    else
                    {
                        emailSeen.Add(email);
                        plan.Deliveries.Add(new PlannedDelivery
                        {
                            Channel = CustomNotificationChannels.Email,
                            Receiver = email,
                            Locale = "ar",
                            Subject = BilingualSubject(notification),
                            Body = RenderBilingualEmailHtml(notification),
                        });
                    }
                }

                if (notification.SendSms)
                {
                    plan.Deliveries.Add(SkipOrg(CustomNotificationChannels.Sms, receiver, SkipNoPhone));
                }

                if (notification.SendSystem)
                {
                    plan.Deliveries.Add(SkipOrg(CustomNotificationChannels.System, receiver, SkipNoAccount));
                }
            }

            var jobSeekerSeen = new HashSet<int>();
            foreach (var js in unregisteredJobSeekers ?? Enumerable.Empty<RecipientUnregisteredJobSeeker>())
            {
                if (js == null || !jobSeekerSeen.Add(js.ProfileId))
                {
                    continue;
                }

                var email = (js.Email ?? string.Empty).Trim();
                var phone = (js.Phone ?? string.Empty).Trim();
                var phoneKey = NormalizePhone(phone);
                var emailNew = email.Length > 0 && !emailSeen.Contains(email);
                var phoneNew = phoneKey.Length > 0 && !phoneSeen.Contains(phoneKey);
                if (!emailNew && !phoneNew && (email.Length > 0 || phoneKey.Length > 0))
                {
                    continue;
                }

                plan.TotalRecipients++;
                var title = PickByLocale("ar", notification.Title_en, notification.Title_ar);
                var body = PickByLocale("ar", notification.Body_en, notification.Body_ar);
                var receiver = phone.Length > 0 ? phone : (email.Length > 0 ? email : (js.Name ?? ("Profile " + js.ProfileId)));

                if (notification.SendEmail)
                {
                    if (email.Length == 0)
                    {
                        plan.Deliveries.Add(SkipJobSeeker(CustomNotificationChannels.Email, receiver, js, SkipNoEmail));
                    }
                    else if (emailSeen.Add(email))
                    {
                        plan.Deliveries.Add(new PlannedDelivery
                        {
                            Channel = CustomNotificationChannels.Email,
                            Receiver = email,
                            Locale = "ar",
                            ProfileId = js.ProfileId,
                            Subject = title,
                            Body = RenderEmailHtml(title, body, "ar"),
                        });
                    }
                }

                if (notification.SendSms)
                {
                    if (phoneKey.Length == 0)
                    {
                        plan.Deliveries.Add(SkipJobSeeker(CustomNotificationChannels.Sms, receiver, js, SkipNoPhone));
                    }
                    else if (phoneSeen.Add(phoneKey))
                    {
                        plan.Deliveries.Add(new PlannedDelivery
                        {
                            Channel = CustomNotificationChannels.Sms,
                            Receiver = phone,
                            Locale = "ar",
                            ProfileId = js.ProfileId,
                            Body = (title + "\n" + body).Trim(),
                        });
                    }
                }

                if (notification.SendSystem)
                {
                    plan.Deliveries.Add(SkipJobSeeker(CustomNotificationChannels.System, receiver, js, SkipNoJobSeekerAccount));
                }
            }

            return plan;
        }

        public static string ComputeFinalStatus(int sentCount, int failedCount)
        {
            if (failedCount <= 0)
            {
                return CustomNotificationStatuses.Sent;
            }

            return sentCount > 0 ? CustomNotificationStatuses.PartiallyFailed : CustomNotificationStatuses.Failed;
        }

        public static string BilingualSubject(CustomNotification n)
        {
            return $"{(n.Title_ar ?? string.Empty).Trim()} | {(n.Title_en ?? string.Empty).Trim()}";
        }

        public static string RenderEmailHtml(string title, string body, string locale)
        {
            var dir = locale == "ar" ? "rtl" : "ltr";
            return $"<div dir=\"{dir}\" lang=\"{locale}\"><h3>{Encode(title)}</h3><p>{EncodeMultiline(body)}</p></div>";
        }

        public static string RenderBilingualEmailHtml(CustomNotification n)
        {
            return RenderEmailHtml(n.Title_ar, n.Body_ar, "ar")
                + "<hr/>"
                + RenderEmailHtml(n.Title_en, n.Body_en, "en");
        }

        private static string PickByLocale(string locale, string en, string ar)
        {
            var picked = locale == "ar" ? ar : en;
            if (string.IsNullOrWhiteSpace(picked))
            {
                picked = locale == "ar" ? en : ar;
            }

            return (picked ?? string.Empty).Trim();
        }

        private static string NormalizePhone(string phone)
        {
            return new string((phone ?? string.Empty).Where(char.IsDigit).ToArray());
        }

        private static bool IsType(CustomNotificationTargetDto t, string type)
        {
            return string.Equals((t?.TargetType ?? string.Empty).Trim(), type, StringComparison.OrdinalIgnoreCase);
        }

        private static PlannedDelivery Skip(string channel, string receiver, string locale, RecipientUser user, string reason)
        {
            return new PlannedDelivery
            {
                Channel = channel,
                Receiver = receiver,
                Locale = locale,
                UserId = user.UserId,
                ProfileId = user.ProfileId,
                Skipped = true,
                SkipReason = reason,
            };
        }

        private static PlannedDelivery SkipJobSeeker(string channel, string receiver, RecipientUnregisteredJobSeeker js, string reason)
        {
            return new PlannedDelivery
            {
                Channel = channel,
                Receiver = receiver,
                Locale = "ar",
                ProfileId = js.ProfileId,
                Skipped = true,
                SkipReason = reason,
            };
        }

        private static PlannedDelivery SkipOrg(string channel, string receiver, string reason)
        {
            return new PlannedDelivery
            {
                Channel = channel,
                Receiver = receiver,
                Locale = "ar",
                Skipped = true,
                SkipReason = reason,
            };
        }

        private static string Encode(string text)
        {
            return WebUtility.HtmlEncode((text ?? string.Empty).Trim());
        }

        private static string EncodeMultiline(string text)
        {
            return Encode(text).Replace("\r\n", "\n").Replace("\n", "<br/>");
        }
    }
}

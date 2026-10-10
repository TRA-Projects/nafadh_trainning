using ConstructionHub_Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bidding_API.ExceptionHandler
{
    /// <summary>
    /// Creates support tickets, writes .txt logs, and emails support with the log attached.
    /// </summary>
    public sealed class SupportTicketService
    {
        private static readonly ConcurrentDictionary<string, DateTime> RecentClientReports =
            new ConcurrentDictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        private readonly IConfiguration _configuration;
        private readonly ILoggerManager _logger;
        private readonly EmailSettings _emailSettings;

        public SupportTicketService(
            IConfiguration configuration,
            ILoggerManager logger,
            EmailSettings emailSettings)
        {
            _configuration = configuration;
            _logger = logger;
            _emailSettings = emailSettings;
        }

        public SupportTicketInfo CreateTicket(
            string source,
            string summary,
            string detailBody,
            HttpContext httpContext = null,
            string preferredTicketId = null)
        {
            var occurredAtUtc = DateTime.UtcNow;
            var ticketId = NormalizeOrCreateTicketId(preferredTicketId, occurredAtUtc);

            var sb = new StringBuilder();
            sb.AppendLine("NAFADH Jobseeker support ticket");
            sb.AppendLine("================================");
            sb.AppendLine($"TicketId: {ticketId}");
            sb.AppendLine($"OccurredAtUtc: {occurredAtUtc:O}");
            sb.AppendLine($"OccurredAtLocal: {occurredAtUtc.ToLocalTime():O}");
            sb.AppendLine($"Source: {source}");
            sb.AppendLine($"Summary: {summary}");

            if (httpContext != null)
            {
                sb.AppendLine($"Method: {httpContext.Request?.Method}");
                sb.AppendLine($"Path: {httpContext.Request?.Path}{httpContext.Request?.QueryString}");
                sb.AppendLine($"TraceIdentifier: {httpContext.TraceIdentifier}");
                sb.AppendLine($"RemoteIp: {httpContext.Connection?.RemoteIpAddress}");
                var userId = httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? httpContext.User?.FindFirst("sub")?.Value
                    ?? httpContext.User?.FindFirst("ProfileId")?.Value;
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    sb.AppendLine($"User: {userId}");
                }
            }

            sb.AppendLine();
            sb.AppendLine("Details");
            sb.AppendLine("-------");
            sb.AppendLine(detailBody ?? "(none)");

            var text = sb.ToString();
            var bytes = Encoding.UTF8.GetBytes(text);
            var path = WriteTicketFile(ticketId, bytes);

            var messageTemplate = _configuration["GlobalErrorMessage"]
                ?? "Contact NAFADH support with ticket {0} ({1} UTC).";
            var userMessage = string.Format(
                messageTemplate,
                ticketId,
                occurredAtUtc.ToString("yyyy-MM-dd HH:mm:ss"));

            return new SupportTicketInfo
            {
                TicketId = ticketId,
                OccurredAtUtc = occurredAtUtc,
                LogFilePath = path,
                LogBytes = bytes,
                UserMessage = userMessage
            };
        }

        public void TryEmailTicketFireAndForget(SupportTicketInfo ticket, string subjectHint = null)
        {
            if (ticket == null)
            {
                return;
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await SendTicketEmailAsync(ticket, subjectHint).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Support ticket email failed for {ticket.TicketId}", ex);
                }
            });
        }

        public async Task<bool> SendTicketEmailAsync(SupportTicketInfo ticket, string subjectHint = null)
        {
            if (ticket == null)
            {
                return false;
            }

            var enabled = true;
            var enabledRaw = _configuration["SupportTickets:Enabled"];
            if (!string.IsNullOrWhiteSpace(enabledRaw) && bool.TryParse(enabledRaw, out var parsed))
            {
                enabled = parsed;
            }

            if (!enabled)
            {
                return false;
            }

            var toList = _configuration.GetSection("SupportTickets:To").GetChildren()
                .Select(c => c.Value)
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => v.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (toList.Count == 0)
            {
                _logger.LogWarn($"SupportTickets:To is empty; ticket {ticket.TicketId} was not emailed.");
                return false;
            }

            var prefix = _configuration["SupportTickets:SubjectPrefix"] ?? "[NAFADH Jobseeker Ticket]";
            var subject = $"{prefix} {ticket.TicketId}".Trim();
            if (!string.IsNullOrWhiteSpace(subjectHint))
            {
                subject = $"{subject} — {subjectHint.Trim()}";
            }

            var body = new StringBuilder();
            body.AppendLine("<p>A support ticket was created.</p>");
            body.AppendLine($"<p><strong>Ticket:</strong> {ticket.TicketId}<br/>");
            body.AppendLine($"<strong>UTC:</strong> {ticket.OccurredAtUtc:yyyy-MM-dd HH:mm:ss}</p>");
            body.AppendLine("<p>See the attached .txt log for details.</p>");

            try
            {
                var email = new Email(_emailSettings);
                _logger.LogInfo(
                    $"Support ticket email sending {ticket.TicketId} to {string.Join(", ", toList)} via {_emailSettings.Host}:{_emailSettings.Port}");
                var ok = await email.SendEmailWithAttachmentAsync(
                    subject,
                    body.ToString(),
                    toList,
                    ticket.LogBytes ?? Array.Empty<byte>(),
                    $"{ticket.TicketId}.txt").ConfigureAwait(false);

                if (!ok)
                {
                    _logger.LogWarn($"Support ticket email returned false for {ticket.TicketId}");
                    return false;
                }

                _logger.LogInfo($"Support ticket email sent for {ticket.TicketId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Support ticket email exception for {ticket.TicketId}", ex);
                return false;
            }
        }

        public bool ShouldThrottleClientReport(string key, TimeSpan window)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            var now = DateTime.UtcNow;
            if (RecentClientReports.TryGetValue(key, out var last) && now - last < window)
            {
                return true;
            }

            RecentClientReports[key] = now;

            // Opportunistic cleanup
            if (RecentClientReports.Count > 500)
            {
                foreach (var pair in RecentClientReports.ToArray())
                {
                    if (now - pair.Value > TimeSpan.FromMinutes(10))
                    {
                        RecentClientReports.TryRemove(pair.Key, out _);
                    }
                }
            }

            return false;
        }

        public static string CreateTicketId(DateTime utcNow)
        {
            var suffix = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpperInvariant();
            return $"JS-{utcNow:yyyyMMdd}-{utcNow:HHmmss}-{suffix}";
        }

        private static string NormalizeOrCreateTicketId(string preferred, DateTime utcNow)
        {
            if (!string.IsNullOrWhiteSpace(preferred))
            {
                var trimmed = preferred.Trim();
                if (trimmed.Length <= 64
                    && trimmed.StartsWith("JS-", StringComparison.OrdinalIgnoreCase)
                    && trimmed.All(c => char.IsLetterOrDigit(c) || c == '-'))
                {
                    return trimmed.ToUpperInvariant();
                }
            }

            return CreateTicketId(utcNow);
        }

        private static string WriteTicketFile(string ticketId, byte[] bytes)
        {
            try
            {
                var dir = Path.Combine(AppContext.BaseDirectory, "logs", "tickets");
                Directory.CreateDirectory(dir);
                var path = Path.Combine(dir, $"{ticketId}.txt");
                File.WriteAllBytes(path, bytes);
                return path;
            }
            catch
            {
                return null;
            }
        }

        public static string FormatExceptionDetail(Exception ex)
        {
            if (ex == null)
            {
                return "(no exception)";
            }

            return ex.ToString();
        }

        public static string FormatClientReportDetail(ClientErrorReportRequest req)
        {
            return JsonConvert.SerializeObject(req ?? new ClientErrorReportRequest(), Formatting.Indented);
        }
    }
}

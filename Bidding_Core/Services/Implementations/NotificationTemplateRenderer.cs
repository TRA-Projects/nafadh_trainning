using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bidding_Core.Helpers;
using Bidding_Core.Services.Interfaces;
using Bidding_DLL;
using Bidding_Models.DatabaseModel.SystemCatalog;
using Bidding_Models.Jobseekers;
using Microsoft.EntityFrameworkCore;

namespace Bidding_Core.Services.Implementations
{
    public class NotificationTemplateRenderer : INotificationTemplateRenderer
    {
        private readonly CHDBContext _context;
        private readonly ConcurrentDictionary<string, NotificationTemplate> _cache =
            new ConcurrentDictionary<string, NotificationTemplate>(StringComparer.OrdinalIgnoreCase);

        public NotificationTemplateRenderer(CHDBContext context)
        {
            _context = context;
        }

        public async Task<RenderedNotificationDto> RenderAsync(
            string actionCode,
            string locale,
            IReadOnlyDictionary<string, string> values)
        {
            var template = await GetTemplateAsync(actionCode);
            if (template == null)
            {
                return null;
            }

            var isArabic = string.Equals(locale, "ar", StringComparison.OrdinalIgnoreCase);
            var emailSubject = isArabic ? template.EmailSubjectAr : template.EmailSubjectEn;
            var emailBody = isArabic ? template.EmailBodyAr : template.EmailBodyEn;
            var smsBody = isArabic ? template.SmsBodyAr : template.SmsBodyEn;

            return new RenderedNotificationDto
            {
                ActionCode = template.ActionCode,
                IsActive = template.IsActive,
                Email = template.IsActive && template.EmailEnabled
                    ? new RenderedNotificationChannelDto
                    {
                        Subject = NotificationTemplateTokenParser.ReplaceTokens(emailSubject, values),
                        Body = NotificationTemplateTokenParser.ReplaceTokens(emailBody, values),
                    }
                    : null,
                Sms = template.IsActive && template.SmsEnabled
                    ? new RenderedNotificationChannelDto
                    {
                        Subject = null,
                        Body = NotificationTemplateTokenParser.ReplaceTokens(smsBody, values),
                    }
                    : null,
            };
        }

        public void InvalidateCache(string actionCode = null)
        {
            if (string.IsNullOrWhiteSpace(actionCode))
            {
                _cache.Clear();
                return;
            }

            _cache.TryRemove(actionCode.Trim(), out _);
        }

        private async Task<NotificationTemplate> GetTemplateAsync(string actionCode)
        {
            if (string.IsNullOrWhiteSpace(actionCode))
            {
                return null;
            }

            var key = actionCode.Trim();
            if (_cache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var row = await _context.NotificationTemplates.AsNoTracking()
                .FirstOrDefaultAsync(t => t.ActionCode == key);
            if (row != null)
            {
                _cache[key] = row;
            }

            return row;
        }
    }
}

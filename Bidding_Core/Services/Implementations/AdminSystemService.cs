using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Bidding_API.Helpers;
using Bidding_Core.Helpers;
using Bidding_Core.Services.Interfaces;
using Bidding_DLL;
using Bidding_Models.DatabaseModel.SystemCatalog;
using Bidding_Models.Jobseekers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Bidding_Core.Services.Implementations
{
    public class AdminSystemService : IAdminSystemService
    {
        private readonly CHDBContext _context;
        private readonly INotificationTemplateRenderer _notificationTemplateRenderer;
        private readonly INotificationDispatchService _notificationDispatch;

        public AdminSystemService(
            CHDBContext context,
            INotificationTemplateRenderer notificationTemplateRenderer,
            INotificationDispatchService notificationDispatch)
        {
            _context = context;
            _notificationTemplateRenderer = notificationTemplateRenderer;
            _notificationDispatch = notificationDispatch;
        }

        public async Task<AdminSystemPagedListDto<SystemVariableDto>> GetSystemVariablesPageAsync(int? pageIndex, int? pageSize, string q)
        {
            var rows = await _context.SystemVariables.AsNoTracking().OrderBy(v => v.VariableKey).ToListAsync();
            var filtered = FilterSystemVariables(rows, q);
            return AdminSystemPaging.SlicePage(filtered, pageIndex, pageSize);
        }

        public async Task<SystemVariableDto> UpdateSystemVariableAsync(string variableKey, object value)
        {
            var row = await _context.SystemVariables.FirstOrDefaultAsync(v => v.VariableKey == variableKey);
            if (row == null)
            {
                return null;
            }

            row.Value = NormalizeSystemVariableValue(row.VariableKey, row.ValueType, value);
            row.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return MapSystemVariable(row);
        }

        public async Task<AdminSystemPagedListDto<SystemConfigValueDto>> GetConfigValuesPageAsync(
            string code,
            int? pageIndex,
            int? pageSize,
            string parentReasonId,
            bool includeInactive = false)
        {
            var group = await FindConfigGroupAsync(code);
            if (group == null)
            {
                return AdminSystemPaging.SlicePage(new List<SystemConfigValueDto>(), pageIndex, pageSize);
            }

            var query = _context.SystemConfigValues.AsNoTracking()
                .Where(v => v.ConfigGroupId == group.Id);
            if (!includeInactive)
            {
                query = query.Where(v => v.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(parentReasonId))
            {
                query = query.Where(v => v.ParentValueKey == parentReasonId);
            }

            var rows = await query.OrderBy(v => v.SortOrder).ThenBy(v => v.LabelEn).ToListAsync();
            return AdminSystemPaging.SlicePage(rows.Select(MapConfigValue).ToList(), pageIndex, pageSize);
        }

        public async Task<List<SystemConfigValueDto>> SearchConfigValuesAsync(string code, string q)
        {
            var needle = AdminSystemPaging.NormalizeSearch(q);
            if (string.IsNullOrEmpty(needle))
            {
                return new List<SystemConfigValueDto>();
            }

            var group = await FindConfigGroupAsync(code);
            if (group == null)
            {
                return new List<SystemConfigValueDto>();
            }

            var rows = await _context.SystemConfigValues.AsNoTracking()
                .Where(v => v.ConfigGroupId == group.Id && v.IsActive)
                .OrderBy(v => v.SortOrder)
                .ToListAsync();

            return rows
                .Where(row =>
                    AdminSystemPaging.NormalizeSearch(row.LabelEn).Contains(needle) ||
                    AdminSystemPaging.NormalizeSearch(row.LabelAr).Contains(needle))
                .Select(MapConfigValue)
                .ToList();
        }

        public async Task<SystemConfigValueDto> EnsureConfigValueAsync(string code, EnsureSystemConfigValueRequestDto draft)
        {
            var group = await EnsureConfigGroupAsync(code);
            var en = (draft?.label_en ?? string.Empty).Trim();
            var ar = (draft?.label_ar ?? string.Empty).Trim();

            var existing = await _context.SystemConfigValues
                .Where(v => v.ConfigGroupId == group.Id)
                .ToListAsync();

            var found = FindExistingConfigValue(existing, en, ar);
            if (found != null)
            {
                return MapConfigValue(found);
            }

            var baseSlug = new string((en + ar).ToLowerInvariant()
                .Where(ch => char.IsLetterOrDigit(ch) || ch == '-')
                .ToArray());
            if (string.IsNullOrWhiteSpace(baseSlug))
            {
                baseSlug = "inst-" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }

            var slug = baseSlug.Length > 40 ? baseSlug.Substring(0, 40) : baseSlug;
            var n = 1;
            while (existing.Any(row => string.Equals(row.ValueKey, slug, StringComparison.OrdinalIgnoreCase)))
            {
                slug = $"{baseSlug}-{n++}";
            }

            var nextCode = string.Equals(code, "WRK_SCOPE", StringComparison.OrdinalIgnoreCase)
                ? NextWorkScopeNumericCode(existing)
                : slug.ToUpperInvariant().Length > 3
                    ? slug.ToUpperInvariant().Substring(0, 3)
                    : slug.ToUpperInvariant();

            return await CreateConfigValueAsync(code, new CreateSystemConfigValueRequestDto
            {
                code = nextCode,
                label_en = string.IsNullOrEmpty(en) ? ar : en,
                label_ar = string.IsNullOrEmpty(ar) ? en : ar,
                IsActive = draft?.IsActive ?? true,
                parentReasonId = draft?.parentReasonId
            });
        }

        /// <summary>
        /// Exact match on labels / value key / code, then longest contains match
        /// (e.g. Nafath "دبلوم تعليم العام" → QUAL_DEGR "دبلوم").
        /// </summary>
        private static SystemConfigValue FindExistingConfigValue(
            List<SystemConfigValue> existing,
            string en,
            string ar)
        {
            if (existing == null || existing.Count == 0)
            {
                return null;
            }

            var exact = existing.FirstOrDefault(row =>
                (!string.IsNullOrEmpty(en) && (
                    string.Equals(row.LabelEn, en, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(row.ValueKey, en, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(row.Code, en, StringComparison.OrdinalIgnoreCase))) ||
                (!string.IsNullOrEmpty(ar) && (
                    string.Equals(row.LabelAr, ar, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(row.ValueKey, ar, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(row.Code, ar, StringComparison.OrdinalIgnoreCase))));
            if (exact != null)
            {
                return exact;
            }

            SystemConfigValue best = null;
            var bestScore = 0;
            foreach (var row in existing)
            {
                var labelEn = (row.LabelEn ?? string.Empty).Trim();
                var labelAr = (row.LabelAr ?? string.Empty).Trim();
                var score = 0;
                if (!string.IsNullOrEmpty(en) && !string.IsNullOrEmpty(labelEn) && LabelsOverlap(en, labelEn))
                {
                    score = Math.Max(score, labelEn.Length);
                }

                if (!string.IsNullOrEmpty(ar) && !string.IsNullOrEmpty(labelAr) && LabelsOverlap(ar, labelAr))
                {
                    score = Math.Max(score, labelAr.Length);
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    best = row;
                }
            }

            // Require a meaningful overlap (avoid matching single-character noise).
            return bestScore >= 2 ? best : null;
        }

        private static bool LabelsOverlap(string a, string b)
        {
            return a.IndexOf(b, StringComparison.OrdinalIgnoreCase) >= 0
                || b.IndexOf(a, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string NextWorkScopeNumericCode(List<SystemConfigValue> existing)
        {
            var maxNumeric = 0;
            foreach (var row in existing)
            {
                var code = (row.Code ?? string.Empty).Trim();
                if (int.TryParse(code, out var parsed) && parsed > maxNumeric)
                {
                    maxNumeric = parsed;
                }
            }

            return (maxNumeric + 1).ToString();
        }

        public async Task<SystemConfigValueDto> CreateConfigValueAsync(string code, CreateSystemConfigValueRequestDto row)
        {
            var group = await EnsureConfigGroupAsync(code);
            var nextCode = (row?.code ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(nextCode))
            {
                throw new InvalidOperationException("Missing config value code");
            }

            var valueKey = !string.IsNullOrWhiteSpace(nextCode) ? nextCode.ToLowerInvariant() : nextCode;
            var duplicate = await _context.SystemConfigValues.AnyAsync(v =>
                v.ConfigGroupId == group.Id &&
                (v.Code == nextCode || v.ValueKey == valueKey));
            if (duplicate)
            {
                throw new InvalidOperationException("Duplicate config value code");
            }

            var utcNow = DateTime.UtcNow;
            var created = new SystemConfigValue
            {
                ConfigGroupId = group.Id,
                ValueKey = valueKey,
                Code = nextCode,
                LabelEn = row.label_en ?? string.Empty,
                LabelAr = row.label_ar ?? string.Empty,
                IsActive = row.IsActive,
                SubReasonDropdown = row.subReasonDropdown,
                ParentValueKey = Convert.ToString(row.parentReasonId),
                SortOrder = await _context.SystemConfigValues.CountAsync(v => v.ConfigGroupId == group.Id) + 1,
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };

            _context.SystemConfigValues.Add(created);
            await _context.SaveChangesAsync();
            return MapConfigValue(created);
        }

        public async Task<SystemConfigValueDto> UpdateConfigValueAsync(string code, string id, UpdateSystemConfigValueRequestDto patch)
        {
            var group = await FindConfigGroupAsync(code);
            if (group == null)
            {
                return null;
            }

            var row = await _context.SystemConfigValues.FirstOrDefaultAsync(v =>
                v.ConfigGroupId == group.Id &&
                (v.ValueKey == id || v.Code == id));
            if (row == null)
            {
                return null;
            }

            if (patch != null)
            {
                if (patch.label_en != null) row.LabelEn = patch.label_en;
                if (patch.label_ar != null) row.LabelAr = patch.label_ar;
                row.IsActive = patch.IsActive;
                if (patch.subReasonDropdown.HasValue) row.SubReasonDropdown = patch.subReasonDropdown;
                if (patch.parentReasonId != null) row.ParentValueKey = Convert.ToString(patch.parentReasonId);
            }

            row.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return MapConfigValue(row);
        }

        public async Task<AdminSystemPagedListDto<SkillCategoryDto>> GetSkillCategoriesPageAsync(int? pageIndex, int? pageSize, string q)
        {
            var rows = await _context.CatalogSkillCategories.AsNoTracking().OrderBy(c => c.SortOrder).ToListAsync();
            var filtered = FilterSkillCategories(rows, q);
            return AdminSystemPaging.SlicePage(filtered.Select(MapSkillCategory).ToList(), pageIndex, pageSize);
        }

        public async Task<SkillCategoryDto> CreateSkillCategoryAsync(SkillCategoryDto body)
        {
            var utcNow = DateTime.UtcNow;
            var created = new CatalogSkillCategory
            {
                NameEn = body.NameEn,
                NameAr = body.NameAr,
                SortOrder = body.SortOrder,
                IsActive = body.IsActive,
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };
            _context.CatalogSkillCategories.Add(created);
            await _context.SaveChangesAsync();
            return MapSkillCategory(created);
        }

        public async Task<SkillCategoryDto> UpdateSkillCategoryAsync(int id, SkillCategoryDto body)
        {
            var row = await _context.CatalogSkillCategories.FirstOrDefaultAsync(c => c.Id == id);
            if (row == null)
            {
                return null;
            }

            row.NameEn = body.NameEn ?? row.NameEn;
            row.NameAr = body.NameAr ?? row.NameAr;
            row.SortOrder = body.SortOrder;
            row.IsActive = body.IsActive;
            row.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return MapSkillCategory(row);
        }

        public async Task<AdminSystemPagedListDto<SkillDto>> GetSkillsPageAsync(int? pageIndex, int? pageSize, string q, int? categoryId, int? kind)
        {
            var query = _context.CatalogSkills.AsNoTracking().AsQueryable();
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(s => s.CategoryId == categoryId.Value);
            }

            if (kind.HasValue && kind.Value > 0)
            {
                var kindValue = (CatalogSkillKind)kind.Value;
                query = query.Where(s => s.Kind == kindValue);
            }

            var rows = await query.OrderBy(s => s.NameEn).ToListAsync();
            var filtered = FilterSkills(rows, q);
            return AdminSystemPaging.SlicePage(filtered.Select(MapSkill).ToList(), pageIndex, pageSize);
        }

        public async Task<SkillDto> CreateSkillAsync(SkillDto body)
        {
            body = body ?? new SkillDto();
            var utcNow = DateTime.UtcNow;
            var kind = NormalizeKind(body.Kind);
            var created = new CatalogSkill
            {
                CategoryId = kind == CatalogSkillKind.TechStack ? null : body.CategoryId,
                Kind = kind,
                NameEn = body.NameEn,
                NameAr = body.NameAr,
                IsActive = body.IsActive,
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };
            _context.CatalogSkills.Add(created);
            await _context.SaveChangesAsync();
            return MapSkill(created);
        }

        public async Task<SkillDto> UpdateSkillAsync(int id, SkillDto body)
        {
            var row = await _context.CatalogSkills.FirstOrDefaultAsync(s => s.Id == id);
            if (row == null)
            {
                return null;
            }

            row.CategoryId = body.CategoryId;
            row.Kind = NormalizeKind(body.Kind);
            row.NameEn = body.NameEn ?? row.NameEn;
            row.NameAr = body.NameAr ?? row.NameAr;
            row.IsActive = body.IsActive;
            row.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return MapSkill(row);
        }

        private static CatalogSkillKind NormalizeKind(CatalogSkillKind kind) =>
            Enum.IsDefined(typeof(CatalogSkillKind), kind) ? kind : CatalogSkillKind.Professional;

        public async Task<string> GetSystemVariableValueAsync(string variableKey)
        {
            if (string.IsNullOrWhiteSpace(variableKey))
            {
                return string.Empty;
            }

            var row = await _context.SystemVariables.AsNoTracking()
                .FirstOrDefaultAsync(v => v.VariableKey == variableKey.Trim());
            return row?.Value ?? string.Empty;
        }

        public async Task<AdminSystemPagedListDto<NotificationTemplateListItemDto>> GetNotificationTemplatesPageAsync(
            int? pageIndex,
            int? pageSize,
            string q)
        {
            var rows = await _context.NotificationTemplates.AsNoTracking()
                .OrderBy(t => t.SortOrder)
                .ThenBy(t => t.ActionCode)
                .ToListAsync();
            var filtered = FilterNotificationTemplates(rows, q);
            return AdminSystemPaging.SlicePage(filtered, pageIndex, pageSize);
        }

        public async Task<NotificationTemplateDto> GetNotificationTemplateAsync(string actionCode)
        {
            var row = await FindNotificationTemplateAsync(actionCode);
            return row == null ? null : MapNotificationTemplate(row);
        }

        public Task<NotificationTemplatePlaceholdersDto> GetNotificationTemplatePlaceholdersAsync(string actionCode)
        {
            var code = (actionCode ?? string.Empty).Trim();
            return Task.FromResult(new NotificationTemplatePlaceholdersDto
            {
                ActionCode = code,
                Placeholders = NotificationPlaceholderRegistry.GetPlaceholders(code).ToList(),
            });
        }

        public async Task<NotificationTemplateDto> UpdateNotificationTemplateAsync(
            string actionCode,
            UpdateNotificationTemplateRequestDto body)
        {
            var row = await FindNotificationTemplateAsync(actionCode, tracked: true);
            if (row == null)
            {
                return null;
            }

            body = body ?? new UpdateNotificationTemplateRequestDto();
            var labelEn = (body.LabelEn ?? string.Empty).Trim();
            var labelAr = (body.LabelAr ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(labelEn) || string.IsNullOrEmpty(labelAr))
            {
                throw new InvalidOperationException("LabelEn and LabelAr are required");
            }

            var emailBodyEn = NotificationHtmlSanitizer.Sanitize(body.EmailBodyEn);
            var emailBodyAr = NotificationHtmlSanitizer.Sanitize(body.EmailBodyAr);
            var smsBodyEn = NormalizeSms(body.SmsBodyEn);
            var smsBodyAr = NormalizeSms(body.SmsBodyAr);

            ValidateTemplateTokens(row.ActionCode, body.EmailSubjectEn, body.EmailSubjectAr, emailBodyEn, emailBodyAr, smsBodyEn, smsBodyAr);

            row.LabelEn = labelEn;
            row.LabelAr = labelAr;
            row.EmailSubjectEn = (body.EmailSubjectEn ?? string.Empty).Trim();
            row.EmailSubjectAr = (body.EmailSubjectAr ?? string.Empty).Trim();
            row.EmailBodyEn = emailBodyEn;
            row.EmailBodyAr = emailBodyAr;
            row.SmsBodyEn = smsBodyEn;
            row.SmsBodyAr = smsBodyAr;
            row.IsActive = body.IsActive;
            row.EmailEnabled = body.EmailEnabled;
            row.SmsEnabled = body.SmsEnabled;
            row.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _notificationTemplateRenderer.InvalidateCache(row.ActionCode);
            return MapNotificationTemplate(row);
        }

        public async Task<RenderedNotificationDto> PreviewNotificationTemplateAsync(string actionCode, string locale)
        {
            var code = (actionCode ?? string.Empty).Trim();
            var placeholders = NotificationPlaceholderRegistry.GetPlaceholders(code);
            var values = placeholders.ToDictionary(p => p.Token, p => p.SampleValue ?? string.Empty);
            return await _notificationTemplateRenderer.RenderAsync(code, locale, values);
        }

        private async Task<NotificationTemplate> FindNotificationTemplateAsync(string actionCode, bool tracked = false)
        {
            if (string.IsNullOrWhiteSpace(actionCode))
            {
                return null;
            }

            var query = tracked
                ? _context.NotificationTemplates.AsQueryable()
                : _context.NotificationTemplates.AsNoTracking();
            return await query.FirstOrDefaultAsync(t => t.ActionCode == actionCode.Trim());
        }

        private static string NormalizeSms(string value)
        {
            var plain = NotificationHtmlSanitizer.StripHtml(value ?? string.Empty).Trim();
            if (plain.Length > 500)
            {
                throw new InvalidOperationException("SMS body must be 500 characters or fewer");
            }

            return plain;
        }

        private static void ValidateTemplateTokens(string actionCode, params string[] contents)
        {
            var unknown = NotificationPlaceholderRegistry.FindUnknownTokens(actionCode, contents).ToList();
            if (unknown.Count > 0)
            {
                throw new InvalidOperationException("Unknown placeholders: " + string.Join(", ", unknown));
            }
        }

        private static NotificationTemplateListItemDto MapNotificationTemplateListItem(NotificationTemplate row) =>
            new NotificationTemplateListItemDto
            {
                Id = row.Id,
                ActionCode = row.ActionCode,
                LabelEn = row.LabelEn,
                LabelAr = row.LabelAr,
                IsActive = row.IsActive,
                EmailEnabled = row.EmailEnabled,
                SmsEnabled = row.SmsEnabled,
                SortOrder = row.SortOrder,
            };

        private static NotificationTemplateDto MapNotificationTemplate(NotificationTemplate row)
        {
            var list = MapNotificationTemplateListItem(row);
            return new NotificationTemplateDto
            {
                Id = list.Id,
                ActionCode = list.ActionCode,
                LabelEn = list.LabelEn,
                LabelAr = list.LabelAr,
                IsActive = list.IsActive,
                EmailEnabled = list.EmailEnabled,
                SmsEnabled = list.SmsEnabled,
                SortOrder = list.SortOrder,
                EmailSubjectEn = row.EmailSubjectEn ?? string.Empty,
                EmailSubjectAr = row.EmailSubjectAr ?? string.Empty,
                EmailBodyEn = row.EmailBodyEn ?? string.Empty,
                EmailBodyAr = row.EmailBodyAr ?? string.Empty,
                SmsBodyEn = row.SmsBodyEn ?? string.Empty,
                SmsBodyAr = row.SmsBodyAr ?? string.Empty,
            };
        }

        private static List<NotificationTemplateListItemDto> FilterNotificationTemplates(IReadOnlyList<NotificationTemplate> rows, string q)
        {
            var needle = AdminSystemPaging.NormalizeSearch(q);
            if (string.IsNullOrEmpty(needle))
            {
                return rows.Select(MapNotificationTemplateListItem).ToList();
            }

            return rows.Where(r =>
                AdminSystemPaging.NormalizeSearch(r.ActionCode).Contains(needle) ||
                AdminSystemPaging.NormalizeSearch(r.LabelEn).Contains(needle) ||
                AdminSystemPaging.NormalizeSearch(r.LabelAr).Contains(needle))
                .Select(MapNotificationTemplateListItem)
                .ToList();
        }

        private async Task<SystemConfigGroup> FindConfigGroupAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            return await _context.SystemConfigGroups.AsNoTracking()
                .FirstOrDefaultAsync(g => g.Code == code);
        }

        private async Task<SystemConfigGroup> EnsureConfigGroupAsync(string code)
        {
            var group = await _context.SystemConfigGroups.FirstOrDefaultAsync(g => g.Code == code);
            if (group != null)
            {
                return group;
            }

            var utcNow = DateTime.UtcNow;
            group = new SystemConfigGroup
            {
                Code = code,
                NameEn = code,
                NameAr = code,
                SortOrder = 0,
                IsActive = true,
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            };
            _context.SystemConfigGroups.Add(group);
            await _context.SaveChangesAsync();
            return group;
        }

        private static SystemVariableDto MapSystemVariable(SystemVariable row)
        {
            object parsedValue = row.Value;
            if (string.Equals(row.ValueType, "Boolean", StringComparison.OrdinalIgnoreCase))
            {
                parsedValue = bool.TryParse(row.Value, out var b) && b;
            }
            else if (string.Equals(row.ValueType, "Integer", StringComparison.OrdinalIgnoreCase))
            {
                parsedValue = int.TryParse(row.Value, out var n) ? n : 0;
            }
            else if (string.Equals(row.ValueType, "Json", StringComparison.OrdinalIgnoreCase))
            {
                parsedValue = row.Value ?? string.Empty;
            }

            return new SystemVariableDto
            {
                Code = row.VariableKey,
                VariableKey = row.VariableKey,
                VariableName = row.VariableNameEn ?? row.VariableName ?? string.Empty,
                VariableNameEn = row.VariableNameEn ?? row.VariableName ?? string.Empty,
                VariableNameAr = row.VariableNameAr ?? row.VariableName ?? string.Empty,
                ValueType = row.ValueType,
                Value = parsedValue,
                Description = row.DescriptionEn ?? row.Description ?? string.Empty,
                DescriptionEn = row.DescriptionEn ?? row.Description ?? string.Empty,
                DescriptionAr = row.DescriptionAr ?? row.Description ?? string.Empty,
            };
        }

        private static SystemConfigValueDto MapConfigValue(SystemConfigValue row)
        {
            var code = !string.IsNullOrWhiteSpace(row.Code) ? row.Code : row.ValueKey;
            return new SystemConfigValueDto
            {
                id = row.ValueKey,
                code = code,
                label_en = row.LabelEn ?? string.Empty,
                label_ar = row.LabelAr ?? string.Empty,
                IsActive = row.IsActive,
                subReasonDropdown = row.SubReasonDropdown,
                parentReasonId = row.ParentValueKey
            };
        }

        private static SkillCategoryDto MapSkillCategory(CatalogSkillCategory row) =>
            new SkillCategoryDto
            {
                Id = row.Id,
                NameEn = row.NameEn,
                NameAr = row.NameAr,
                SortOrder = row.SortOrder,
                IsActive = row.IsActive
            };

        private static SkillDto MapSkill(CatalogSkill row) =>
            new SkillDto
            {
                Id = row.Id,
                CategoryId = row.CategoryId,
                Kind = row.Kind,
                NameEn = row.NameEn,
                NameAr = row.NameAr,
                IsActive = row.IsActive
            };

        private static string NormalizeSystemVariableValue(string variableKey, string valueType, object value)
        {
            var raw = CoerceSystemVariableValue(value);
            if (string.Equals(variableKey, SystemVariableCodes.NafathValidityCheck, StringComparison.OrdinalIgnoreCase))
            {
                if (!NafathValidityCheck.TryCanonicalize(raw, out var canonical, out var error))
                {
                    throw new SafeExecutionHelper.BadRequestException(
                        "InvalidValue",
                        error ?? "Invalid Nafath validity check.");
                }

                return canonical;
            }

            if (!string.Equals(valueType, "Json", StringComparison.OrdinalIgnoreCase))
            {
                return raw;
            }

            if (string.Equals(variableKey, SystemVariableCodes.EducationDegreeRanks, StringComparison.OrdinalIgnoreCase))
            {
                if (!EducationDegreeRanks.TryCanonicalize(raw, out var canonical, out var error))
                {
                    throw new SafeExecutionHelper.BadRequestException(
                        "InvalidValue",
                        error ?? "Invalid education degree ranks.");
                }

                return canonical;
            }

            try
            {
                JToken.Parse(string.IsNullOrWhiteSpace(raw) ? "[]" : raw);
            }
            catch (Exception)
            {
                throw new SafeExecutionHelper.BadRequestException("InvalidValue", "Invalid JSON value.");
            }

            return raw;
        }

        private static string CoerceSystemVariableValue(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value is string s)
            {
                return s;
            }

            if (value is JToken token)
            {
                return token.ToString(Newtonsoft.Json.Formatting.None);
            }

            return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        private static List<SystemVariableDto> FilterSystemVariables(IReadOnlyList<SystemVariable> rows, string q)
        {
            var needle = AdminSystemPaging.NormalizeSearch(q);
            if (string.IsNullOrEmpty(needle))
            {
                return rows.Select(MapSystemVariable).ToList();
            }

            return rows.Where(r =>
                AdminSystemPaging.NormalizeSearch(r.VariableKey).Contains(needle) ||
                AdminSystemPaging.NormalizeSearch(r.VariableName).Contains(needle) ||
                AdminSystemPaging.NormalizeSearch(r.VariableNameEn).Contains(needle) ||
                AdminSystemPaging.NormalizeSearch(r.VariableNameAr).Contains(needle) ||
                AdminSystemPaging.NormalizeSearch(r.ValueType).Contains(needle) ||
                AdminSystemPaging.NormalizeSearch(r.Value).Contains(needle) ||
                AdminSystemPaging.NormalizeSearch(r.Description).Contains(needle) ||
                AdminSystemPaging.NormalizeSearch(r.DescriptionEn).Contains(needle) ||
                AdminSystemPaging.NormalizeSearch(r.DescriptionAr).Contains(needle))
                .Select(MapSystemVariable)
                .ToList();
        }

        private static List<CatalogSkillCategory> FilterSkillCategories(IReadOnlyList<CatalogSkillCategory> rows, string q)
        {
            var needle = AdminSystemPaging.NormalizeSearch(q);
            if (string.IsNullOrEmpty(needle))
            {
                return rows.ToList();
            }

            return rows.Where(r =>
                AdminSystemPaging.NormalizeSearch(r.NameEn).Contains(needle) ||
                AdminSystemPaging.NormalizeSearch(r.NameAr).Contains(needle)).ToList();
        }

        private static List<CatalogSkill> FilterSkills(IReadOnlyList<CatalogSkill> rows, string q)
        {
            var needle = AdminSystemPaging.NormalizeSearch(q);
            if (string.IsNullOrEmpty(needle))
            {
                return rows.ToList();
            }

            return rows.Where(r =>
                AdminSystemPaging.NormalizeSearch(r.NameEn).Contains(needle) ||
                AdminSystemPaging.NormalizeSearch(r.NameAr).Contains(needle)).ToList();
        }

        public async Task<AdminSystemPagedListDto<SystemAuditTrailRowDto>> GetAuditTrailPageAsync(
            int? pageIndex,
            int? pageSize,
            string module,
            string entityType,
            string entityReference,
            string field,
            string action,
            string source,
            string updatedByUserId,
            DateTime? updatedFrom,
            DateTime? updatedTo,
            string value)
        {
            var query = _context.SystemAuditTrails.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(module))
            {
                var m = module.Trim();
                query = query.Where(r => r.Module == m);
            }

            if (!string.IsNullOrWhiteSpace(entityType))
            {
                var et = entityType.Trim();
                query = query.Where(r => r.EntityType == et);
            }

            if (!string.IsNullOrWhiteSpace(entityReference))
            {
                var er = entityReference.Trim();
                query = query.Where(r => r.EntityReference == er);
            }

            if (!string.IsNullOrWhiteSpace(field))
            {
                var f = field.Trim();
                query = query.Where(r => r.Field == f);
            }

            if (!string.IsNullOrWhiteSpace(action))
            {
                var a = action.Trim();
                query = query.Where(r => r.Action == a);
            }

            if (!string.IsNullOrWhiteSpace(source))
            {
                var s = source.Trim();
                query = query.Where(r => r.Source == s);
            }

            if (!string.IsNullOrWhiteSpace(updatedByUserId))
            {
                var u = updatedByUserId.Trim();
                query = query.Where(r => r.UpdatedByUserId == u);
            }

            if (updatedFrom.HasValue)
            {
                query = query.Where(r => r.UpdatedDate >= updatedFrom.Value);
            }

            if (updatedTo.HasValue)
            {
                query = query.Where(r => r.UpdatedDate <= updatedTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                var v = value.Trim();
                query = query.Where(r => r.OldValue == v || r.NewValue == v);
            }

            var ordered = await query
                .OrderByDescending(r => r.UpdatedDate)
                .ThenByDescending(r => r.Id)
                .ToListAsync();

            var mapped = ordered.Select(MapAuditTrailRow).ToList();
            return AdminSystemPaging.SlicePage(mapped, pageIndex, pageSize);
        }

        public async Task<AdminSystemPagedListDto<SystemAuditValueHistoryRowDto>> GetAuditValueHistoryPageAsync(
            int? pageIndex,
            int? pageSize,
            string module,
            string entityType,
            string entityReference,
            string field,
            string value,
            DateTime? rangeFrom,
            DateTime? rangeTo)
        {
            if (string.IsNullOrWhiteSpace(entityType) ||
                string.IsNullOrWhiteSpace(entityReference) ||
                string.IsNullOrWhiteSpace(field) ||
                string.IsNullOrWhiteSpace(value))
            {
                return AdminSystemPaging.SlicePage(new List<SystemAuditValueHistoryRowDto>(), pageIndex, pageSize);
            }

            var et = entityType.Trim();
            var er = entityReference.Trim();
            var f = field.Trim();
            var v = value.Trim();

            var query = _context.SystemAuditTrails.AsNoTracking()
                .Where(r => r.EntityType == et && r.EntityReference == er && r.Field == f);

            if (!string.IsNullOrWhiteSpace(module))
            {
                var m = module.Trim();
                query = query.Where(r => r.Module == m);
            }

            var changes = await query
                .OrderBy(r => r.UpdatedDate)
                .ThenBy(r => r.Id)
                .ToListAsync();

            var periods = new List<SystemAuditValueHistoryRowDto>();
            for (var i = 0; i < changes.Count; i++)
            {
                var row = changes[i];
                if (!string.Equals(row.NewValue ?? string.Empty, v, StringComparison.Ordinal))
                {
                    continue;
                }

                var validFrom = row.UpdatedDate;
                DateTime? validTo = null;
                if (i + 1 < changes.Count)
                {
                    validTo = changes[i + 1].UpdatedDate;
                }

                if (rangeFrom.HasValue && validTo.HasValue && validTo.Value < rangeFrom.Value)
                {
                    continue;
                }

                if (rangeTo.HasValue && validFrom > rangeTo.Value)
                {
                    continue;
                }

                periods.Add(new SystemAuditValueHistoryRowDto
                {
                    Module = row.Module,
                    EntityType = row.EntityType,
                    EntityReference = row.EntityReference,
                    Field = row.Field,
                    Value = v,
                    ValidFrom = validFrom.ToUniversalTime().ToString("o"),
                    ValidTo = validTo?.ToUniversalTime().ToString("o"),
                    UpdatedByUserId = row.UpdatedByUserId,
                    Source = row.Source,
                });
            }

            periods.Reverse();
            return AdminSystemPaging.SlicePage(periods, pageIndex, pageSize);
        }

        public async Task<AdminSystemPagedListDto<NotificationTransactionLogRowDto>> GetNotificationTransactionLogsPageAsync(
            int? pageIndex,
            int? pageSize,
            string channelType,
            string receiver,
            string actionCode,
            bool? isSuccess,
            DateTime? createdFrom,
            DateTime? createdTo)
        {
            var query = _context.NotificationTransactionLogs.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(channelType))
            {
                var channel = channelType.Trim();
                query = query.Where(r => r.ChannelType == channel);
            }

            if (!string.IsNullOrWhiteSpace(receiver))
            {
                var needle = receiver.Trim().ToLowerInvariant();
                query = query.Where(r => r.Receiver != null && r.Receiver.ToLower().Contains(needle));
            }

            if (!string.IsNullOrWhiteSpace(actionCode))
            {
                var needle = actionCode.Trim().ToLowerInvariant();
                var matchingCodes = await _context.NotificationTemplates.AsNoTracking()
                    .Where(t =>
                        (t.ActionCode != null && t.ActionCode.ToLower().Contains(needle))
                        || (t.LabelEn != null && t.LabelEn.ToLower().Contains(needle))
                        || (t.LabelAr != null && t.LabelAr.ToLower().Contains(needle)))
                    .Select(t => t.ActionCode)
                    .ToListAsync();
                query = query.Where(r =>
                    matchingCodes.Contains(r.ActionCode)
                    || (r.ActionCode != null && r.ActionCode.ToLower().Contains(needle)));
            }

            if (isSuccess.HasValue)
            {
                var ok = isSuccess.Value;
                query = query.Where(r => r.IsSuccess == ok);
            }

            if (createdFrom.HasValue)
            {
                query = query.Where(r => r.CreatedAtUtc >= createdFrom.Value);
            }

            if (createdTo.HasValue)
            {
                query = query.Where(r => r.CreatedAtUtc <= createdTo.Value);
            }

            var ordered = await query
                .OrderByDescending(r => r.CreatedAtUtc)
                .ThenByDescending(r => r.Id)
                .ToListAsync();

            var mapped = ordered.Select(NotificationDispatchService.MapTransactionLog).ToList();
            var codes = mapped
                .Select(r => r.ActionCode)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (codes.Count > 0)
            {
                var labels = await _context.NotificationTemplates.AsNoTracking()
                    .Where(t => codes.Contains(t.ActionCode))
                    .Select(t => new { t.ActionCode, t.LabelEn, t.LabelAr })
                    .ToListAsync();
                var byCode = labels
                    .GroupBy(t => t.ActionCode, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
                foreach (var row in mapped)
                {
                    if (!string.IsNullOrWhiteSpace(row.ActionCode)
                        && byCode.TryGetValue(row.ActionCode, out var label))
                    {
                        row.ActionLabelEn = label.LabelEn;
                        row.ActionLabelAr = label.LabelAr;
                    }
                    else if (string.Equals(row.ActionCode, NotificationTemplateActionCodes.CustomNotification, StringComparison.OrdinalIgnoreCase))
                    {
                        row.ActionLabelEn = "Custom notification";
                        row.ActionLabelAr = "إشعار مخصص";
                    }
                }
            }

            return AdminSystemPaging.SlicePage(mapped, pageIndex, pageSize);
        }

        public Task<NotificationTransactionLogRowDto> ResendNotificationTransactionAsync(long id)
        {
            return _notificationDispatch.ResendFromLogAsync(id);
        }

        private static SystemAuditTrailRowDto MapAuditTrailRow(SystemAuditTrail row)
        {
            return new SystemAuditTrailRowDto
            {
                Id = row.Id,
                Module = row.Module,
                EntityType = row.EntityType,
                EntityReference = row.EntityReference,
                Field = row.Field,
                OldValue = row.OldValue,
                NewValue = row.NewValue,
                Action = row.Action,
                Source = row.Source,
                UpdatedByUserId = row.UpdatedByUserId,
                UpdatedDate = row.UpdatedDate.ToUniversalTime().ToString("o"),
            };
        }

    }
}

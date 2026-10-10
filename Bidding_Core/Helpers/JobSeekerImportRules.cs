using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Bidding_Models;
using Bidding_Models.DatabaseModel;

namespace Bidding_Core.Helpers
{
    /// <summary>
    /// Pure rules for the MOL job seekers Excel import and for claiming an imported profile on registration.
    /// </summary>
    public static class JobSeekerImportRules
    {
        public const string DegreeConfigCode = "QUAL_DEGR";
        public const string InstitutionConfigCode = "EDUC_INST";
        public const string FieldOfStudyConfigCode = "STDY_FILDS";
        public const string DefaultTrainingType = "in-person-program";

        private static readonly HashSet<string> EmptyGrades = new HashSet<string>(StringComparer.Ordinal)
        {
            "لايوجد",
            "لا يوجد",
            "لا ينطبق",
        };

        public sealed class ImportedJobSeekerRow
        {
            public string CivilNo { get; set; }
            public int Year { get; set; }
            public int Quarter { get; set; }
            public string FullName { get; set; }
            public string Gender { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string Age { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string MaritalStatus { get; set; }
            public string HealthStatus { get; set; }
            public string JobSeekerStatus { get; set; }
            public string EmploymentStatus { get; set; }
            public DateTime? FirstRegistration { get; set; }
            public int TotalExperienceYears { get; set; }
            public string GovernorateAr { get; set; }
            public string WilayatAr { get; set; }
            public string EducationLevel { get; set; }
            public string GraduationYear { get; set; }
            public string GraduationCountry { get; set; }
            public string Gpa { get; set; }
            public string Grade { get; set; }
            public string InstitutionAr { get; set; }
            public string SpecializationAr { get; set; }
            public string SpecializationCriteriaAr { get; set; }
            public string ArabicScore { get; set; }
            public string EnglishScore { get; set; }
            public string MathematicsScore { get; set; }
            public string SelectedWorkplace { get; set; }
            public string SelectedJobTitle { get; set; }
            public string SelectedOccupation { get; set; }
            public string WorkExperienceList { get; set; }
            public string TrainingProgramList { get; set; }
        }

        public sealed class ParsedExperience
        {
            public string Organization { get; set; }
            public string Role { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime? ToDate { get; set; }
        }

        public sealed class ParsedTraining
        {
            public string Title { get; set; }
            public string Country { get; set; }
        }

        /// <summary>Maps one sheet row (header → raw cell text) to an import row; null when the civil number is missing.</summary>
        public static ImportedJobSeekerRow MapRow(IReadOnlyDictionary<string, string> cells)
        {
            string Get(string header) =>
                cells != null && cells.TryGetValue(header, out var v) ? Clean(v) : null;

            var civil = NormalizeIntegerText(Get("Civil No"));
            if (string.IsNullOrEmpty(civil))
            {
                return null;
            }

            return new ImportedJobSeekerRow
            {
                CivilNo = civil,
                Year = ParseInt(Get("Year")),
                Quarter = ParseInt(Get("Quarter")),
                FullName = Get("Full Name"),
                Gender = Get("Gender"),
                DateOfBirth = FromExcelDate(Get("Date Of Birth")),
                Age = NormalizeIntegerText(Get("Age")),
                Phone = NormalizeIntegerText(Get("Phone Number")),
                Email = Get("Email"),
                MaritalStatus = Get("Marital Status"),
                HealthStatus = Get("Health Status"),
                JobSeekerStatus = Get("Job Seeker Status"),
                EmploymentStatus = Get("Employment Status"),
                FirstRegistration = FromExcelDate(Get("First Registration")),
                TotalExperienceYears = ParseInt(Get("Total Experience Years")),
                GovernorateAr = Get("Governorate (AR)"),
                WilayatAr = Get("Wilayat (AR)"),
                EducationLevel = Get("Education Level"),
                GraduationYear = NormalizeIntegerText(Get("Graduation Year")),
                GraduationCountry = Get("Graduation Country"),
                Gpa = CleanGpa(Get("GPA")),
                Grade = CleanGrade(Get("Grade")),
                InstitutionAr = Get("Graduation Institution (AR)"),
                SpecializationAr = Get("Specialization (AR)"),
                SpecializationCriteriaAr = Get("Specialization Criteria (AR)"),
                ArabicScore = Get("Arabic Language Subject Score"),
                EnglishScore = Get("English Language Subject Score"),
                MathematicsScore = Get("Mathematics Subject Score"),
                SelectedWorkplace = Get("Selected Workplace"),
                SelectedJobTitle = Get("Selected Job Title"),
                SelectedOccupation = Get("Selected Occupation"),
                WorkExperienceList = cells != null && cells.TryGetValue("Work Experience List", out var w) ? w : null,
                TrainingProgramList = cells != null && cells.TryGetValue("Training Program List", out var t) ? t : null,
            };
        }

        /// <summary>
        /// One row per civil number: the latest year/quarter wins; blank fields fall back to older quarters.
        /// </summary>
        public static List<ImportedJobSeekerRow> PickLatestPerCivilNo(IEnumerable<ImportedJobSeekerRow> rows)
        {
            var result = new List<ImportedJobSeekerRow>();
            foreach (var group in (rows ?? Enumerable.Empty<ImportedJobSeekerRow>())
                .Where(r => r != null && !string.IsNullOrEmpty(r.CivilNo))
                .GroupBy(r => r.CivilNo, StringComparer.Ordinal))
            {
                var ordered = group.OrderByDescending(r => r.Year).ThenByDescending(r => r.Quarter).ToList();
                var latest = ordered[0];
                foreach (var older in ordered.Skip(1))
                {
                    FillBlanks(latest, older);
                }

                result.Add(latest);
            }

            return result;
        }

        /// <summary>Accepts an Excel serial number (e.g. <c>33430</c>) or an ISO date text.</summary>
        public static DateTime? FromExcelDate(string value)
        {
            var text = Clean(value);
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var serial))
            {
                if (serial < 1 || serial > 2958465)
                {
                    return null;
                }

                return DateTime.FromOADate(serial).Date;
            }

            return DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
                ? parsed.Date
                : (DateTime?)null;
        }

        /// <summary>
        /// Lines are <c>workplace | start[ - end]</c> or <c>job title | workplace | start[ - end]</c>.
        /// The open-ended (or latest) job gets the selected job title / occupation as its role.
        /// </summary>
        public static List<ParsedExperience> ParseExperiences(string list, string selectedJobTitle = null, string selectedOccupation = null)
        {
            var result = new List<ParsedExperience>();
            foreach (var line in SplitLines(list))
            {
                var parts = line.Split('|').Select(p => Clean(p)).ToArray();
                if (parts.Length < 2)
                {
                    continue;
                }

                var dates = parts[parts.Length - 1] ?? string.Empty;
                var organization = parts[parts.Length - 2];
                var role = parts.Length >= 3 ? parts[0] : null;
                var match = Regex.Match(dates, @"^(\d{4}-\d{2}-\d{2})(?:\s*-\s*(\d{4}-\d{2}-\d{2}))?$");
                if (string.IsNullOrEmpty(organization) || !match.Success)
                {
                    continue;
                }

                var from = DateTime.ParseExact(match.Groups[1].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime? to = match.Groups[2].Success
                    ? DateTime.ParseExact(match.Groups[2].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                    : (DateTime?)null;
                result.Add(new ParsedExperience
                {
                    Organization = organization,
                    Role = role ?? string.Empty,
                    FromDate = from,
                    ToDate = to,
                });
            }

            var selectedRole = Clean(selectedJobTitle) ?? Clean(selectedOccupation);
            if (selectedRole != null && result.Count > 0)
            {
                var latest = result
                    .OrderBy(e => e.ToDate.HasValue ? 1 : 0)
                    .ThenByDescending(e => e.FromDate)
                    .First();
                if (string.IsNullOrEmpty(latest.Role))
                {
                    latest.Role = selectedRole;
                }
            }

            return result;
        }

        /// <summary>Lines are <c>program | country | Nm</c>; the month count is dropped (always 0 in the source).</summary>
        public static List<ParsedTraining> ParseTrainings(string list)
        {
            var result = new List<ParsedTraining>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var line in SplitLines(list))
            {
                var parts = line.Split('|').Select(p => Clean(p)).ToArray();
                var title = parts.Length > 0 ? parts[0] : null;
                if (string.IsNullOrEmpty(title) || !seen.Add(title))
                {
                    continue;
                }

                result.Add(new ParsedTraining
                {
                    Title = title,
                    Country = parts.Length > 1 ? parts[1] : null,
                });
            }

            return result;
        }

        public static string CleanGrade(string value)
        {
            var text = Clean(value);
            return text == null || EmptyGrades.Contains(text) ? null : text;
        }

        public static string CleanGpa(string value)
        {
            var text = Clean(value);
            if (text == null)
            {
                return null;
            }

            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var gpa) && gpa <= 0
                ? null
                : text;
        }

        /// <summary>MOL education level → <c>QUAL_DEGR</c> value key.</summary>
        public static string MapDegreeValueKey(string level)
        {
            switch ((Clean(level) ?? string.Empty).ToUpperInvariant())
            {
                case "BACHELOR": return "bachelor";
                case "DIPLOMA": return "diploma";
                case "SECONDARY": return "high-school";
                case "MASTER": return "master";
                case "PHD":
                case "DOCTORATE": return "phd";
                default: return null;
            }
        }

        /// <summary>Arabic name key for lookup matching: drops tatweel, the "محافظة" prefix and extra spaces.</summary>
        public static string ArabicLookupKey(string value)
        {
            var text = Clean(value);
            if (text == null)
            {
                return null;
            }

            text = text.Replace("ـ", string.Empty);
            text = Regex.Replace(text, @"\s+", " ").Trim();
            if (text.StartsWith("محافظة ", StringComparison.Ordinal))
            {
                text = text.Substring("محافظة ".Length);
            }

            return text.Length == 0 ? null : text;
        }

        /// <summary>
        /// Turns an imported (unregistered) profile into the registering person's profile.
        /// Wizard state is kept: <see cref="Profile.UserProfileCompleted"/> stays false.
        /// </summary>
        public static void ClaimImportedProfile(
            Profile profile,
            string nameEn,
            string nameAr,
            string email,
            string phone,
            bool isOmani,
            DateTime now)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            profile.Name = nameEn;
            profile.NameAr = nameAr;
            profile.CompanyNameEnglish = nameEn;
            profile.CompanyNameArabic = nameAr;
            profile.EmailAddress = email;
            profile.PhoneNumber = phone;
            profile.PreferredLanguage = "en";
            profile.IsActive = true;
            profile.IsOmani = isOmani;
            profile.JoinDate = now;
            profile.ProfileType = (int)Enums.ProfileTypes.JobSeeker;
            profile.IsPostOpertunity = false;
            profile.UserProfileCompleted = false;
            profile.MissionVision ??= string.Empty;
            profile.AboutUs ??= string.Empty;
            profile.IsRegistered = true;
            profile.UpdatedDate = now;
        }

        public static string FormatDate(DateTime? value) =>
            value.HasValue ? value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null;

        public static string Clip(string value, int max)
        {
            var text = Clean(value);
            if (text == null)
            {
                return null;
            }

            return text.Length <= max ? text : text.Substring(0, max);
        }

        private static void FillBlanks(ImportedJobSeekerRow target, ImportedJobSeekerRow source)
        {
            foreach (var prop in typeof(ImportedJobSeekerRow).GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    if (string.IsNullOrWhiteSpace((string)prop.GetValue(target)))
                    {
                        prop.SetValue(target, prop.GetValue(source));
                    }
                }
                else if (prop.PropertyType == typeof(DateTime?))
                {
                    if (prop.GetValue(target) == null)
                    {
                        prop.SetValue(target, prop.GetValue(source));
                    }
                }
            }
        }

        private static IEnumerable<string> SplitLines(string list) =>
            (list ?? string.Empty)
                .Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => l.Length > 0);

        private static string Clean(string value)
        {
            if (value == null)
            {
                return null;
            }

            var text = value.Replace('\u00A0', ' ').Trim();
            return text.Length == 0 ? null : text;
        }

        private static string NormalizeIntegerText(string value)
        {
            var text = Clean(value);
            if (text == null)
            {
                return null;
            }

            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var number)
                && Math.Abs(number - Math.Round(number)) < 1e-9
                && Math.Abs(number) < 1e15)
            {
                return ((long)Math.Round(number)).ToString(CultureInfo.InvariantCulture);
            }

            return text;
        }

        private static int ParseInt(string value) =>
            int.TryParse(NormalizeIntegerText(value), NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? n : 0;
    }
}

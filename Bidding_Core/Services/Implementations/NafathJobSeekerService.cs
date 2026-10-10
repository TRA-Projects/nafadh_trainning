using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bidding_Core.Services.Interfaces;
using Bidding_Models.DTOs;
using Bidding_Models.Jobseekers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bidding_Core.Services.Implementations
{
    /// <summary>
    /// Nafath JobSeeker MOL details client.
    /// Connection: <c>NafathJobSeeker:BaseUrl</c> URL template + <c>AppId</c>/<c>AppPassword</c> (appsettings).
    /// Placeholders in BaseUrl (replaced only when present): <c>{civil_id}</c>, <c>{user}</c>, <c>{password}</c>.
    /// Omit placeholders to hardcode a full troubleshooting URL.
    /// Toggles: system variables <c>NAFATH_REGISTRATION_CHECK_ENABLED</c>, <c>NAFATH_USE_MOCK</c>.
    /// </summary>
    public class NafathJobSeekerService : INafathJobSeekerService
    {
        public const string MockCivilIdEmpty = "00000000";
        public const string MockCivilIdActive = "11111111";
        public const string MockCivilIdExpired = "22222222";

        private const string ConfigSection = "NafathJobSeeker";
        private const string PlaceholderCivilId = "{civil_id}";
        private const string PlaceholderUser = "{user}";
        private const string PlaceholderPassword = "{password}";
        private const string ActiveFixtureResource = "Bidding_Core.Fixtures.nafath-jobseeker-active.json";
        private static readonly DateTime DefaultRegistrationDate = new DateTime(1, 1, 1);

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAdminSystemService _adminSystemService;
        private readonly ILogger<NafathJobSeekerService> _logger;

        public NafathJobSeekerService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IAdminSystemService adminSystemService,
            ILogger<NafathJobSeekerService> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _adminSystemService = adminSystemService;
            _logger = logger;
        }

        public Task<bool> IsRegistrationCheckEnabledAsync() =>
            IsSystemFlagEnabledAsync(SystemVariableCodes.NafathRegistrationCheckEnabled, defaultEnabled: true);

        public Task<bool> IsUseMockAsync() =>
            IsSystemFlagEnabledAsync(SystemVariableCodes.NafathUseMock, defaultEnabled: true);

        public async Task<NafathJobSeekerFetchResult> GetDetailsByCivilIdAsync(
            string civilId,
            CancellationToken cancellationToken = default)
        {
            var trimmed = (civilId ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return new NafathJobSeekerFetchResult
                {
                    Success = false,
                    ErrorMessage = "Civil ID is required.",
                    HasData = false,
                    IsActiveJobSeeker = false,
                };
            }

            var validity = await GetValidityCheckModeAsync();
            if (await IsUseMockAsync())
            {
                var mock = BuildMock(trimmed, validity);
                _logger.LogInformation(
                    "NafathJobSeeker MOCK civilNo={CivilId} fixture={Fixture} hasData={HasData} isActive={IsActive} validity={Validity} response={Response}",
                    trimmed,
                    mock.MockFixture,
                    mock.HasData,
                    mock.IsActiveJobSeeker,
                    validity,
                    JsonConvert.SerializeObject(mock.Data));
                return mock;
            }

            return await FetchLiveAsync(trimmed, validity, cancellationToken);
        }

        public async Task ApplyEducationToMolSummaryAsync(FreelancerProfileFromMolDto dto, NafathJobSeekerDetailsDto data)
        {
            var json = await _adminSystemService.GetSystemVariableValueAsync(SystemVariableCodes.EducationDegreeRanks);
            ApplyEducationToMolSummary(dto, data, EducationDegreeRanks.Parse(json));
        }

        private async Task<bool> IsSystemFlagEnabledAsync(string variableKey, bool defaultEnabled)
        {
            var raw = await _adminSystemService.GetSystemVariableValueAsync(variableKey);
            if (string.IsNullOrWhiteSpace(raw))
            {
                return defaultEnabled;
            }

            return ParseBool(raw, defaultEnabled);
        }

        private async Task<NafathValidityCheckMode> GetValidityCheckModeAsync()
        {
            var raw = await _adminSystemService.GetSystemVariableValueAsync(SystemVariableCodes.NafathValidityCheck);
            return NafathValidityCheck.Parse(raw);
        }

        private async Task<NafathJobSeekerFetchResult> FetchLiveAsync(
            string civilId,
            NafathValidityCheckMode validity,
            CancellationToken cancellationToken)
        {
            var urlTemplate = (_configuration[$"{ConfigSection}:BaseUrl"] ?? string.Empty).Trim();
            var appId = (_configuration[$"{ConfigSection}:AppId"] ?? string.Empty).Trim();
            var appPassword = (_configuration[$"{ConfigSection}:AppPassword"] ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(urlTemplate))
            {
                _logger.LogError(
                    "NafathJobSeeker settings incomplete. BaseUrl required. civilNo={CivilId}",
                    civilId);
                return Fail("Failed to fetch jobseekers data.");
            }

            if (urlTemplate.IndexOf(PlaceholderUser, StringComparison.OrdinalIgnoreCase) >= 0 &&
                string.IsNullOrWhiteSpace(appId))
            {
                _logger.LogError(
                    "NafathJobSeeker settings incomplete. AppId required when BaseUrl contains {user}. civilNo={CivilId}",
                    civilId);
                return Fail("Failed to fetch jobseekers data.");
            }

            if (urlTemplate.IndexOf(PlaceholderPassword, StringComparison.OrdinalIgnoreCase) >= 0 &&
                string.IsNullOrWhiteSpace(appPassword))
            {
                _logger.LogError(
                    "NafathJobSeeker settings incomplete. AppPassword required when BaseUrl contains {password}. civilNo={CivilId}",
                    civilId);
                return Fail("Failed to fetch jobseekers data.");
            }

            var url = ApplyUrlTemplate(urlTemplate, civilId, appId, appPassword, redactPassword: false);
            var safeUrl = ApplyUrlTemplate(urlTemplate, civilId, appId, appPassword, redactPassword: true);
            _logger.LogInformation("NafathJobSeeker REQUEST GET {SafeUrl}", safeUrl);

            try
            {
                var client = _httpClientFactory.CreateClient("NafathJobSeeker");
                // Do not bind to the inbound request token: Angular abort (~old 15s timeout)
                // cancelled this call while the MOL URL still returned data in the browser.
                using var response = await client.GetAsync(url, CancellationToken.None);
                var body = await response.Content.ReadAsStringAsync();

                _logger.LogInformation(
                    "NafathJobSeeker RESPONSE civilNo={CivilId} status={StatusCode} bodyLength={BodyLength} body={Body}",
                    civilId,
                    (int)response.StatusCode,
                    body?.Length ?? 0,
                    body);

                if (!response.IsSuccessStatusCode)
                {
                    return Fail("Failed to fetch jobseekers data.");
                }

                var data = JsonConvert.DeserializeObject<NafathJobSeekerDetailsDto>(body) ?? new NafathJobSeekerDetailsDto();
                return Evaluate(data, mockFixture: null, validity);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "NafathJobSeeker CANCELLED/TIMEOUT civilNo={CivilId} url={SafeUrl}", civilId, safeUrl);
                return Fail("Failed to fetch jobseekers data.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NafathJobSeeker CONNECTION FAILED civilNo={CivilId} url={SafeUrl}", civilId, safeUrl);
                return Fail("Failed to fetch jobseekers data.");
            }
        }

        /// <summary>
        /// Replaces <c>{civil_id}</c>, <c>{user}</c>, <c>{password}</c> only when present (case-insensitive).
        /// No auto query-string append — paste a full hardcoded URL with no placeholders to troubleshoot.
        /// </summary>
        private static string ApplyUrlTemplate(
            string template,
            string civilId,
            string appId,
            string appPassword,
            bool redactPassword)
        {
            var url = template ?? string.Empty;

            if (url.IndexOf(PlaceholderCivilId, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                url = ReplaceIgnoreCase(url, PlaceholderCivilId, Uri.EscapeDataString(civilId ?? string.Empty));
            }

            if (url.IndexOf(PlaceholderUser, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                url = ReplaceIgnoreCase(url, PlaceholderUser, Uri.EscapeDataString(appId ?? string.Empty));
            }

            if (url.IndexOf(PlaceholderPassword, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var passwordValue = redactPassword
                    ? "***"
                    : Uri.EscapeDataString(appPassword ?? string.Empty);
                url = ReplaceIgnoreCase(url, PlaceholderPassword, passwordValue);
            }

            return url;
        }

        private static string ReplaceIgnoreCase(string input, string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(oldValue))
            {
                return input;
            }

            var replacement = newValue ?? string.Empty;
            var result = input;
            while (true)
            {
                var index = result.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
                if (index < 0)
                {
                    return result;
                }

                result = result.Substring(0, index) + replacement + result.Substring(index + oldValue.Length);
            }
        }

        private NafathJobSeekerFetchResult BuildMock(string civilId, NafathValidityCheckMode validity)
        {
            string fixture;
            NafathJobSeekerDetailsDto data;

            if (string.Equals(civilId, MockCivilIdEmpty, StringComparison.Ordinal))
            {
                fixture = "empty";
                data = CreateEmptyFixture();
            }
            else if (string.Equals(civilId, MockCivilIdExpired, StringComparison.Ordinal))
            {
                fixture = "expired";
                data = CreateExpiredFixture(civilId);
            }
            else
            {
                // Any other civil ID → active (active_Flag Y and a future exp_Active_Date)
                fixture = "active";
                data = CreateActiveFixture(civilId);
            }

            var result = Evaluate(data, fixture, validity);
            result.Success = true;
            return result;
        }

        private static NafathJobSeekerDetailsDto CreateEmptyFixture()
        {
            return new NafathJobSeekerDetailsDto
            {
                ActiveDate = "null",
                ActActivityId = "null",
                CategoryCode = "null",
                CurrentEmpDate = "null",
                DateOfBirth = "null",
                EducationId = "0",
                ExpireDate = "null",
                ExpActiveDate = "null",
                GsmNo = "null",
                HandicapType = "null",
                HealthStatus = "null",
                IssueDate = "null",
                JobDate = "null",
                JobStatus = "null",
                TranEducationDet = new List<NafathEducationDetDto>(),
                MaritialStatus = "null",
                PerRegionCode = "null",
                PerVillageCode = "null",
                PerWilayatCode = "null",
                RegistrationDate = "0001-01-01T00:00:00",
                RegistrationNo = "null",
                ResRegionCode = "null",
                ResVillageCode = "null",
                ResWilayatCode = "null",
                SocialBenefitNumber = "null",
                SponsorNo = "null",
                SubCategoryCode = "null",
                TelNo1 = "null",
                TelNo2 = "null",
            };
        }

        private static NafathJobSeekerDetailsDto CreateActiveFixture(string civilId)
        {
            // Privacy-scrubbed sample from a real Nafath JobSeekerDetails payload (see Fixtures/).
            var json = ReadEmbeddedResource(ActiveFixtureResource);
            var data = JsonConvert.DeserializeObject<NafathJobSeekerDetailsDto>(json)
                       ?? new NafathJobSeekerDetailsDto();
            data.NationalId = civilId;
            data.ActiveFlag = "Y";
            if (NormalizeField(data.ActiveDescArb) == null)
            {
                data.ActiveDescArb = "نشط";
            }
            data.ExpActiveDate = DateTime.UtcNow.AddMonths(6).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            return data;
        }

        private static NafathJobSeekerDetailsDto CreateExpiredFixture(string civilId)
        {
            var active = CreateActiveFixture(civilId);
            active.ExpActiveDate = DateTime.UtcNow.AddMonths(-6).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            active.ExpireDate = DateTime.UtcNow.AddMonths(-6).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            active.ActiveFlag = "N";
            active.ActiveDescArb = "غير نشط";
            active.JobStatus = "0";
            active.JobStatusDesc = "منتهي";
            return active;
        }

        /// <summary>
        /// Primary specialization (التخصص الرئيسي) from education rows:
        /// prefer <c>NIMR_MOHE_MAJOR_QLFN_DESC</c>, else <c>NIMR_MOHE_MINOR_QLFN_DESC</c>.
        /// Picks the last education row that has either value (highest listed qualification in MOL samples).
        /// </summary>
        public static string ExtractPrimarySpecificationAr(NafathJobSeekerDetailsDto data)
        {
            var rows = data?.TranEducationDet;
            if (rows == null || rows.Count == 0)
            {
                return null;
            }

            for (var i = rows.Count - 1; i >= 0; i--)
            {
                var row = rows[i];
                if (row == null)
                {
                    continue;
                }

                var major = NormalizeField(row.NimrMoheMajorQlfnDesc);
                if (major != null)
                {
                    return major;
                }

                var minor = NormalizeField(row.NimrMoheMinorQlfnDesc);
                if (minor != null)
                {
                    return minor;
                }
            }

            return null;
        }

        /// <summary>
        /// Highest usable education row for the readonly higher-education summary.
        /// When <paramref name="ranks"/> is provided, picks the lowest rank (1 = highest) among matching rows.
        /// Ties use later graduation year, then later list index. Unmatched config falls back to last usable row.
        /// </summary>
        public static NafathEducationDetDto SelectPrimaryEducationRow(
            NafathJobSeekerDetailsDto data,
            IReadOnlyList<EducationDegreeRankLevel> ranks = null)
        {
            var rows = data?.TranEducationDet;
            if (rows == null || rows.Count == 0)
            {
                return null;
            }

            if (ranks != null && ranks.Count > 0)
            {
                NafathEducationDetDto best = null;
                var bestRank = int.MaxValue;
                var bestYear = int.MinValue;
                var bestIndex = -1;

                for (var i = 0; i < rows.Count; i++)
                {
                    var row = rows[i];
                    if (row == null)
                    {
                        continue;
                    }

                    var rank = EducationDegreeRanks.RankForLabel(ranks, row.EducationDescArb, row.EducationCode);
                    if (rank == null)
                    {
                        continue;
                    }

                    var year = GraduationYearSortKey(row);
                    if (rank.Value < bestRank
                        || (rank.Value == bestRank && year > bestYear)
                        || (rank.Value == bestRank && year == bestYear && i > bestIndex))
                    {
                        best = row;
                        bestRank = rank.Value;
                        bestYear = year;
                        bestIndex = i;
                    }
                }

                if (best != null)
                {
                    return best;
                }
            }

            for (var i = rows.Count - 1; i >= 0; i--)
            {
                var row = rows[i];
                if (row == null)
                {
                    continue;
                }

                if (NormalizeField(row.NimrMoheUniversityDesc) != null
                    || NormalizeField(row.NimrMoheMajorQlfnDesc) != null
                    || NormalizeField(row.NimrMoheMinorQlfnDesc) != null
                    || NormalizeField(row.EducationDescArb) != null
                    || ExtractGraduationYear(row.GraduationDate) != null)
                {
                    return row;
                }
            }

            return null;
        }

        /// <summary>Map Nafath education into MOL profile summary education fields (diploma subject marks left empty when absent).</summary>
        public static void ApplyEducationToMolSummary(
            FreelancerProfileFromMolDto dto,
            NafathJobSeekerDetailsDto data,
            IReadOnlyList<EducationDegreeRankLevel> ranks = null)
        {
            if (dto == null || data == null)
            {
                return;
            }

            var row = SelectPrimaryEducationRow(data, ranks);
            if (row == null)
            {
                dto.EducationLevel = NormalizeField(data.EducationDescArb) ?? dto.EducationLevel;
                return;
            }

            dto.EducationalInstitutions = NormalizeField(row.NimrMoheUniversityDesc) ?? dto.EducationalInstitutions;
            dto.UniversityMajor =
                NormalizeField(row.NimrMoheMajorQlfnDesc)
                ?? NormalizeField(row.NimrMoheMinorQlfnDesc)
                ?? dto.UniversityMajor;
            dto.EducationLevel =
                NormalizeField(row.EducationDescArb)
                ?? NormalizeField(data.EducationDescArb)
                ?? dto.EducationLevel;
            dto.CountryOfGraduation =
                NormalizeField(row.GraduationCountryDescArb)
                ?? NormalizeField(row.GraduationCountryCode)
                ?? dto.CountryOfGraduation;
            dto.YearOfGraduation = ExtractGraduationYear(row.GraduationDate) ?? dto.YearOfGraduation;
            dto.Grade =
                NormalizeField(row.GpaSch)
                ?? NormalizeField(row.NmrGradesDescArb)
                ?? NormalizeField(row.NimrMoheGradsDesc)
                ?? dto.Grade;
        }

        /// <summary>
        /// Build profile-completion education records from Nafath <c>tranEducationDet</c>.
        /// Rows without a degree label/code are skipped. Missing start year falls back to end year.
        /// </summary>
        public static List<JobSeekerEducationRecordDto> MapEducationRecordsFromNafath(NafathJobSeekerDetailsDto data)
        {
            var result = new List<JobSeekerEducationRecordDto>();
            var rows = data?.TranEducationDet;
            if (rows == null || rows.Count == 0)
            {
                return result;
            }

            foreach (var row in rows)
            {
                if (row == null)
                {
                    continue;
                }

                var degreeKey = NormalizeField(row.EducationDescArb) ?? NormalizeField(row.EducationCode);
                var endYear = ExtractGraduationYear(row.GraduationDate);
                if (degreeKey == null || endYear == null)
                {
                    continue;
                }

                var startYear = ExtractGraduationYear(row.StartDt) ?? endYear;
                var institution = NormalizeField(row.NimrMoheUniversityDesc) ?? string.Empty;
                var major = NormalizeField(row.NimrMoheMajorQlfnDesc);
                var minor = NormalizeField(row.NimrMoheMinorQlfnDesc);
                var majorId = NormalizeField(row.NimrMoheMajorQlfnId);
                var country = NormalizeField(row.GraduationCountryDescArb);

                result.Add(new JobSeekerEducationRecordDto
                {
                    Institution = institution,
                    // Free-text from MOL — leave lookup empty so we don't invent EDUC_INST keys.
                    InstitutionLookup = null,
                    Degree = degreeKey.Length > 64 ? degreeKey.Substring(0, 64) : degreeKey,
                    FieldOfStudy = major ?? string.Empty,
                    FieldOfStudyLookup = major == null
                        ? null
                        : new ConfigLookupValueDto
                        {
                            id = majorId ?? major,
                            label_en = major,
                            label_ar = major,
                        },
                    SubSpecialty = minor ?? string.Empty,
                    StartYear = startYear,
                    EndYear = endYear,
                    CumulativeGpa = NormalizeField(row.GpaSch) ?? string.Empty,
                    Grade = NormalizeField(row.NmrGradesDescArb)
                            ?? NormalizeField(row.NimrMoheGradsDesc)
                            ?? string.Empty,
                    Description = country == null
                        ? (NormalizeField(row.EducationCode) ?? string.Empty)
                        : $"{NormalizeField(row.EducationCode) ?? string.Empty} — {country}".Trim(' ', '—'),
                    FromIntegration = true,
                });
            }

            return result;
        }

        /// <summary>
        /// Map Nafath <c>listOfExp</c> into experience rows. Skips null/empty lists and rows missing role or start date.
        /// Deduplicates identical sponsor/role/start/end combinations from MOL samples.
        /// </summary>
        public static List<JobSeekerExperienceRowDto> MapExperienceRecordsFromNafath(NafathJobSeekerDetailsDto data)
        {
            var result = new List<JobSeekerExperienceRowDto>();
            var rows = data?.ListOfExp;
            if (rows == null || rows.Count == 0)
            {
                return result;
            }

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var row in rows)
            {
                if (row == null)
                {
                    continue;
                }

                var role = NormalizeField(row.OccupDesc) ?? NormalizeField(row.DesignationDesc);
                var fromDate = FormatDateOnly(row.StartDate);
                if (role == null || fromDate == null)
                {
                    continue;
                }

                var organization = NormalizeField(row.SponsorName) ?? string.Empty;
                var toDate = FormatDateOnly(row.EndDate);
                var key = $"{organization}|{role}|{fromDate}|{toDate ?? ""}";
                if (!seen.Add(key))
                {
                    continue;
                }

                result.Add(new JobSeekerExperienceRowDto
                {
                    Organization = organization,
                    Role = role,
                    FromDate = fromDate,
                    ToDate = toDate ?? string.Empty,
                    SkillNames = new List<string>(),
                    TechStack = new List<string>(),
                    FromIntegration = true,
                });
            }

            return result;
        }

        /// <summary>
        /// Map Nafath <c>listOfTrain</c> into training rows. Skips null/empty lists and rows missing title or a usable date.
        /// </summary>
        public static List<JobSeekerTrainingRowDto> MapTrainingRecordsFromNafath(NafathJobSeekerDetailsDto data)
        {
            var result = new List<JobSeekerTrainingRowDto>();
            var rows = data?.ListOfTrain;
            if (rows == null || rows.Count == 0)
            {
                return result;
            }

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var row in rows)
            {
                if (row == null)
                {
                    continue;
                }

                var title = NormalizeField(row.TrainingCourseDescArb);
                var completed = FormatDateOnly(row.TrainingTo) ?? FormatDateOnly(row.TrainingFrom);
                if (title == null || completed == null)
                {
                    continue;
                }

                var provider = NormalizeField(row.TrainingCountryDescArb) ?? "MOL";
                var key = $"{title}|{provider}|{completed}";
                if (!seen.Add(key))
                {
                    continue;
                }

                var trainingType = title.IndexOf("On Job", StringComparison.OrdinalIgnoreCase) >= 0
                    || title.IndexOf("OJT", StringComparison.OrdinalIgnoreCase) >= 0
                    ? "ojt"
                    : "in-person-program";

                result.Add(new JobSeekerTrainingRowDto
                {
                    Title = title,
                    Provider = provider,
                    ProviderLookup = null,
                    CompletedDate = completed,
                    TrainingType = trainingType,
                    SkillNames = new List<string>(),
                    TechStack = new List<string>(),
                    FromIntegration = true,
                });
            }

            return result;
        }

        /// <summary>
        /// Map Nafath <c>listOfLang</c> into profile skill rows (name + proficiency level text).
        /// Skips null/empty lists and rows without a language name.
        /// </summary>
        public static List<(string SkillName, string SkillLevel)> MapLanguageSkillsFromNafath(NafathJobSeekerDetailsDto data)
        {
            var result = new List<(string SkillName, string SkillLevel)>();
            var rows = data?.ListOfLang;
            if (rows == null || rows.Count == 0)
            {
                return result;
            }

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var row in rows)
            {
                if (row == null)
                {
                    continue;
                }

                var name = NormalizeField(row.LanguageDescArb);
                if (name == null || !seen.Add(name))
                {
                    continue;
                }

                var parts = new List<string>();
                var speak = NormalizeField(row.LangSpeakDescArb);
                var read = NormalizeField(row.LangReadDescArb);
                var write = NormalizeField(row.LangWriteDescArb);
                if (speak != null)
                {
                    parts.Add($"Speak: {speak}");
                }

                if (read != null)
                {
                    parts.Add($"Read: {read}");
                }

                if (write != null)
                {
                    parts.Add($"Write: {write}");
                }

                result.Add((name, parts.Count == 0 ? string.Empty : string.Join(" · ", parts)));
            }

            return result;
        }

        /// <summary>Normalize MOL date strings to <c>yyyy-MM-dd</c> (returns null for empty / "null").</summary>
        public static string FormatDateOnly(string raw)
        {
            var parsed = TryParseMolDate(raw);
            return parsed?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        /// <summary>Normalize MOL date strings to <c>dd/MM/yyyy</c> for profile display.</summary>
        public static string FormatDisplayDate(string raw)
        {
            var parsed = TryParseMolDate(raw);
            return parsed?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        public static DateTime? TryParseMolDate(string raw)
        {
            var value = NormalizeField(raw);
            if (value == null)
            {
                return null;
            }

            if (DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var display)
                || DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out display)
                || DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out display)
                || DateTime.TryParse(value, out display))
            {
                return display;
            }

            return null;
        }

        /// <summary>Extract a 4-digit year from MOL graduation / start values.</summary>
        public static string ExtractGraduationYear(string raw)
        {
            var value = NormalizeField(raw);
            if (value == null)
            {
                return null;
            }

            if (value.Length == 4 && int.TryParse(value, out _))
            {
                return value;
            }

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt)
                || DateTime.TryParse(value, out dt))
            {
                return dt.Year.ToString(CultureInfo.InvariantCulture);
            }

            var match = System.Text.RegularExpressions.Regex.Match(value, @"\b(19|20)\d{2}\b");
            return match.Success ? match.Value : null;
        }

        private static int GraduationYearSortKey(NafathEducationDetDto row)
        {
            var year = ExtractGraduationYear(row?.GraduationDate);
            return int.TryParse(year, out var n) ? n : int.MinValue;
        }

        /// <summary>Join English name parts from Nafath payload (first + second + third + tribe).</summary>
        public static string FormatFullNameEn(NafathJobSeekerDetailsDto data)
        {
            if (data == null)
            {
                return null;
            }

            return JoinNameParts(
                NormalizeField(data.NmrFirstNameEng),
                NormalizeField(data.NmrSecondNameEng),
                NormalizeField(data.NmrThirdNameEng),
                NormalizeField(data.NmrTribeNameEng));
        }

        /// <summary>Join Arabic name parts from Nafath payload.</summary>
        public static string FormatFullNameAr(NafathJobSeekerDetailsDto data)
        {
            if (data == null)
            {
                return null;
            }

            return JoinNameParts(
                NormalizeField(data.NmrFirstName),
                NormalizeField(data.NmrSecondName),
                NormalizeField(data.NmrThirdName),
                NormalizeField(data.NmrTribeName));
        }

        /// <summary>
        /// Permanent (then residence) village, wilayat, and governorate from Nafath — MOL has no street line.
        /// </summary>
        public static string FormatAddress(NafathJobSeekerDetailsDto data)
        {
            if (data == null)
            {
                return null;
            }

            var village = NormalizeField(data.PerVillageDescArb)
                          ?? NormalizeField(data.ResVillageDescArb);
            var wilayat = NormalizeField(data.PerWilayatDescArb)
                          ?? NormalizeField(data.ResWilayatDescArb)
                          ?? NormalizeField(data.PerWilayatCode)
                          ?? NormalizeField(data.ResWilayatCode);
            var governorate = NormalizeField(data.PerRegionDesc)
                              ?? NormalizeField(data.ResRegionDesc)
                              ?? NormalizeField(data.PerRegionCode)
                              ?? NormalizeField(data.ResRegionCode);

            var parts = new[] { village, wilayat, governorate }.Where(p => p != null);
            var joined = string.Join("، ", parts);
            return string.IsNullOrWhiteSpace(joined) ? null : joined;
        }

        private static string JoinNameParts(params string[] parts)
        {
            var joined = string.Join(
                " ",
                (parts ?? Array.Empty<string>()).Where(p => !string.IsNullOrWhiteSpace(p)));
            return string.IsNullOrWhiteSpace(joined) ? null : joined;
        }

        private static string ReadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                var available = string.Join(", ", assembly.GetManifestResourceNames());
                throw new InvalidOperationException(
                    $"Embedded resource '{resourceName}' not found. Available: {available}");
            }

            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            return reader.ReadToEnd();
        }

        private static NafathJobSeekerFetchResult Evaluate(
            NafathJobSeekerDetailsDto data,
            string mockFixture,
            NafathValidityCheckMode validity)
        {
            var hasData = HasMeaningfulData(data);
            var isActive = hasData && IsActiveJobSeekerRecord(data, validity);
            return new NafathJobSeekerFetchResult
            {
                Success = true,
                Data = data,
                HasData = hasData,
                IsActiveJobSeeker = isActive,
                InactiveDetail = isActive || !hasData ? null : FormatInactiveDetail(data, validity),
                MockFixture = mockFixture,
            };
        }

        private static NafathJobSeekerFetchResult Fail(string message) =>
            new NafathJobSeekerFetchResult
            {
                Success = false,
                ErrorMessage = message,
                HasData = false,
                IsActiveJobSeeker = false,
            };

        internal static bool HasMeaningfulData(NafathJobSeekerDetailsDto data)
        {
            if (data == null)
            {
                return false;
            }

            // Empty sample from MOL: almost every field is the string "null" / empty, education_Id "0",
            // registration_Date default, empty tranEducationDet.
            var candidates = new[]
            {
                data.ExpActiveDate,
                data.ActiveDate,
                data.ActiveFlag,
                data.RegistrationNo,
                data.DateOfBirth,
                data.GsmNo,
                data.JobStatus,
                data.JobStatusDesc,
                data.CategoryCode,
                data.ActActivityId,
                data.HealthStatus,
                data.MaritialStatus,
                data.PerRegionCode,
                data.PerWilayatCode,
                data.CurrentEmpDate,
                data.JobDate,
                data.NmrFirstNameEng,
                data.NmrFirstName,
                data.NationalId,
            };

            if (candidates.Any(HasValue))
            {
                return true;
            }

            if (HasValue(data.RegistrationDate) &&
                !string.Equals(data.RegistrationDate.Trim(), "0001-01-01T00:00:00", StringComparison.OrdinalIgnoreCase) &&
                !(DateTime.TryParse(data.RegistrationDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var reg) &&
                  reg.Date == DefaultRegistrationDate))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Active according to <paramref name="mode"/> (default: flag Y and expiry after UTC today).
        /// </summary>
        internal static string FormatInactiveDetail(
            NafathJobSeekerDetailsDto data,
            NafathValidityCheckMode mode)
        {
            var flagRaw = NormalizeField(data?.ActiveFlag) ?? string.Empty;
            var expiryDisplay = FormatDisplayDate(data?.ExpActiveDate)
                                ?? NormalizeField(data?.ExpActiveDate)
                                ?? string.Empty;
            var flagOk = IsEnabledYnFlag(data?.ActiveFlag);
            var expiryOk = IsFutureDate(data?.ExpActiveDate);
            var flagLabel = string.IsNullOrEmpty(flagRaw) ? "not set" : flagRaw;
            var expiryLabel = string.IsNullOrEmpty(expiryDisplay) ? "not set" : expiryDisplay;

            var parts = new List<string> { NafathJobSeekerErrorCodes.InactiveMessage };
            if (mode == NafathValidityCheckMode.ActiveFlag || mode == NafathValidityCheckMode.Both)
            {
                parts.Add(flagOk
                    ? $"Active flag found: {flagLabel}."
                    : $"Active flag found: {flagLabel} (expected Y).");
            }

            if (mode == NafathValidityCheckMode.ExpiryDate || mode == NafathValidityCheckMode.Both)
            {
                parts.Add(expiryOk
                    ? $"Expiry date found: {expiryLabel}."
                    : $"Expiry date found: {expiryLabel} (must be after today).");
            }

            parts.Add(
                $"|flag={flagRaw}|flagOk={(flagOk ? "1" : "0")}|expiry={expiryDisplay}|expiryOk={(expiryOk ? "1" : "0")}|mode={NafathValidityCheck.ToCode(mode)}");
            return string.Join(" ", parts);
        }

        internal static bool IsActiveJobSeekerRecord(
            NafathJobSeekerDetailsDto data,
            NafathValidityCheckMode mode = NafathValidityCheckMode.Both)
        {
            if (data == null)
            {
                return false;
            }

            switch (mode)
            {
                case NafathValidityCheckMode.None:
                    return true;
                case NafathValidityCheckMode.ExpiryDate:
                    return IsFutureDate(data.ExpActiveDate);
                case NafathValidityCheckMode.ActiveFlag:
                    return IsEnabledYnFlag(data.ActiveFlag);
                default:
                    return IsEnabledYnFlag(data.ActiveFlag) && IsFutureDate(data.ExpActiveDate);
            }
        }

        internal static bool IsFutureDate(string raw)
        {
            if (!TryParseFlexibleDate(raw, out var date))
            {
                return false;
            }

            return date.Date > DateTime.UtcNow.Date;
        }

        private static bool IsEnabledYnFlag(string raw)
        {
            var value = NormalizeField(raw);
            if (value == null)
            {
                return false;
            }

            return ParseBool(value, defaultValue: false);
        }

        public static string NormalizeField(string raw)
        {
            if (!HasValue(raw))
            {
                return null;
            }

            return raw.Trim();
        }

        private static bool HasValue(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return false;
            }

            var trimmed = raw.Trim();
            return !string.Equals(trimmed, "null", StringComparison.OrdinalIgnoreCase) &&
                   !string.Equals(trimmed, "undefined", StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryParseFlexibleDate(string raw, out DateTime date)
        {
            date = default;
            if (!HasValue(raw))
            {
                return false;
            }

            var formats = new[]
            {
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm:ss.fff",
                "yyyy-MM-dd",
                "dd/MM/yyyy",
                "MM/dd/yyyy",
            };

            if (DateTime.TryParseExact(
                    raw.Trim(),
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out date))
            {
                return true;
            }

            return DateTime.TryParse(
                raw.Trim(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out date);
        }

        private static bool ParseBool(string raw, bool defaultValue)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return defaultValue;
            }

            if (bool.TryParse(raw, out var b))
            {
                return b;
            }

            if (string.Equals(raw.Trim(), "Y", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(raw.Trim(), "1", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(raw.Trim(), "yes", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (string.Equals(raw.Trim(), "N", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(raw.Trim(), "0", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(raw.Trim(), "no", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return defaultValue;
        }
    }
}

using Bidding_API.Helpers;
using Bidding_Common;
using Bidding_Common.Helpers;
using Bidding_Core.Helpers;
using Bidding_Core.Services.Interfaces;
using Bidding_DLL.Services.Interfaces;
using Bidding_Models;
using Bidding_Models.DatabaseModel;
using Bidding_Models.DTOs;
using Bidding_Models.Jobseekers;
using ConstructionHub_Core.Helpers;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bidding_Core.Services.Implementations
{
    public class RegistrationUserService : IRegistrationUserService
    {
        /// <summary>
        /// Temporary fixed password for jobseeker self-registration (UI no longer collects a password).
        /// Replace with a random generated password when product is ready.
        /// </summary>
        private const string JobseekerRegistrationDefaultPassword = "Oomaniman#007";

        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly IAdminSystemService _adminSystemService;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly INafathJobSeekerService _nafathJobSeekerService;
        public EmailSettings EmailSettings { get; }

        public RegistrationUserService(IUserRepository userRepository, EmailSettings EmailSettings,
            IConfiguration configuration, IAuthService authService, IAdminSystemService adminSystemService,
            INotificationDispatchService notificationDispatchService,
            INafathJobSeekerService nafathJobSeekerService)
        {
            _userRepository = userRepository;
            this.EmailSettings = EmailSettings;
            _configuration = configuration;
            _authService = authService;
            _adminSystemService = adminSystemService;
            _notificationDispatchService = notificationDispatchService;
            _nafathJobSeekerService = nafathJobSeekerService;
        }
        public async Task<object> GetRiyadaCompanyData(string crNumber, CancellationToken cancellationToken)
        {
            if (crNumber == "1") return null;
            // add randem data to Riyada Model
            // TODO
            // HttpClient client = new HttpClient();
            // var response = await client.GetAsync("https://api.example.com/resource", cancellationToken);
            Riyada riyada = new Riyada
            {
                CRNumber = crNumber,
                IsRiyadaCard = "true",
                RegistrationDate = DateTime.Now.ToString(),
                CardExpiryDate = DateTime.Now.AddYears(3).ToString(),
                Result = "valied",
                SMEName = "Global Company",
                IsSMERegistered = "true",
                SMETypeName = "Middle",
            };


            return riyada;
        }
        public async Task<object> IsValidCivilIdWithMol(string civilId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(civilId) || civilId.Trim() == "1")
            {
                return null;
            }

            var trimmed = civilId.Trim();

            // Nafath JobSeekerDetails: live BaseUrl from appsettings; mock/check toggles from system variables.
            return await LookupCivilIdViaNafathAsync(trimmed, cancellationToken);
        }

        /// <summary>
        /// Nafath JobSeeker check for civil-id lookup during registration.
        /// </summary>
        private async Task<object> LookupCivilIdViaNafathAsync(string civilId, CancellationToken cancellationToken)
        {
            if (!await _nafathJobSeekerService.IsRegistrationCheckEnabledAsync())
            {
                return new
                {
                    Name_en = (string)null,
                    Name_ar = (string)null,
                    MobileNumber = (string)null,
                    PrimarySpecification_ar = (string)null,
                    ExpActiveDate = (string)null,
                    JobStatus = (string)null,
                    IsActiveJobSeeker = true,
                    MockFixture = (string)null,
                    RegistrationCheckSkipped = true,
                };
            }

            var fetch = await _nafathJobSeekerService.GetDetailsByCivilIdAsync(civilId, cancellationToken);
            ThrowIfNafathRegistrationRejected(fetch);

            var data = fetch.Data;
            var mobile = NafathJobSeekerService.NormalizeField(data?.GsmNo)
                         ?? NafathJobSeekerService.NormalizeField(data?.TelNo1);

            return new
            {
                Name_en = NafathJobSeekerService.FormatFullNameEn(data),
                Name_ar = NafathJobSeekerService.FormatFullNameAr(data),
                MobileNumber = mobile,
                PrimarySpecification_ar = NafathJobSeekerService.ExtractPrimarySpecificationAr(data),
                ExpActiveDate = NafathJobSeekerService.NormalizeField(data?.ExpActiveDate),
                JobStatus = NafathJobSeekerService.NormalizeField(data?.JobStatusDesc)
                            ?? NafathJobSeekerService.NormalizeField(data?.JobStatus),
                IsActiveJobSeeker = true,
                MockFixture = fetch.MockFixture,
            };
        }

        private async Task<NafathJobSeekerDetailsDto> EnsureActiveNafathJobSeekerAsync(
            string civilId,
            CancellationToken cancellationToken = default)
        {
            if (!await _nafathJobSeekerService.IsRegistrationCheckEnabledAsync())
            {
                return null;
            }

            var fetch = await _nafathJobSeekerService.GetDetailsByCivilIdAsync(civilId, cancellationToken);
            ThrowIfNafathRegistrationRejected(fetch);
            return fetch.Data;
        }

        private static void ThrowIfNafathRegistrationRejected(NafathJobSeekerFetchResult fetch)
        {
            if (!fetch.Success)
            {
                throw new SafeExecutionHelper.BadRequestException(
                    NafathJobSeekerErrorCodes.FetchFailed,
                    fetch.ErrorMessage ?? NafathJobSeekerErrorCodes.FetchFailedMessage);
            }

            if (!fetch.HasData)
            {
                throw new SafeExecutionHelper.NotFoundException(
                    NafathJobSeekerErrorCodes.NoData,
                    NafathJobSeekerErrorCodes.NoDataMessage);
            }

            if (!fetch.IsActiveJobSeeker)
            {
                throw new SafeExecutionHelper.NotAcceptableException(
                    NafathJobSeekerErrorCodes.Inactive,
                    fetch.InactiveDetail ?? NafathJobSeekerErrorCodes.InactiveMessage);
            }
        }

        private static void ApplyNafathDetailsToProfile(Profile profile, NafathJobSeekerDetailsDto data)
        {
            if (profile == null || data == null)
            {
                return;
            }

            var nameEn = NafathJobSeekerService.FormatFullNameEn(data);
            var nameAr = NafathJobSeekerService.FormatFullNameAr(data);
            if (string.IsNullOrWhiteSpace(profile.Name))
            {
                profile.Name = nameEn ?? nameAr ?? profile.Name;
            }

            profile.Mol_DateOfBirth = NafathJobSeekerService.NormalizeField(data.DateOfBirth) ?? profile.Mol_DateOfBirth;
            profile.Mol_MaritalStatus = NafathJobSeekerService.NormalizeField(data.MaritialDescArb)
                                        ?? NafathJobSeekerService.NormalizeField(data.MaritialStatus)
                                        ?? profile.Mol_MaritalStatus;
            profile.Mol_HealthStatus = NafathJobSeekerService.NormalizeField(data.HealthDescArb)
                                       ?? NafathJobSeekerService.NormalizeField(data.HealthStatus)
                                       ?? profile.Mol_HealthStatus;
            profile.Mol_Governorate = NafathJobSeekerService.NormalizeField(data.PerRegionDesc)
                                      ?? NafathJobSeekerService.NormalizeField(data.PerRegionCode)
                                      ?? profile.Mol_Governorate;
            profile.Mol_Willaya = NafathJobSeekerService.NormalizeField(data.PerWilayatDescArb)
                                  ?? NafathJobSeekerService.NormalizeField(data.PerWilayatCode)
                                  ?? profile.Mol_Willaya;
            var jobStatus = NafathJobSeekerService.NormalizeField(data.JobStatusDesc)
                            ?? NafathJobSeekerService.NormalizeField(data.JobStatus);
            profile.Mol_JobStatus = jobStatus ?? profile.Mol_JobStatus;
            profile.Mol_JobSearchStatus = jobStatus ?? profile.Mol_JobSearchStatus;
            profile.Mol_DateOfFirstRegistrationAsJobSeeker =
                NafathJobSeekerService.NormalizeField(data.RegistrationDate) ?? profile.Mol_DateOfFirstRegistrationAsJobSeeker;
            profile.Mol_DateOfLastRegistrationAsJobSeeker =
                NafathJobSeekerService.NormalizeField(data.ActiveDate) ?? profile.Mol_DateOfLastRegistrationAsJobSeeker;
            profile.Mol_CurrentEmploymentDate =
                NafathJobSeekerService.NormalizeField(data.CurrentEmpDate) ?? profile.Mol_CurrentEmploymentDate;
            if (!string.IsNullOrWhiteSpace(NafathJobSeekerService.NormalizeField(data.SexDescEng)))
            {
                profile.Gender = NafathJobSeekerService.NormalizeField(data.SexDescEng);
            }
        }

        private async Task<object> TryFetchMakeenCivilDataAsync(string civilId, CancellationToken cancellationToken)
        {
            var baseUrl = _configuration["Makeen:MAKEENBASEURL"]?.Trim();
            var userName = _configuration["Makeen:ServiceUserName"]?.Trim();
            var password = _configuration["Makeen:ServicePassword"]?.Trim();
            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            try
            {
                userName = userName.Decrypt();
                password = password.Decrypt();
            }
            catch
            {
                return null;
            }

            var endpoint = baseUrl.Contains("?", StringComparison.Ordinal)
                ? $"{baseUrl}&civilId={Uri.EscapeDataString(civilId)}"
                : $"{baseUrl}?civilId={Uri.EscapeDataString(civilId)}";

            using var client = new HttpClient();
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            using var response = await client.GetAsync(endpoint, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var mol = JsonConvert.DeserializeObject<CivilIdValidationResponse>(json);
            if (mol?.Status != 1 || mol.Data == null)
            {
                return null;
            }

            var data = mol.Data;
            var nameEn = JoinNameParts(
                data.NMR_FIRST_NAME_ENG,
                data.NMR_SECOND_NAME_ENG,
                data.NMR_THIRD_NAME_ENG,
                data.NMR_TRIBE_NAME_ENG);
            var nameAr = JoinNameParts(
                data.NMR_FIRST_NAME,
                data.NMR_SECOND_NAME,
                data.NMR_THIRD_NAME,
                data.NMR_TRIBE_NAME);

            if (string.IsNullOrWhiteSpace(nameEn) && string.IsNullOrWhiteSpace(nameAr))
            {
                return null;
            }

            return new
            {
                Name_en = nameEn,
                Name_ar = nameAr,
                MobileNumber = FormatMolMobileNumber(data.GSM_NO ?? data.TELNO1),
            };
        }

        private static object BuildDevelopmentMolLookup(string civilId)
        {
            var suffix = civilId.Length >= 4 ? civilId.Substring(civilId.Length - 4) : civilId;
            var mobileTail = civilId.Length >= 7
                ? civilId.Substring(civilId.Length - 7)
                : civilId.PadLeft(7, '0');

            return new
            {
                Name_en = $"Jobseeker {suffix}",
                Name_ar = $"باحث عمل {suffix}",
                MobileNumber = $"9{mobileTail}",
            };
        }

        private static string JoinNameParts(params object[] parts)
        {
            return string.Join(
                " ",
                parts
                    .Select(p => p?.ToString()?.Trim())
                    .Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        private static string FormatMolMobileNumber(object raw)
        {
            if (raw == null)
            {
                return null;
            }

            var digits = new string(raw.ToString().Where(char.IsDigit).ToArray());
            return string.IsNullOrWhiteSpace(digits) ? null : digits;
        }
        public async Task<string> GetCivilIdByCrNumber(string crNumber)
        {
            var _mociHelper = new MociHelper(_configuration);
            var mociData = await _mociHelper.GetDataByCrAsync(crNumber);
            if (mociData == null)
                throw new SafeExecutionHelper.NotFoundException(
                    $"CR number '{crNumber}' not found in MOCIIP database.");

            string civilId = mociData.persons != null && mociData.persons.Any() ? mociData.persons[0].idNumber : null;
            if (string.IsNullOrWhiteSpace(civilId))
                throw new SafeExecutionHelper.NotFoundException(
                    $"No valid Civil ID found for CR number '{crNumber}' in MOCIIP Database.");

            return civilId;
        }
        public async Task<string> GetCompanyNameByCrNumber(string crNumber)
        {
            var _mociHelper = new MociHelper(_configuration);
            var mociData = await _mociHelper.GetDataByCrAsync(crNumber);
            if (mociData == null)
                throw new SafeExecutionHelper.NotFoundException(
                    $"CR number '{crNumber}' not found in MOCIIP database.");

            string companyName = !string.IsNullOrWhiteSpace(mociData.crNo.companyNameEnglish) ? mociData.crNo.companyNameEnglish : mociData.crNo.companyNameArabic;
            if (string.IsNullOrWhiteSpace(companyName))
                throw new SafeExecutionHelper.NotFoundException(
                    $"No valid Company Name found for CR number '{crNumber}' in MOCIIP Database.");

            return companyName;
        }
        public async Task<object> RegisterSmeAsync(RegisterSmeRequestDto registerSmeRequest)
        {
            if (OtpEncryptionHelper.CompareOtp(registerSmeRequest.OTP, registerSmeRequest.OtpConfirmed) == false)
            {
                throw new SafeExecutionHelper.NotAcceptableException("OTP is not valid.");
            }
            var _mociHelper = new MociHelper(_configuration);
            var companyInfoFromMoci = await _mociHelper.GetDataByCrAsync(registerSmeRequest.CrNumber);
            if (companyInfoFromMoci == null)
                throw new SafeExecutionHelper.NotFoundException(
                    $"CR number '{registerSmeRequest.CrNumber}' not found in MOCIIP database.");

            var signatoriesCivilIds = companyInfoFromMoci?.signatories?
                .Where(s => s?.person?.idNumber != null)
                .Select(s => s.person.idNumber)
                .ToList()
                ?? new List<string>();
            if (!signatoriesCivilIds.Any())
                throw new SafeExecutionHelper.NotFoundException(
                    $"CR number '{registerSmeRequest.CrNumber}' has no signatory in MOCIIP database.");

            //verify Token
            if (string.IsNullOrWhiteSpace(registerSmeRequest.TempToken))
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"Invalid Token");

            IsValidTempTokenForCompany(registerSmeRequest.TempToken, signatoriesCivilIds, registerSmeRequest.PhoneNumber);


            if (registerSmeRequest.CrNumber == "1")
            {
                throw new SafeExecutionHelper.NotFoundException(
                    $"Sme with ID '{registerSmeRequest.CrNumber}' not found.");
            }

            // add randem data to Riyada Model
            // TODO check with Riyada API
            // HttpClient client = new HttpClient();
            // var response = await client.GetAsync("https://api.example.com/resource", cancellationToken);
            Riyada riyada = new Riyada
            {
                CRNumber = registerSmeRequest.CrNumber,
                IsRiyadaCard = "true",
                RegistrationDate = DateTime.Now.ToString(),
                CardExpiryDate = DateTime.Now.AddYears(3).ToString(),
                Result = "valied",
                SMEName = "Global Company",
                IsSMERegistered = "true",
                SMETypeName = "Middle",
            };
            var foundUserByEmail = await _userRepository.GetUserByEmailAsync(registerSmeRequest.Email);
            if (foundUserByEmail != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with email '{registerSmeRequest.Email}' already exists.");
            var foundUserByPhone = await _userRepository.GetUserProfileByPhoneNumberAsync(registerSmeRequest.PhoneNumber);
            if (foundUserByPhone != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with phone number '{registerSmeRequest.PhoneNumber}' already exists.");
            var foundUserByCrNumber = await _userRepository.GetUserProfileByCrNumberAsync(registerSmeRequest.CrNumber);
            if (foundUserByCrNumber != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with CR number '{registerSmeRequest.CrNumber}' already exists.");

            var setting = CreateDefaultProfileSetting();
            var profile = new Profile
            {
                CrNumber = registerSmeRequest.CrNumber,
                CompanyNameEnglish = registerSmeRequest.CompanyName,
                Settings = setting,
                EmailAddress = registerSmeRequest.Email,
                ComanyEmail = registerSmeRequest.Email,
                CompanyNameArabic = registerSmeRequest.CompanyName,
                PhoneNumber = registerSmeRequest.PhoneNumber,
                ComanyPhone = registerSmeRequest.PhoneNumber,

                IsActive = true,
                JoinDate = DateTime.Now,
                IsOmani = true,
                ProfileType = (int)Enums.ProfileTypes.SME,
                IsPostOpertunity = false,
                MissionVision = registerSmeRequest.MissionVision,
                Name = registerSmeRequest.CompanyName
            };

            profile.CompanyInfo = new CompanyInfo()
            {
                CrNumber = companyInfoFromMoci.crNo.crNumber,
                CompanyNameArabic = companyInfoFromMoci.crNo.companyNameArabic,
                CompanyNameEnglish = companyInfoFromMoci.crNo.companyNameEnglish,
                CompanyNameWithLegalArabic = companyInfoFromMoci.crNo.companyNameWithLegalArabic,
                CompanyNameWithLegalEnglish = companyInfoFromMoci.crNo.companyNameWithLegalEnglish,
                CompanyStatus = new RegistrationStatus()
                {
                    Code = companyInfoFromMoci.crNo.registrationStatus.code,
                    NameAr = companyInfoFromMoci.crNo.registrationStatus.nameAr,
                    NameEn = companyInfoFromMoci.crNo.registrationStatus.nameEn,
                },
                LegalStatus = new LegalStatus()
                {
                    Code = companyInfoFromMoci.crNo.legalStatus.code,
                    NameAr = companyInfoFromMoci.crNo.legalStatus.nameAr,
                    NameEn = companyInfoFromMoci.crNo.legalStatus.nameEn,
                },
                Capital = new Capital()
                {
                    CashCapital = companyInfoFromMoci.capitalData.cashCapital.ToString(),
                    AssetCapital = companyInfoFromMoci.capitalData.assetCapital.ToString(),
                    TotalCapital = companyInfoFromMoci.capitalData.totalCapital.ToString(),
                    ShareValue = companyInfoFromMoci.capitalData.shareValue.ToString(),
                    TotalShares = companyInfoFromMoci.capitalData.totalShares.ToString(),
                    CapitalVerified = companyInfoFromMoci.capitalData.capitalVerified,
                },
                RegistrationDate = Convert.ToDateTime(companyInfoFromMoci.crNo.registrationDate),
                ExpiryDate = Convert.ToDateTime(companyInfoFromMoci.crNo.expiryDate),
                CompanyGrade = "Unknown",
                Location = new Location()
                {
                    Governorate = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].governorate.nameEn : "",
                    Town = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].town.nameEn : "",
                    State = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].state.nameEn : "",
                },
                SubjectToForeignInvestment = companyInfoFromMoci.crNo.subjectToForeignInvestment,
                DeclaredActivities = companyInfoFromMoci.activities != null ? companyInfoFromMoci.activities.ConvertAll(da => new DeclaredActivity
                {
                    IsicVersion = da.isicVersion,
                    IsicCode = da.isicCode,
                    NameAr = da.nameAr,
                    NameEn = da.nameEn,
                    RegistrationDate = da.registrationDate,
                }) : null,
                Signatories = companyInfoFromMoci.signatories != null ? companyInfoFromMoci.signatories.ConvertAll(sg => new CompanyInfoSignatory
                {
                    PersonNameAr = sg.person.nameAr,
                    PersonNameEn = sg.person.nameEn,
                    CivilId = sg.person.idNumber,
                    Designation = new Designation()
                    {
                        code = sg.designation.code,
                        nameAr = sg.designation.nameAr,
                        nameEn = sg.designation.nameEn,
                    },
                    SignatureType = new Signaturetype()
                    {
                        code = sg.signatureType.code,
                        nameAr = sg.signatureType.nameAr,
                        nameEn = sg.signatureType.nameEn,
                    },
                    SignatoryStartDate = Convert.ToDateTime(sg.signatoryStartDate),
                }) : null


            };
            var newGuid = Guid.NewGuid();
            var appUser = new AppUser
            {
                Email = registerSmeRequest.Email,
                UserName = registerSmeRequest.Email.ToLower(),
                EmailConfirmed = false,
                Password = "A@123456bcd",
                PhoneNumber = registerSmeRequest.PhoneNumber,
                PhoneNumberConfirmed = true,
                OTP = registerSmeRequest.OTP,
                Profile = profile,
                CreatedDate = DateTime.Now,
                RoleName = "vendor",
                IsVendor = true,
                VerificationToken = newGuid.ToString()
            };
            object createdUser;
            try
            {
                createdUser = await _userRepository.CreateUserAsync(appUser);
                await SendVerificationEmailAsync(appUser);
            }
            catch (Exception e)
            {
                throw new SafeExecutionHelper.InternalServerErrorException(e.Message);
            }

            return createdUser;
        }
        public async Task<object> RegisterSmeWithoutOtpAsync(RegisterSmeWithoutOtpRequestDto registerSmeRequest)
        {
            var _mociHelper = new MociHelper(_configuration);
            var companyInfoFromMoci = await _mociHelper.GetDataByCrAsync(registerSmeRequest.CrNumber);
            if (companyInfoFromMoci == null)
                throw new SafeExecutionHelper.NotFoundException(
                    $"CR number '{registerSmeRequest.CrNumber}' not found in MOCIIP database.");

            var signatoriesCivilIds = companyInfoFromMoci?.signatories?
                .Where(s => s?.person?.idNumber != null)
                .Select(s => s.person.idNumber)
                .ToList()
                ?? new List<string>();
            if (!signatoriesCivilIds.Any())
                throw new SafeExecutionHelper.NotFoundException(
                    $"CR number '{registerSmeRequest.CrNumber}' has no signatory in MOCIIP database.");

            //verify Token
            if (string.IsNullOrWhiteSpace(registerSmeRequest.TempToken))
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"Invalid Token");

            IsValidTempTokenForCompany(registerSmeRequest.TempToken, signatoriesCivilIds, registerSmeRequest.PhoneNumber);

            if (registerSmeRequest.CrNumber == "1")
            {
                throw new SafeExecutionHelper.NotFoundException(
                    $"Sme with ID '{registerSmeRequest.CrNumber}' not found.");
            }

            //TODO: Call Riyada
            // add randem data to Riyada Model
            // TODO check with Riyada API
         
            Riyada riyada = new Riyada
            {
                CRNumber = registerSmeRequest.CrNumber,
                IsRiyadaCard = "true",
                RegistrationDate = DateTime.Now.ToString(),
                CardExpiryDate = DateTime.Now.AddYears(3).ToString(),
                Result = "valied",
                SMEName = "Global Company",
                IsSMERegistered = "true",
                SMETypeName = "Middle",
            };
            var foundUserByEmail = await _userRepository.GetUserByEmailAsync(registerSmeRequest.Email);
            if (foundUserByEmail != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with email '{registerSmeRequest.Email}' already exists.");

            var foundUserByPhone = await _userRepository.GetUserProfileByPhoneNumberAsync(registerSmeRequest.PhoneNumber);
            if (foundUserByPhone != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with phone number '{registerSmeRequest.PhoneNumber}' already exists.");

            var foundUserByCrNumber = await _userRepository.GetUserProfileByCrNumberAsync(registerSmeRequest.CrNumber);
            if (foundUserByCrNumber != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with CR number '{registerSmeRequest.CrNumber}' already exists.");

            var setting = CreateDefaultProfileSetting();
            var profile = new Profile
            {
                CrNumber = registerSmeRequest.CrNumber,
                CompanyNameEnglish = registerSmeRequest.CompanyName,
                Settings = setting,
                EmailAddress = registerSmeRequest.Email,
                ComanyEmail = registerSmeRequest.Email,
                CompanyNameArabic = registerSmeRequest.CompanyName,
                PhoneNumber = registerSmeRequest.PhoneNumber,
                ComanyPhone = registerSmeRequest.PhoneNumber,
                IsActive = true,
                JoinDate = DateTime.Now,
                IsOmani = true,
                ProfileType = (int)Enums.ProfileTypes.SME,
                IsPostOpertunity = false,
                MissionVision = registerSmeRequest.MissionVision,
                Name = registerSmeRequest.CompanyName
            };

           
            profile.CompanyInfo = new CompanyInfo()
            {
                CrNumber = companyInfoFromMoci.crNo.crNumber,
                CompanyNameArabic = companyInfoFromMoci.crNo.companyNameArabic,
                CompanyNameEnglish = companyInfoFromMoci.crNo.companyNameEnglish,
                CompanyNameWithLegalArabic = companyInfoFromMoci.crNo.companyNameWithLegalArabic,
                CompanyNameWithLegalEnglish = companyInfoFromMoci.crNo.companyNameWithLegalEnglish,
                CompanyStatus = new RegistrationStatus()
                {
                    Code = companyInfoFromMoci.crNo.registrationStatus.code,
                    NameAr = companyInfoFromMoci.crNo.registrationStatus.nameAr,
                    NameEn = companyInfoFromMoci.crNo.registrationStatus.nameEn,
                },
                LegalStatus = new LegalStatus()
                {
                    Code = companyInfoFromMoci.crNo.legalStatus.code,
                    NameAr = companyInfoFromMoci.crNo.legalStatus.nameAr,
                    NameEn = companyInfoFromMoci.crNo.legalStatus.nameEn,
                },
                Capital = new Capital()
                {
                    CashCapital = companyInfoFromMoci.capitalData.cashCapital.ToString(),
                    AssetCapital = companyInfoFromMoci.capitalData.assetCapital.ToString(),
                    TotalCapital = companyInfoFromMoci.capitalData.totalCapital.ToString(),
                    ShareValue = companyInfoFromMoci.capitalData.shareValue.ToString(),
                    TotalShares = companyInfoFromMoci.capitalData.totalShares.ToString(),
                    CapitalVerified = companyInfoFromMoci.capitalData.capitalVerified,
                },
                RegistrationDate = Convert.ToDateTime(companyInfoFromMoci.crNo.registrationDate),
                ExpiryDate = Convert.ToDateTime(companyInfoFromMoci.crNo.expiryDate),
                CompanyGrade = "Unknown",
                Location = new Location()
                {
                    Governorate = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].governorate.nameEn : "",
                    Town = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].town.nameEn : "",
                    State = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].state.nameEn : "",
                },
                SubjectToForeignInvestment = companyInfoFromMoci.crNo.subjectToForeignInvestment,
                DeclaredActivities = companyInfoFromMoci.activities != null ? companyInfoFromMoci.activities.ConvertAll(da => new DeclaredActivity
                {
                    IsicVersion = da.isicVersion,
                    IsicCode = da.isicCode,
                    NameAr = da.nameAr,
                    NameEn = da.nameEn,
                    RegistrationDate = da.registrationDate,
                }) : null,
                Signatories = companyInfoFromMoci.signatories != null ? companyInfoFromMoci.signatories.ConvertAll(sg => new CompanyInfoSignatory
                {
                    PersonNameAr = sg.person.nameAr,
                    PersonNameEn = sg.person.nameEn,
                    CivilId = sg.person.idNumber,
                    Designation = new Designation()
                    {
                        code = sg.designation.code,
                        nameAr = sg.designation.nameAr,
                        nameEn = sg.designation.nameEn,
                    },
                    SignatureType = new Signaturetype()
                    {
                        code = sg.signatureType.code,
                        nameAr = sg.signatureType.nameAr,
                        nameEn = sg.signatureType.nameEn,
                    },
                    SignatoryStartDate = Convert.ToDateTime(sg.signatoryStartDate),
                }) : null


            };
            var newGuid = Guid.NewGuid();
            var appUser = new AppUser
            {
                Email = registerSmeRequest.Email,
                UserName = registerSmeRequest.Email.ToLower(),
                EmailConfirmed = false,
                Password = "A@123456b",
                PhoneNumber = registerSmeRequest.PhoneNumber,
                PhoneNumberConfirmed = true,
                OTP = string.Empty,
                Profile = profile,
                CreatedDate = DateTime.Now,
                RoleName = "vendor",
                IsVendor = true,
                VerificationToken = newGuid.ToString()
            };
            object createdUser;
            try
            {
                createdUser = await _userRepository.CreateUserAsync(appUser);
                await SendVerificationEmailAsync(appUser);
            }
            catch (Exception e)
            {
                throw new SafeExecutionHelper.InternalServerErrorException(e.Message);
            }
            return createdUser;
        }
        public async Task<object> RegisterFreelancerAsync(RegisterFreelancerRequestDto freelancerRequestDto)
        {
            if (OtpEncryptionHelper.CompareOtp(freelancerRequestDto.OTP, freelancerRequestDto.OtpConfirmed) == false)
            {
                throw new SafeExecutionHelper.NotAcceptableException("OTP is not valid.");
            }
            var _mociHelper = new MociHelper(_configuration);
            var companyInfoFromMoci = await _mociHelper.GetDataByCrAsync(freelancerRequestDto.CrNumber);
            if (companyInfoFromMoci == null)
                throw new SafeExecutionHelper.NotFoundException(
                    $"CR number '{freelancerRequestDto.CrNumber}' not found in MOCIIP database.");
            
            var signatoriesCivilIds = companyInfoFromMoci?.signatories?
                .Where(s => s?.person?.idNumber != null)
                .Select(s => s.person.idNumber)
                .ToList()
                ?? new List<string>();
            if (!signatoriesCivilIds.Any())
                throw new SafeExecutionHelper.NotFoundException(
                    $"CR number '{freelancerRequestDto.CrNumber}' has no signatory in MOCIIP database.");

            //verify Token
            if (string.IsNullOrWhiteSpace(freelancerRequestDto.TempToken))
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"Invalid Token");

            IsValidTempTokenForCompany(freelancerRequestDto.TempToken, signatoriesCivilIds, freelancerRequestDto.PhoneNumber);

            var foundUserByEmail = await _userRepository.GetUserByEmailAsync(freelancerRequestDto.Email);
            if (foundUserByEmail != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with email '{freelancerRequestDto.Email}' already exists.");
            var foundUserByPhone = await _userRepository.GetUserProfileByPhoneNumberAsync(freelancerRequestDto.PhoneNumber);
            if (foundUserByPhone != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with phone number '{freelancerRequestDto.PhoneNumber}' already exists.");
            var foundUserByCrNumber = await _userRepository.GetUserProfileByCrNumberAsync(freelancerRequestDto.CivilId);
           
            if (foundUserByCrNumber != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with CR number '{freelancerRequestDto.CivilId}' already exists.");

            var dataFromMol = new FreelancerDataFromMol()
            {
                REGISTRATION_NO = 1181383,
                REGISTRATION_DATE = "2020-11-03T07:48:12",
                NATIONAL_ID = "12083989",
                NMR_FIRST_NAME = "رحيل",
                NMR_SECOND_NAME = "صالح",
                NMR_THIRD_NAME = "حسن",
                NMR_TRIBE_NAME = "الأنصارية",
                NMR_FIRST_NAME_ENG = "Rahil",
                NMR_SECOND_NAME_ENG = "Saleh",
                NMR_THIRD_NAME_ENG = "HASSAN",
                NMR_TRIBE_NAME_ENG = "Al Ansari",
                DATE_OF_BIRTH = "1997-04-17T00:00:00",
                PLACE_OF_BIRTH = "صحار",
                ISSUE_DATE = "2020-09-14T20:00:00",
                EXPIRE_DATE = "2025-09-14T20:00:00",
                SEX_Desc_Eng = "Female",
                MARITIAL_STATUS = 2,
                MARITIAL_DESC_ARB = "اعزب \\ عزباء",
                PER_REGION_CODE = 2,
                PER_REGION_desc = "محافظة شمال الباطنة ",
                PER_WILAYAT_CODE = 1,
                PER_WILAYAT_DESC_ARB = "صــحـــار",
                PER_VILLAGE_CODE = 97,
                PER_VILLAGE_DESC_ARB = "مجيس",
                RES_REGION_CODE = 2,
                RES_REGION_desc = "محافظة شمال الباطنة ",
                RES_WILAYAT_CODE = 1,
                RES_WILAYAT_DESC_ARB = "صــحـــار",
                RES_VILLAGE_CODE = 870,
                RES_VILLAGE_DESC_ARB = "حارة الشيخ",
                NATIONALITY_CODE = "OMN",
                NATIONALITY_DESC_ARB = "سلطنة عمان",
                TELNO1 = 71223766,
                TELNO2 = 0,
                GSM_NO = 92413316,
                EMAIL_ADDRESS = "KARAMELA2010@LIVE.COM",
                DESIGNATION_CODE = "",
                DESIGNATION_DESC = "",
                EDUCATION_CODE = "02010202",
                EDUCATION_DESC_ARB = "دبلوم تعليم العام",
                OCCUPATION_CODE = "1133031",
                OCCUPATION_DESC_ARB = "مبرمج حاسب الي",
                CURRENT_EMP_DATE = "2022-02-02T00:00:00",
                BASIC_SALARY = "0.0",
                TOTAL_ALLOWANCE = "122",
                SPONSOR_CR_NO = "1624822",
                SPONSOR_NAME = "الشركة الدولية لخدمات الحاسب الالي ش م م",
                JOB_STATUS_Desc = "يعمل",
                CATEGORY_DESC_ARB = "القطاع الخاص",
                CATEGORY_CODE = 2,
                message = "Data Retrieved Successfully"
            }; //TODO: CALL MOL HERE

            var setting = CreateDefaultProfileSetting();
            var profile = new Profile
            {
                CrNumber = freelancerRequestDto.CivilId,
                
                CompanyNameEnglish = freelancerRequestDto.FullName,
                Name = freelancerRequestDto.FullName,
                Settings = setting,
                EmailAddress = freelancerRequestDto.Email,
                PhoneNumber = freelancerRequestDto.PhoneNumber,
                IsActive = true,
                JoinDate = DateTime.Now,
                IsOmani = true,
                ProfileType = (int)Enums.ProfileTypes.FreeLancer,
                IsPostOpertunity = false,
                MissionVision = "Mission and Vision",
                Mol_Allowance = dataFromMol.TOTAL_ALLOWANCE ,
                Mol_CurrentEmploymentDate = dataFromMol.CURRENT_EMP_DATE,
                Mol_DateOfBirth = dataFromMol.DATE_OF_BIRTH,    
                Mol_DateOfFirstRegistrationAsJobSeeker = dataFromMol.REGISTRATION_DATE, 
                Mol_DateOfLastRegistrationAsJobSeeker = dataFromMol.CURRENT_EMP_DATE,
                Mol_Governorate = dataFromMol.PER_REGION_desc,
                Mol_HealthStatus = "Healthy",
                Mol_JobStatus = dataFromMol.JOB_STATUS_Desc,    
                Mol_JobSearchDuration = "0",
                Mol_JobSearchStatus = "Searching",
                Mol_MaritalStatus = dataFromMol.MARITIAL_DESC_ARB,
                Mol_Nationality = dataFromMol.NATIONALITY_DESC_ARB,
                Mol_Willaya = dataFromMol.PER_WILAYAT_DESC_ARB,
            };
            

            profile.CompanyInfo = new CompanyInfo()
            {
                CrNumber = companyInfoFromMoci.crNo.crNumber,
                CompanyNameArabic = companyInfoFromMoci.crNo.companyNameArabic,
                CompanyNameEnglish = companyInfoFromMoci.crNo.companyNameEnglish,
                CompanyNameWithLegalArabic = companyInfoFromMoci.crNo.companyNameWithLegalArabic,
                CompanyNameWithLegalEnglish = companyInfoFromMoci.crNo.companyNameWithLegalEnglish,
                CompanyStatus = new RegistrationStatus()
                {
                    Code = companyInfoFromMoci.crNo.registrationStatus.code,
                    NameAr = companyInfoFromMoci.crNo.registrationStatus.nameAr,
                    NameEn = companyInfoFromMoci.crNo.registrationStatus.nameEn,
                },
                LegalStatus = new LegalStatus()
                {
                    Code = companyInfoFromMoci.crNo.legalStatus.code,
                    NameAr = companyInfoFromMoci.crNo.legalStatus.nameAr,
                    NameEn = companyInfoFromMoci.crNo.legalStatus.nameEn,
                },
                Capital = new Capital()
                {
                    CashCapital = companyInfoFromMoci.capitalData.cashCapital.ToString(),
                    AssetCapital = companyInfoFromMoci.capitalData.assetCapital.ToString(),
                    TotalCapital = companyInfoFromMoci.capitalData.totalCapital.ToString(),
                    ShareValue = companyInfoFromMoci.capitalData.shareValue.ToString(),
                    TotalShares = companyInfoFromMoci.capitalData.totalShares.ToString(),
                    CapitalVerified = companyInfoFromMoci.capitalData.capitalVerified,
                },
                RegistrationDate = Convert.ToDateTime(companyInfoFromMoci.crNo.registrationDate),
                ExpiryDate = Convert.ToDateTime(companyInfoFromMoci.crNo.expiryDate),
                CompanyGrade = "Unknown",
                Location = new Location()
                {
                    Governorate = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].governorate.nameEn : "",
                    Town = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].town.nameEn : "",
                    State = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].state.nameEn : "",
                },
                SubjectToForeignInvestment = companyInfoFromMoci.crNo.subjectToForeignInvestment,
                DeclaredActivities = companyInfoFromMoci.activities != null ? companyInfoFromMoci.activities.ConvertAll(da => new DeclaredActivity
                {
                    IsicVersion = da.isicVersion,
                    IsicCode = da.isicCode,
                    NameAr = da.nameAr,
                    NameEn = da.nameEn,
                    RegistrationDate = da.registrationDate,
                }) : null,
                Signatories = companyInfoFromMoci.signatories != null ? companyInfoFromMoci.signatories.ConvertAll(sg => new CompanyInfoSignatory
                {
                    PersonNameAr = sg.person.nameAr,
                    PersonNameEn = sg.person.nameEn,
                    CivilId = sg.person.idNumber,
                    Designation = new Designation()
                    {
                        code = sg.designation.code,
                        nameAr = sg.designation.nameAr,
                        nameEn = sg.designation.nameEn,
                    },
                    SignatureType = new Signaturetype()
                    {
                        code = sg.signatureType.code,
                        nameAr = sg.signatureType.nameAr,
                        nameEn = sg.signatureType.nameEn,
                    },
                    SignatoryStartDate = Convert.ToDateTime(sg.signatoryStartDate),
                }) : null


            };

            var newGuid = Guid.NewGuid();
            var appUser = new AppUser
            {
                Email = freelancerRequestDto.Email,
                UserName = freelancerRequestDto.Email.ToLower(),
                EmailConfirmed = false,
                Password = "A@123456bcd",
                PhoneNumber = freelancerRequestDto.PhoneNumber,
                PhoneNumberConfirmed = true,
                OTP = freelancerRequestDto.OTP,
                Profile = profile,
                CreatedDate = DateTime.Now,
                VerificationToken = newGuid.ToString(),
                RoleName = "vendor",
                IsVendor = true,
                
            };

            object createdUser;
            try
            {
                createdUser = await _userRepository.CreateUserAsync(appUser);
                await _userRepository.ApplyRegistrationIdentityAccessAsync(
                    freelancerRequestDto.Email,
                    (int)Enums.ProfileTypes.FreeLancer);
                await SendVerificationEmailAsync(appUser);
            }
            catch (Exception e)
            {
                throw new SafeExecutionHelper.InternalServerErrorException(e.Message);
            }

            return createdUser;
        }
        public async Task<object> RegisterFreelancerWithoutOtpAsync(RegisterFreelancerWithoutOtpRequestDto freelancerRequestDto)
        {
            //verify Token
            if (string.IsNullOrWhiteSpace(freelancerRequestDto.TempToken) || !IsValidTempTokenForFreelancer(freelancerRequestDto.TempToken,freelancerRequestDto.CivilId,freelancerRequestDto.PhoneNumber))
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"Invalid Token");

            var foundUserByEmail = await _userRepository.GetUserByEmailAsync(freelancerRequestDto.Email);
            if (foundUserByEmail != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with email '{freelancerRequestDto.Email}' already exists.");
            var foundUserByPhone = await _userRepository.GetUserProfileByPhoneNumberAsync(freelancerRequestDto.PhoneNumber);
            if (foundUserByPhone != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with phone number '{freelancerRequestDto.PhoneNumber}' already exists.");
            var foundUserByCrNumber = await _userRepository.GetUserProfileByCrNumberAsync(freelancerRequestDto.CivilId);

            if (foundUserByCrNumber != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with CR number '{freelancerRequestDto.CivilId}' already exists.");

            var dataFromMol = new FreelancerDataFromMol()
            {
                REGISTRATION_NO = 1181383,
                REGISTRATION_DATE = "2020-11-03T07:48:12",
                NATIONAL_ID = "12083989",
                NMR_FIRST_NAME = "رحيل",
                NMR_SECOND_NAME = "صالح",
                NMR_THIRD_NAME = "حسن",
                NMR_TRIBE_NAME = "الأنصارية",
                NMR_FIRST_NAME_ENG = "Rahil",
                NMR_SECOND_NAME_ENG = "Saleh",
                NMR_THIRD_NAME_ENG = "HASSAN",
                NMR_TRIBE_NAME_ENG = "Al Ansari",
                DATE_OF_BIRTH = "1997-04-17T00:00:00",
                PLACE_OF_BIRTH = "صحار",
                ISSUE_DATE = "2020-09-14T20:00:00",
                EXPIRE_DATE = "2025-09-14T20:00:00",
                SEX_Desc_Eng = "Female",
                MARITIAL_STATUS = 2,
                MARITIAL_DESC_ARB = "اعزب \\ عزباء",
                PER_REGION_CODE = 2,
                PER_REGION_desc = "محافظة شمال الباطنة ",
                PER_WILAYAT_CODE = 1,
                PER_WILAYAT_DESC_ARB = "صــحـــار",
                PER_VILLAGE_CODE = 97,
                PER_VILLAGE_DESC_ARB = "مجيس",
                RES_REGION_CODE = 2,
                RES_REGION_desc = "محافظة شمال الباطنة ",
                RES_WILAYAT_CODE = 1,
                RES_WILAYAT_DESC_ARB = "صــحـــار",
                RES_VILLAGE_CODE = 870,
                RES_VILLAGE_DESC_ARB = "حارة الشيخ",
                NATIONALITY_CODE = "OMN",
                NATIONALITY_DESC_ARB = "سلطنة عمان",
                TELNO1 = 71223766,
                TELNO2 = 0,
                GSM_NO = 92413316,
                EMAIL_ADDRESS = "KARAMELA2010@LIVE.COM",
                DESIGNATION_CODE = "",
                DESIGNATION_DESC = "",
                EDUCATION_CODE = "02010202",
                EDUCATION_DESC_ARB = "دبلوم تعليم العام",
                OCCUPATION_CODE = "1133031",
                OCCUPATION_DESC_ARB = "مبرمج حاسب الي",
                CURRENT_EMP_DATE = "2022-02-02T00:00:00",
                BASIC_SALARY = "0.0",
                TOTAL_ALLOWANCE = "122",
                SPONSOR_CR_NO = "1624822",
                SPONSOR_NAME = "الشركة الدولية لخدمات الحاسب الالي ش م م",
                JOB_STATUS_Desc = "يعمل",
                CATEGORY_DESC_ARB = "القطاع الخاص",
                CATEGORY_CODE = 2,
                message = "Data Retrieved Successfully"
            }; //TODO: CALL MOL HERE

            var setting = CreateDefaultProfileSetting();
            var profile = new Profile
            {
                CrNumber = freelancerRequestDto.CivilId,

                CompanyNameEnglish = freelancerRequestDto.FullName,
                Name = freelancerRequestDto.FullName,
                Settings = setting,
                EmailAddress = freelancerRequestDto.Email,
                PhoneNumber = freelancerRequestDto.PhoneNumber,
                IsActive = true,
                JoinDate = DateTime.Now,
                IsOmani = true,
                ProfileType = (int)Enums.ProfileTypes.FreeLancer,
                IsPostOpertunity = false,
                MissionVision = "Mission and Vision",
                Mol_Allowance = dataFromMol.TOTAL_ALLOWANCE,
                Mol_CurrentEmploymentDate = dataFromMol.CURRENT_EMP_DATE,
                Mol_DateOfBirth = dataFromMol.DATE_OF_BIRTH,
                Mol_DateOfFirstRegistrationAsJobSeeker = dataFromMol.REGISTRATION_DATE,
                Mol_DateOfLastRegistrationAsJobSeeker = dataFromMol.CURRENT_EMP_DATE,
                Mol_Governorate = dataFromMol.PER_REGION_desc,
                Mol_HealthStatus = "Healthy",
                Mol_JobStatus = dataFromMol.JOB_STATUS_Desc,
                Mol_JobSearchDuration = "0",
                Mol_JobSearchStatus = "Searching",
                Mol_MaritalStatus = dataFromMol.MARITIAL_DESC_ARB,
                Mol_Nationality = dataFromMol.NATIONALITY_DESC_ARB,
                Mol_Willaya = dataFromMol.PER_WILAYAT_DESC_ARB,
            };
            var _mociHelper = new MociHelper(_configuration);
            var companyInfoFromMoci = await _mociHelper.GetDataByCrAsync(freelancerRequestDto.CrNumber);
            if (companyInfoFromMoci == null)
                throw new SafeExecutionHelper.NotFoundException(
                    $"CR number '{freelancerRequestDto.CrNumber}' not found in MOCIIP database.");

            profile.CompanyInfo = new CompanyInfo()
            {
                CrNumber = companyInfoFromMoci.crNo.crNumber,
                CompanyNameArabic = companyInfoFromMoci.crNo.companyNameArabic,
                CompanyNameEnglish = companyInfoFromMoci.crNo.companyNameEnglish,
                CompanyNameWithLegalArabic = companyInfoFromMoci.crNo.companyNameWithLegalArabic,
                CompanyNameWithLegalEnglish = companyInfoFromMoci.crNo.companyNameWithLegalEnglish,
                CompanyStatus = new RegistrationStatus()
                {
                    Code = companyInfoFromMoci.crNo.registrationStatus.code,
                    NameAr = companyInfoFromMoci.crNo.registrationStatus.nameAr,
                    NameEn = companyInfoFromMoci.crNo.registrationStatus.nameEn,
                },
                LegalStatus = new LegalStatus()
                {
                    Code = companyInfoFromMoci.crNo.legalStatus.code,
                    NameAr = companyInfoFromMoci.crNo.legalStatus.nameAr,
                    NameEn = companyInfoFromMoci.crNo.legalStatus.nameEn,
                },
                Capital = new Capital()
                {
                    CashCapital = companyInfoFromMoci.capitalData.cashCapital.ToString(),
                    AssetCapital = companyInfoFromMoci.capitalData.assetCapital.ToString(),
                    TotalCapital = companyInfoFromMoci.capitalData.totalCapital.ToString(),
                    ShareValue = companyInfoFromMoci.capitalData.shareValue.ToString(),
                    TotalShares = companyInfoFromMoci.capitalData.totalShares.ToString(),
                    CapitalVerified = companyInfoFromMoci.capitalData.capitalVerified,
                },
                RegistrationDate = Convert.ToDateTime(companyInfoFromMoci.crNo.registrationDate),
                ExpiryDate = Convert.ToDateTime(companyInfoFromMoci.crNo.expiryDate),
                CompanyGrade = "Unknown",
                Location = new Location()
                {
                    Governorate = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].governorate.nameEn : "",
                    Town = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].town.nameEn : "",
                    State = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].state.nameEn : "",
                },
                SubjectToForeignInvestment = companyInfoFromMoci.crNo.subjectToForeignInvestment,
                DeclaredActivities = companyInfoFromMoci.activities != null ? companyInfoFromMoci.activities.ConvertAll(da => new DeclaredActivity
                {
                    IsicVersion = da.isicVersion,
                    IsicCode = da.isicCode,
                    NameAr = da.nameAr,
                    NameEn = da.nameEn,
                    RegistrationDate = da.registrationDate,
                }) : null,
                Signatories = companyInfoFromMoci.signatories != null ? companyInfoFromMoci.signatories.ConvertAll(sg => new CompanyInfoSignatory
                {
                    PersonNameAr = sg.person.nameAr,
                    PersonNameEn = sg.person.nameEn,
                    CivilId = sg.person.idNumber,
                    Designation = new Designation()
                    {
                        code = sg.designation.code,
                        nameAr = sg.designation.nameAr,
                        nameEn = sg.designation.nameEn,
                    },
                    SignatureType = new Signaturetype()
                    {
                        code = sg.signatureType.code,
                        nameAr = sg.signatureType.nameAr,
                        nameEn = sg.signatureType.nameEn,
                    },
                    SignatoryStartDate = Convert.ToDateTime(sg.signatoryStartDate),
                }) : null


            };

            var newGuid = Guid.NewGuid();
            var appUser = new AppUser
            {
                Email = freelancerRequestDto.Email,
                UserName = freelancerRequestDto.Email.ToLower(),
                EmailConfirmed = false,
                Password = "A@123456b",
                PhoneNumber = freelancerRequestDto.PhoneNumber,
                PhoneNumberConfirmed = true,
                OTP = string.Empty,
                Profile = profile,
                CreatedDate = DateTime.Now,
                VerificationToken = newGuid.ToString(),
                RoleName = "vendor",
                IsVendor = true,
                
            };

            object createdUser;
            try
            {
                createdUser = await _userRepository.CreateUserAsync(appUser);
                await _userRepository.ApplyRegistrationIdentityAccessAsync(
                    freelancerRequestDto.Email,
                    (int)Enums.ProfileTypes.FreeLancer);
                await SendVerificationEmailAsync(appUser);
            }
            catch (Exception e)
            {
                throw new SafeExecutionHelper.InternalServerErrorException(e.Message);
            }

            return createdUser;
        }
        public async Task<object> RegisterOperatorsAsync(RegisterOperatorsRequestDto operatorsRequestDto)
        {
            // compare OTP with OTConfirmed
            if (OtpEncryptionHelper.CompareOtp(operatorsRequestDto.OTP, operatorsRequestDto.OtpConfirmed) == false)
            {
                throw new SafeExecutionHelper.NotAcceptableException("OTP is not valid.");
            }

            var _mociHelper = new MociHelper(_configuration);
            var companyInfoFromMoci = await _mociHelper.GetDataByCrAsync(operatorsRequestDto.CrNumber);
            if (companyInfoFromMoci == null)
                throw new SafeExecutionHelper.NotFoundException(
                    $"CR number '{operatorsRequestDto.CrNumber}' not found in MOCIIP database.");

            var signatoriesCivilIds = companyInfoFromMoci?.signatories?
                .Where(s => s?.person?.idNumber != null)
                .Select(s => s.person.idNumber)
                .ToList()
                ?? new List<string>();
            if (!signatoriesCivilIds.Any())
                throw new SafeExecutionHelper.NotFoundException(
                    $"CR number '{operatorsRequestDto.CrNumber}' has no signatory in MOCIIP database.");

            //verify Token
            if (string.IsNullOrWhiteSpace(operatorsRequestDto.TempToken))
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"Invalid Token");

            IsValidTempTokenForCompany(operatorsRequestDto.TempToken, signatoriesCivilIds, operatorsRequestDto.PhoneNumber);

            var foundUserByEmail = await _userRepository.GetUserByEmailAsync(operatorsRequestDto.Email);
            if (foundUserByEmail != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with email '{operatorsRequestDto.Email}' already exists.");
            var foundUserByPhone = await _userRepository.GetUserProfileByPhoneNumberAsync(operatorsRequestDto.PhoneNumber);
            if (foundUserByPhone != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with phone number '{operatorsRequestDto.PhoneNumber}' already exists.");
            var foundUserByCrNumber = await _userRepository.GetUserProfileByCrNumberAsync(operatorsRequestDto.CrNumber);
            if (foundUserByCrNumber != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with CR number '{operatorsRequestDto.CrNumber}' already exists.");
            
            
            var setting = CreateDefaultProfileSetting();
            var profile = new Profile
            {
                Name = operatorsRequestDto.CompanyName,
                CrNumber = operatorsRequestDto.CrNumber,
                CompanyNameEnglish = operatorsRequestDto.CompanyName,
                OperatorRegisterationAttachment = operatorsRequestDto.AttachmentFileName,
                IsActive = false, // default to false, until approved by TRA
                Settings = setting,
                EmailAddress = operatorsRequestDto.Email,
                PhoneNumber = operatorsRequestDto.PhoneNumber,
                JoinDate = DateTime.Now,
                IsOmani = true,
                ProfileType = (int)Enums.ProfileTypes.Operator,
                IsPostOpertunity = false,
                MissionVision = "Mission and Vision",
            };
            
            profile.CompanyInfo = new CompanyInfo()
            {
                CrNumber = companyInfoFromMoci.crNo.crNumber,
                CompanyNameArabic = companyInfoFromMoci.crNo.companyNameArabic,
                CompanyNameEnglish = companyInfoFromMoci.crNo.companyNameEnglish,
                CompanyNameWithLegalArabic = companyInfoFromMoci.crNo.companyNameWithLegalArabic,
                CompanyNameWithLegalEnglish = companyInfoFromMoci.crNo.companyNameWithLegalEnglish,
                CompanyStatus = new RegistrationStatus()
                {
                    Code = companyInfoFromMoci.crNo.registrationStatus.code,
                    NameAr = companyInfoFromMoci.crNo.registrationStatus.nameAr,
                    NameEn = companyInfoFromMoci.crNo.registrationStatus.nameEn,
                },
                LegalStatus = new LegalStatus()
                {
                    Code = companyInfoFromMoci.crNo.legalStatus.code,
                    NameAr = companyInfoFromMoci.crNo.legalStatus.nameAr,
                    NameEn = companyInfoFromMoci.crNo.legalStatus.nameEn,
                },
                Capital = new Capital()
                {
                    CashCapital = companyInfoFromMoci.capitalData.cashCapital.ToString(),
                    AssetCapital = companyInfoFromMoci.capitalData.assetCapital.ToString(),
                    TotalCapital = companyInfoFromMoci.capitalData.totalCapital.ToString(),
                    ShareValue = companyInfoFromMoci.capitalData.shareValue.ToString(),
                    TotalShares = companyInfoFromMoci.capitalData.totalShares.ToString(),
                    CapitalVerified = companyInfoFromMoci.capitalData.capitalVerified,
                },
                RegistrationDate = Convert.ToDateTime(companyInfoFromMoci.crNo.registrationDate),
                ExpiryDate = Convert.ToDateTime(companyInfoFromMoci.crNo.expiryDate),
                CompanyGrade = "Unknown",
                Location = new Location()
                {
                    Governorate = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].governorate.nameEn : "",
                    Town = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].town.nameEn : "",
                    State = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].state.nameEn : "",
                },
                SubjectToForeignInvestment = companyInfoFromMoci.crNo.subjectToForeignInvestment,
                DeclaredActivities = companyInfoFromMoci.activities != null ? companyInfoFromMoci.activities.ConvertAll(da => new DeclaredActivity
                {
                    IsicVersion = da.isicVersion,
                    IsicCode = da.isicCode,
                    NameAr = da.nameAr,
                    NameEn = da.nameEn,
                    RegistrationDate = da.registrationDate,
                }) : null,
                Signatories = companyInfoFromMoci.signatories != null ? companyInfoFromMoci.signatories.ConvertAll(sg => new CompanyInfoSignatory
                {
                    PersonNameAr = sg.person.nameAr,
                    PersonNameEn = sg.person.nameEn,
                    CivilId = sg.person.idNumber,
                    Designation = new Designation()
                    {
                        code = sg.designation.code,
                        nameAr = sg.designation.nameAr,
                        nameEn = sg.designation.nameEn,
                    },
                    SignatureType = new Signaturetype()
                    {
                        code = sg.signatureType.code,
                        nameAr = sg.signatureType.nameAr,
                        nameEn = sg.signatureType.nameEn,
                    },
                    SignatoryStartDate = Convert.ToDateTime(sg.signatoryStartDate),
                }) : null


            };
            var newGuid = Guid.NewGuid();
            var appUser = new AppUser
            {
                Email = operatorsRequestDto.Email,
                UserName = operatorsRequestDto.Email.ToLower(),
                EmailConfirmed = false,
                Password = "A@123456bcd",
                PhoneNumber = operatorsRequestDto.PhoneNumber,
                PhoneNumberConfirmed = true,
                OTP = operatorsRequestDto.OTP,
                Profile = profile,
                CreatedDate = DateTime.Now,
                VerificationToken = newGuid.ToString(),
                RoleName = "vendor",
                IsVendor = true,
                
            };

            
            try
            {
                var createdUser = await _userRepository.CreateUserAsync(appUser);
                await SendVerificationEmailAsync(appUser);
                
                // TODO: Add logic To send notification to operators about their account
                // it will be pending. until TRA approval.
                var response = new
                {
                    IsSuccess = true, 
                    Message = "User created successfully, waiting for TRA approval"
                };
                return response; ;
            }
            catch (Exception e)
            {
                throw new SafeExecutionHelper.InternalServerErrorException(e.Message);
            }

           
        }
        public async Task<object> RegisterOperatorsWithtoutOtpAsync(RegisterOperatorsWithoutOtpRequestDto operatorsRequestDto)
        {
            var _mociHelper = new MociHelper(_configuration);
            var companyInfoFromMoci = await _mociHelper.GetDataByCrAsync(operatorsRequestDto.CrNumber);
            if (companyInfoFromMoci == null)
                throw new SafeExecutionHelper.NotFoundException(
                    $"CR number '{operatorsRequestDto.CrNumber}' not found in MOCIIP database.");

            var signatoriesCivilIds = companyInfoFromMoci?.signatories?
                .Where(s => s?.person?.idNumber != null)
                .Select(s => s.person.idNumber)
                .ToList()
                ?? new List<string>();
            if (!signatoriesCivilIds.Any())
                throw new SafeExecutionHelper.NotFoundException(
                    $"CR number '{operatorsRequestDto.CrNumber}' has no signatory in MOCIIP database.");

            //verify Token
            if (string.IsNullOrWhiteSpace(operatorsRequestDto.TempToken))
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"Invalid Token");

            IsValidTempTokenForCompany(operatorsRequestDto.TempToken, signatoriesCivilIds, operatorsRequestDto.PhoneNumber);


            var foundUserByEmail = await _userRepository.GetUserByEmailAsync(operatorsRequestDto.Email);
            if (foundUserByEmail != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with email '{operatorsRequestDto.Email}' already exists.");
            var foundUserByPhone = await _userRepository.GetUserProfileByPhoneNumberAsync(operatorsRequestDto.PhoneNumber);
            if (foundUserByPhone != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with phone number '{operatorsRequestDto.PhoneNumber}' already exists.");
            var foundUserByCrNumber = await _userRepository.GetUserProfileByCrNumberAsync(operatorsRequestDto.CrNumber);
            if (foundUserByCrNumber != null)
                throw new SafeExecutionHelper.NotAcceptableException(
                    $"User with CR number '{operatorsRequestDto.CrNumber}' already exists.");
            
            
            var setting = CreateDefaultProfileSetting();
            var profile = new Profile
            {
                Name = operatorsRequestDto.CompanyName,
                CrNumber = operatorsRequestDto.CrNumber,
                CompanyNameEnglish = operatorsRequestDto.CompanyName,
                OperatorRegisterationAttachment = operatorsRequestDto.AttachmentFileName,
                IsActive = false, // default to false, until approved by TRA
                Settings = setting,
                EmailAddress = operatorsRequestDto.Email,
                PhoneNumber = operatorsRequestDto.PhoneNumber,
                JoinDate = DateTime.Now,
                IsOmani = true,
                ProfileType = (int)Enums.ProfileTypes.Operator,
                IsPostOpertunity = false,
                MissionVision = "Mission and Vision",
            };
           
            profile.CompanyInfo = new CompanyInfo()
            {
                CrNumber = companyInfoFromMoci.crNo.crNumber,
                CompanyNameArabic = companyInfoFromMoci.crNo.companyNameArabic,
                CompanyNameEnglish = companyInfoFromMoci.crNo.companyNameEnglish,
                CompanyNameWithLegalArabic = companyInfoFromMoci.crNo.companyNameWithLegalArabic,
                CompanyNameWithLegalEnglish = companyInfoFromMoci.crNo.companyNameWithLegalEnglish,
                CompanyStatus = new RegistrationStatus()
                {
                    Code = companyInfoFromMoci.crNo.registrationStatus.code,
                    NameAr = companyInfoFromMoci.crNo.registrationStatus.nameAr,
                    NameEn = companyInfoFromMoci.crNo.registrationStatus.nameEn,
                },
                LegalStatus = new LegalStatus()
                {
                    Code = companyInfoFromMoci.crNo.legalStatus.code,
                    NameAr = companyInfoFromMoci.crNo.legalStatus.nameAr,
                    NameEn = companyInfoFromMoci.crNo.legalStatus.nameEn,
                },
                Capital = new Capital()
                {
                    CashCapital = companyInfoFromMoci.capitalData.cashCapital.ToString(),
                    AssetCapital = companyInfoFromMoci.capitalData.assetCapital.ToString(),
                    TotalCapital = companyInfoFromMoci.capitalData.totalCapital.ToString(),
                    ShareValue = companyInfoFromMoci.capitalData.shareValue.ToString(),
                    TotalShares = companyInfoFromMoci.capitalData.totalShares.ToString(),
                    CapitalVerified = companyInfoFromMoci.capitalData.capitalVerified,
                },
                RegistrationDate = Convert.ToDateTime(companyInfoFromMoci.crNo.registrationDate),
                ExpiryDate = Convert.ToDateTime(companyInfoFromMoci.crNo.expiryDate),
                CompanyGrade = "Unknown",
                Location = new Location()
                {
                    Governorate = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].governorate.nameEn : "",
                    Town = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].town.nameEn : "",
                    State = companyInfoFromMoci.businessLocation != null && companyInfoFromMoci.businessLocation.Count > 0 ? companyInfoFromMoci.businessLocation[0].state.nameEn : "",
                },
                SubjectToForeignInvestment = companyInfoFromMoci.crNo.subjectToForeignInvestment,
                DeclaredActivities = companyInfoFromMoci.activities != null ? companyInfoFromMoci.activities.ConvertAll(da => new DeclaredActivity
                {
                    IsicVersion = da.isicVersion,
                    IsicCode = da.isicCode,
                    NameAr = da.nameAr,
                    NameEn = da.nameEn,
                    RegistrationDate = da.registrationDate,
                }) : null,
                Signatories = companyInfoFromMoci.signatories != null ? companyInfoFromMoci.signatories.ConvertAll(sg => new CompanyInfoSignatory
                {
                    PersonNameAr = sg.person.nameAr,
                    PersonNameEn = sg.person.nameEn,
                    CivilId = sg.person.idNumber,
                    Designation = new Designation()
                    {
                        code = sg.designation.code,
                        nameAr = sg.designation.nameAr,
                        nameEn = sg.designation.nameEn,
                    },
                    SignatureType = new Signaturetype()
                    {
                        code = sg.signatureType.code,
                        nameAr = sg.signatureType.nameAr,
                        nameEn = sg.signatureType.nameEn,
                    },
                    SignatoryStartDate = Convert.ToDateTime(sg.signatoryStartDate),
                }) : null


            };
            var newGuid = Guid.NewGuid();
            var appUser = new AppUser
            {
                Email = operatorsRequestDto.Email,
                UserName = operatorsRequestDto.Email.ToLower(),
                EmailConfirmed = false,
                Password = "A@123456b",
                PhoneNumber = operatorsRequestDto.PhoneNumber,
                PhoneNumberConfirmed = true,
                OTP = string.Empty,
                Profile = profile,
                CreatedDate = DateTime.Now,
                VerificationToken = newGuid.ToString(),
                RoleName = "client",
                IsVendor = true,
            };

            try
            {
                var createdUser = await _userRepository.CreateUserAsync(appUser);
                await SendVerificationEmailAsync(appUser);
                
                // TODO: Add logic To send notification to operators about their account
                // it will be pending. until TRA approval.
                var response = new
                {
                    IsSuccess = true, 
                    Message = "User created successfully, waiting for TRA approval"
                };
                return response; ;
            }
            catch (Exception e)
            {
                throw new SafeExecutionHelper.InternalServerErrorException(e.Message);
            }

           
        }
        public async Task<bool> SendVerificationEmailAsync(AppUser registerSmeRequest)
        {
            try
            {
                return await _notificationDispatchService.SendRegistrationEmailVerificationAsync(registerSmeRequest);
            }
            catch (Exception e)
            {
                throw new SafeExecutionHelper.InternalServerErrorException(e.Message);
            }
        }
        public async Task<object> RegisterJobseekerAsync(RegisterJobseekerRequestDto jobseekerRequestDto)
        {
            if (string.IsNullOrWhiteSpace(jobseekerRequestDto?.OTP) ||
                string.IsNullOrWhiteSpace(jobseekerRequestDto?.OtpConfirmed) ||
                OtpEncryptionHelper.CompareOtp(jobseekerRequestDto.OTP, jobseekerRequestDto.OtpConfirmed) == false)
            {
                throw new SafeExecutionHelper.NotAcceptableException("OTP is not valid.");
            }

            var nafathDetails = await EnsureJobseekerRegistrationAvailableAsync(jobseekerRequestDto);

            var email = jobseekerRequestDto.Email.Trim();
            var civilId = jobseekerRequestDto.CivilId.Trim();
            var phone = (jobseekerRequestDto.MobileNumber ?? string.Empty).Trim();

            // When PKI issued a temp token, require it and match civil id / mobile claims.
            if (!string.IsNullOrWhiteSpace(jobseekerRequestDto.TempToken))
            {
                if (!IsValidTempTokenForFreelancer(jobseekerRequestDto.TempToken, civilId, phone))
                {
                    throw new SafeExecutionHelper.NotAcceptableException("PKI temporary token is not valid.");
                }
            }
            else if (await IsPkiEnabledAsync())
            {
                throw new SafeExecutionHelper.NotAcceptableException(
                    "PKI registration requires a temporary token. Sign in with PKI first.");
            }

            var displayNameEn = jobseekerRequestDto.Name_en.Trim();
            var displayNameAr = jobseekerRequestDto.Name_ar.Trim();
            var isOmani = string.IsNullOrWhiteSpace(jobseekerRequestDto.NationalityCode)
                || string.Equals(jobseekerRequestDto.NationalityCode, "OM", StringComparison.OrdinalIgnoreCase);

            var profile = await _userRepository.GetUnregisteredJobSeekerProfileByCivilIdAsync(civilId);
            if (profile != null)
            {
                JobSeekerImportRules.ClaimImportedProfile(profile, displayNameEn, displayNameAr, email, phone, isOmani, DateTime.Now);
                if (profile.Settings == null)
                {
                    profile.Settings = CreateDefaultProfileSetting();
                }
            }
            else profile = new Profile
            {
                CrNumber = civilId,
                Name = displayNameEn,
                NameAr = displayNameAr,
                CompanyNameEnglish = displayNameEn,
                CompanyNameArabic = displayNameAr,
                Settings = CreateDefaultProfileSetting(),
                EmailAddress = email,
                PhoneNumber = phone,
                PreferredLanguage = "en",
                IsActive = true,
                JoinDate = DateTime.Now,
                // Jobseeker self-registration defaults nationality to Oman (UI no longer collects it).
                IsOmani = isOmani,
                ProfileType = (int)Enums.ProfileTypes.JobSeeker,
                IsPostOpertunity = false,
                UserProfileCompleted = false,
                MissionVision = string.Empty,
                AboutUs = string.Empty,
            };
            ApplyNafathDetailsToProfile(profile, nafathDetails);

            var newGuid = Guid.NewGuid();
            var appUser = new AppUser
            {
                Email = email,
                UserName = email.ToLower(),
                EmailConfirmed = false,
                Password = JobseekerRegistrationDefaultPassword,
                PhoneNumber = phone,
                PhoneNumberConfirmed = !string.IsNullOrWhiteSpace(phone),
                OTP = jobseekerRequestDto.OTP,
                Profile = profile,
                CreatedDate = DateTime.Now,
                VerificationToken = newGuid.ToString(),
                RoleName = "vendor",
                IsVendor = true,
            };

            object createdUser;
            try
            {
                createdUser = await _userRepository.CreateUserAsync(appUser);
                await _userRepository.ApplyRegistrationIdentityAccessAsync(
                    email.ToLower(),
                    (int)Enums.ProfileTypes.JobSeeker);
                if (await IsRegistrationEmailEnabledAsync())
                {
                    await SendVerificationEmailAsync(appUser);
                }
            }
            catch (Exception e)
            {
                throw new SafeExecutionHelper.InternalServerErrorException(e.Message);
            }

            var createdProfile = createdUser as Profile;

            return new
            {
                UserId = email.ToLower(),
                MockUserId = string.Empty,
                ProfileId = createdProfile?.Id ?? 0,
                ProfileType = (int)Enums.ProfileTypes.JobSeeker,
            };
        }
        public async Task<object> ValidateJobseekerRegistrationAsync(RegisterJobseekerRequestDto jobseekerRequestDto)
        {
            await EnsureJobseekerRegistrationAvailableAsync(jobseekerRequestDto);
            return new { ok = true };
        }

        public async Task<object> VerifyJobseekerEmailAsync(string email)
        {
            var trimmedEmail = (email ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(trimmedEmail))
            {
                throw new SafeExecutionHelper.NotAcceptableException("Email is required.");
            }

            var findUser = await _userRepository.GetUserByEmailAsync(trimmedEmail);
            if (findUser == null)
            {
                throw new SafeExecutionHelper.NotAcceptableException($"User with email '{trimmedEmail}' was not found.");
            }

            var findUserWithProfile = await _userRepository.GetUserByIdAsync(findUser.Id);
            if (findUserWithProfile == null ||
                findUserWithProfile.ProfileType != (int)Enums.ProfileTypes.JobSeeker)
            {
                throw new SafeExecutionHelper.NotAcceptableException("Email verification is only available for job seeker accounts.");
            }

            if (!findUser.EmailConfirmed)
            {
                findUser.EmailConfirmed = true;
                findUser.VerificationToken = Guid.NewGuid().ToString();
                await _userRepository.UpdateUserAsync(findUser);
            }

            return new
            {
                Email = trimmedEmail,
                ProfileId = findUser.ProfileId ?? 0,
                EmailConfirmed = true,
            };
        }

        private async Task<NafathJobSeekerDetailsDto> EnsureJobseekerRegistrationAvailableAsync(
            RegisterJobseekerRequestDto jobseekerRequestDto)
        {
            var email = (jobseekerRequestDto?.Email ?? string.Empty).Trim();
            var civilId = (jobseekerRequestDto?.CivilId ?? string.Empty).Trim();
            var phone = (jobseekerRequestDto?.MobileNumber ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(civilId) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(jobseekerRequestDto?.Name_en) ||
                string.IsNullOrWhiteSpace(jobseekerRequestDto?.Name_ar))
            {
                throw new SafeExecutionHelper.NotAcceptableException("Invalid registration payload.");
            }

            var foundUserByEmail = await _userRepository.GetUserByEmailAsync(email);
            if (foundUserByEmail != null)
            {
                throw new SafeExecutionHelper.NotAcceptableException($"User with email '{email}' already exists.");
            }

            var foundUserByPhone = await _userRepository.GetUserProfileByPhoneNumberAsync(phone);
            if (foundUserByPhone != null)
            {
                throw new SafeExecutionHelper.NotAcceptableException($"User with phone number '{phone}' already exists.");
            }

            var foundUserByCivilId = await _userRepository.GetUserProfileByCrNumberAsync(civilId);
            if (foundUserByCivilId != null)
            {
                throw new SafeExecutionHelper.NotAcceptableException($"User with civil ID '{civilId}' already exists.");
            }

            return await EnsureActiveNafathJobSeekerAsync(civilId);
        }
        public async Task<object> RequestJobseekerRegistrationOtpAsync(JobseekerRegistrationOtpRequestDto request)
        {
            if (await IsRegistrationSmsEnabledAsync())
            {
                var phone = (request?.PhoneNumber ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(phone))
                {
                    throw new SafeExecutionHelper.NotAcceptableException("Mobile number is required when SMS is enabled.");
                }

                var encryptedOtp = await _authService.SendVerificationCodeAsync(new SendVerificationRequest
                {
                    PhoneNumber = phone,
                    Email = (request?.Email ?? string.Empty).Trim(),
                });

                return new
                {
                    SessionId = encryptedOtp?.ToString() ?? string.Empty,
                    ExpiresInSeconds = 300,
                };
            }

            var plainOtp = GenerateRegistrationOtp();
            var encryptedSession = OtpEncryptionHelper.EncryptOtp(plainOtp);
            return new
            {
                SessionId = encryptedSession,
                OtpCode = plainOtp,
                ExpiresInSeconds = 300,
            };
        }
        public bool IsValidRegistrationOtp(string otp, string otpConfirmed)
        {
            if (string.IsNullOrWhiteSpace(otp) || string.IsNullOrWhiteSpace(otpConfirmed))
            {
                return false;
            }

            return OtpEncryptionHelper.CompareOtp(otp, otpConfirmed);
        }
        public async Task<object> ValidateNewUserRegisteration(ValidateUserDto validateUserDto)
        {
     
            var foundUserByEmail = await _userRepository.GetUserByEmailAsync(validateUserDto.Email);
    
            var foundUserByPhone = await _userRepository.GetUserProfileByPhoneNumberAsync(validateUserDto.PhoneNumber);
            
            var foundUserByCrNumber = await _userRepository.GetUserProfileByCrNumberAsync(validateUserDto.refNumber);
            

            var response = new
            {
                IsSuccess = foundUserByPhone == null && foundUserByEmail == null && foundUserByCrNumber == null,
                Data = new  { phone= foundUserByPhone != null , email= foundUserByEmail != null, refNumber= foundUserByCrNumber != null }
            };
            return response; 
        }
        private static ProfileSetting CreateDefaultProfileSetting()
        {
            return new ProfileSetting
            {
                PreferredLanguage = LanguageEnum.English,
                SendEmail = true,
                SendSms = true,
                ShowEmail = false,
                ShowMobileNo = false
            };
        }
        private static string GenerateRegistrationOtp(int length = 6)
        {
            var random = new Random();
            var otp = string.Empty;
            for (var i = 0; i < length; i++)
            {
                otp += random.Next(0, 10);
            }

            return otp;
        }

        private static bool IsYnEnabled(string raw)
        {
            return string.Equals((raw ?? "N").Trim(), "Y", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<bool> IsRegistrationSmsEnabledAsync()
        {
            var raw = await _adminSystemService.GetSystemVariableValueAsync(SystemVariableCodes.RegistrationSmsEnabled);
            return IsYnEnabled(raw);
        }

        private async Task<bool> IsRegistrationEmailEnabledAsync()
        {
            var raw = await _adminSystemService.GetSystemVariableValueAsync(SystemVariableCodes.RegistrationEmailEnabled);
            return IsYnEnabled(raw);
        }

        private async Task<bool> IsPkiEnabledAsync()
        {
            var raw = await _adminSystemService.GetSystemVariableValueAsync(SystemVariableCodes.PkiEnabled);
            return IsYnEnabled(raw);
        }

        public bool IsValidTempTokenForFreelancer(string tempToken, string civilId, string mobile)
        {
            try
            {
                VerifyTempTokenRequestDto verifyTempTokenRequestDto = new VerifyTempTokenRequestDto()
                {
                    Token = tempToken
                };
                var res = _authService.VerifyTempToken(verifyTempTokenRequestDto);

                if (res == null || (string.IsNullOrWhiteSpace(civilId) && string.IsNullOrWhiteSpace(mobile)))
                    return false;

                    // Validate CivilId claim 
                    var tokenCivilId = res.Claims.FirstOrDefault(c => c.Type == "CivilId")?.Value;
                    if (string.IsNullOrWhiteSpace(tokenCivilId) || tokenCivilId != civilId)
                        return false;

                    // Validate Mobile claim (if present)
                    var tokenMobile = res.Claims.FirstOrDefault(c => c.Type == "Mobile")?.Value;
                    if (!string.IsNullOrWhiteSpace(tokenMobile) && tokenMobile != mobile)
                        return false;
               
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private void IsValidTempTokenForCompany(string tempToken, List<string> signatoriesCivilIds, string mobile)
        {
           
                VerifyTempTokenRequestDto verifyTempTokenRequestDto = new VerifyTempTokenRequestDto()
                {
                    Token = tempToken
                };
                var res = _authService.VerifyTempToken(verifyTempTokenRequestDto);

                if (res == null || 
                   signatoriesCivilIds == null ||
                   !signatoriesCivilIds.Any())
                        throw new SafeExecutionHelper.NotFoundException(
                       $"CR has no signatory in MOCIIP database.");

            // Validate CivilId claim 
            var tokenCivilId = res.Claims.FirstOrDefault(c => c.Type == "CivilId")?.Value;
                if (string.IsNullOrWhiteSpace(tokenCivilId) || !signatoriesCivilIds.Contains(tokenCivilId))
                    throw new SafeExecutionHelper.NotFoundException(
                    $"The entered CR has no signatory that matches with your civil ID.");

                // Validate Mobile claim (if present)
                var tokenMobile = res.Claims.FirstOrDefault(c => c.Type == "Mobile")?.Value;
                if (!string.IsNullOrWhiteSpace(tokenMobile) && tokenMobile != mobile)
                throw new SafeExecutionHelper.NotFoundException(
                $"The entered mobile is not matched with the authenticated mobile");

        }
    }
}
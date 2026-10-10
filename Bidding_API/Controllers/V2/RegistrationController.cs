using System;
using System.Threading;
using System.Threading.Tasks;
using Bidding_API.Helpers;
using Bidding_Common.Helpers;
using Bidding_Core.Services.Interfaces;
using Bidding_Models.DTOs;
using Bidding_Models.Jobseekers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Bidding_API.Controllers.V2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationUserService _registrationUserService;
        private readonly IAdminSystemService _adminSystemService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(
            IRegistrationUserService registrationUserService,
            IAdminSystemService adminSystemService,
            IConfiguration configuration,
            ILogger<RegistrationController> logger
        )
        {
            _registrationUserService = registrationUserService;
            _adminSystemService = adminSystemService;
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("auth-settings")]
        public async Task<IActionResult> GetAuthSettings()
        {
            var pkiRaw = (await _adminSystemService.GetSystemVariableValueAsync(SystemVariableCodes.PkiEnabled) ?? "N").Trim();
            if (string.IsNullOrEmpty(pkiRaw))
            {
                pkiRaw = "N";
            }

            var smsRaw = (await _adminSystemService.GetSystemVariableValueAsync(SystemVariableCodes.RegistrationSmsEnabled) ?? "N").Trim();
            if (string.IsNullOrEmpty(smsRaw))
            {
                smsRaw = "N";
            }

            var emailRaw = (await _adminSystemService.GetSystemVariableValueAsync(SystemVariableCodes.RegistrationEmailEnabled) ?? "N").Trim();
            if (string.IsNullOrEmpty(emailRaw))
            {
                emailRaw = "N";
            }

            var loginOtpRaw = (await _adminSystemService.GetSystemVariableValueAsync(SystemVariableCodes.LoginOtpEnabled) ?? "N").Trim();
            if (string.IsNullOrEmpty(loginOtpRaw))
            {
                loginOtpRaw = "N";
            }

            // Appsettings Auth:LoginOtpEnabled must also be true; Production/Staging keep it false.
            var loginOtpEnabled = IsAuthLoginOtpConfigEnabled()
                                  && string.Equals(loginOtpRaw, "Y", StringComparison.OrdinalIgnoreCase);

            return Ok(new
            {
                Code = "Success",
                Data = new
                {
                    PkiEnabled = string.Equals(pkiRaw, "Y", StringComparison.OrdinalIgnoreCase),
                    PkiEnabledRaw = pkiRaw.ToUpperInvariant(),
                    LoginOtpEnabled = loginOtpEnabled,
                    LoginOtpEnabledRaw = loginOtpEnabled ? "Y" : "N",
                    RegistrationSmsEnabled = string.Equals(smsRaw, "Y", StringComparison.OrdinalIgnoreCase),
                    RegistrationSmsEnabledRaw = smsRaw.ToUpperInvariant(),
                    RegistrationEmailEnabled = string.Equals(emailRaw, "Y", StringComparison.OrdinalIgnoreCase),
                    RegistrationEmailEnabledRaw = emailRaw.ToUpperInvariant(),
                }
            });
        }

        private bool IsAuthLoginOtpConfigEnabled()
        {
            var raw = (_configuration["Auth:LoginOtpEnabled"] ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(raw))
            {
                return false;
            }

            if (bool.TryParse(raw, out var b))
            {
                return b;
            }

            return string.Equals(raw, "Y", StringComparison.OrdinalIgnoreCase)
                   || string.Equals(raw, "1", StringComparison.OrdinalIgnoreCase);
        }
        [HttpGet("verify-with-riyada/{crNumber}")]
        public async Task<IActionResult> VerifyWithRiyada(string crNumber, CancellationToken cancellationToken)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
                {
                    var response = await _registrationUserService.GetRiyadaCompanyData(crNumber, cancellationToken);
                    if (response == null)
                    {
                        return ApiResponseHelper.CreateErrorResponse(
                            "CompanyDataNotFound",
                            "CR number not found in Riyada system",
                            StatusCodes.Status404NotFound);
                    }

                    return new OkObjectResult(new
                    {
                        Code = "Success",
                        Data = response
                    });
                },
                ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("verify-with-mol/{civilId}")]
        public async Task<IActionResult> VerifyWithMol(string civilId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("verify-with-mol START civilId={CivilId}", civilId);
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var response = await _registrationUserService.IsValidCivilIdWithMol(civilId, cancellationToken);
                if (response == null)
                {
                    _logger.LogInformation("verify-with-mol NOT FOUND civilId={CivilId}", civilId);
                    return ApiResponseHelper.CreateErrorResponse(
                        "UserDataNotFound",
                        "Invalid Civil ID",
                        StatusCodes.Status404NotFound);
                }

                _logger.LogInformation("verify-with-mol OK civilId={CivilId}", civilId);
                return new OkObjectResult(new
                {
                    Code = "Success",
                    Data = response
                });
            },
                ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("civilid-from-moci/{crNumber}")]
        public async Task<IActionResult> CivilIdFromMoci(string crNumber)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var civilId = await _registrationUserService.GetCivilIdByCrNumber(crNumber);
               
                return new OkObjectResult(new
                {
                    Code = "Success",
                    Data = civilId
                });
            },
                ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("companyname-from-moci/{crNumber}")]
        public async Task<IActionResult> CompanyNameFromMoci(string crNumber)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var companyName = await _registrationUserService.GetCompanyNameByCrNumber(crNumber);

                return new OkObjectResult(new
                {
                    Code = "Success",
                    Data = companyName
                });
            },
                ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("register-sme")]
        public async Task<IActionResult> RegisterSme([FromBody] RegisterSmeRequestDto smeModel)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
                {
                    var response = await _registrationUserService.RegisterSmeAsync(smeModel);

                    return Ok(new
                    {
                        Code = "Success",
                        Data = response
                    });
                },
                ApiResponseHelper.CreateErrorResponse);
        }
        
        [HttpPost("register-sme-token")]
        public async Task<IActionResult> RegisterSmeWithoutOtp([FromBody] RegisterSmeWithoutOtpRequestDto smeModel)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var response = await _registrationUserService.RegisterSmeWithoutOtpAsync(smeModel);

                return Ok(new
                {
                    Code = "Success",
                    Data = response
                });
            },
                ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("validate-registeration")]
        public async Task<IActionResult> ValidateRegisteration([FromBody] ValidateUserDto validateUserDto)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {

                var response = await _registrationUserService.ValidateNewUserRegisteration(validateUserDto);

                return Ok(response);
            },
                ApiResponseHelper.CreateErrorResponse);
        }
        [HttpPost("is-valid-temp-token")]
        public async Task<IActionResult> IsValidTempToken([FromBody] VerifyTempTokenRequestDto verifyTempTokenDto)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {

                bool response = _registrationUserService.IsValidTempTokenForFreelancer(verifyTempTokenDto.Token, verifyTempTokenDto.CivilId, verifyTempTokenDto.Mobile);

                return Ok(new { IsValid = response });
            },
                ApiResponseHelper.CreateErrorResponse);
        }
        [HttpPost("register-freelancer")]
        public async Task<IActionResult> RegisterFreelancer([FromBody] RegisterFreelancerRequestDto freelancerRequestDto)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
                {
                    var response = await _registrationUserService.RegisterFreelancerAsync(freelancerRequestDto);

                    return Ok(new
                    {
                        Code = "Success",
                        Data = response
                    });
                },
                ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("register-freelancer-token")]
        public async Task<IActionResult> RegisterFreelancerWithoutOtp([FromBody] RegisterFreelancerWithoutOtpRequestDto freelancerRequestDto)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var response = await _registrationUserService.RegisterFreelancerWithoutOtpAsync(freelancerRequestDto);

                return Ok(new
                {
                    Code = "Success",
                    Data = response
                });
            },
                ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("register-jobseeker")]
        public async Task<IActionResult> RegisterJobseeker([FromBody] RegisterJobseekerRequestDto jobseekerRequestDto)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var response = await _registrationUserService.RegisterJobseekerAsync(jobseekerRequestDto);

                return Ok(new
                {
                    Code = "Success",
                    Data = response
                });
            },
                ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("register-jobseeker-validate")]
        public async Task<IActionResult> ValidateJobseekerRegistration([FromBody] RegisterJobseekerRequestDto jobseekerRequestDto)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var response = await _registrationUserService.ValidateJobseekerRegistrationAsync(jobseekerRequestDto);

                return Ok(new
                {
                    Code = "Success",
                    Data = response
                });
            },
                ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("register-jobseeker-verify-email")]
        public async Task<IActionResult> VerifyJobseekerEmail([FromBody] VerifyJobseekerEmailRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var response = await _registrationUserService.VerifyJobseekerEmailAsync(body?.Email);

                return Ok(new
                {
                    Code = "Success",
                    Data = response
                });
            },
                ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("register-jobseeker-otp")]
        public async Task<IActionResult> RequestJobseekerRegistrationOtp([FromBody] JobseekerRegistrationOtpRequestDto body)
        {
            var response = await _registrationUserService.RequestJobseekerRegistrationOtpAsync(body);
            return Ok(new
            {
                Code = "Success",
                Data = response
            });
        }

        [HttpPost("validate-registration-otp")]
        public IActionResult ValidateRegistrationOtp([FromBody] ValidateRegistrationOtpRequestDto body)
        {
            var valid = _registrationUserService.IsValidRegistrationOtp(body?.OTP, body?.OtpConfirmed);
            return Ok(new
            {
                Code = "Success",
                Data = new { Valid = valid }
            });
        }

        [HttpPost("register-operators")]
        public async Task<IActionResult> RegisterOperators([FromBody] RegisterOperatorsRequestDto operatorsRequestDto)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
                {
                    var response = await _registrationUserService.RegisterOperatorsAsync(operatorsRequestDto);

                    return Ok(new
                    {
                        Code = "Success",
                        Data = response
                    });
                },
                ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("register-operators-token")]
        public async Task<IActionResult> RegisterOperatorsWithtoutOtp([FromBody] RegisterOperatorsWithoutOtpRequestDto operatorsRequestDto)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var response = await _registrationUserService.RegisterOperatorsWithtoutOtpAsync(operatorsRequestDto);

                return Ok(new
                {
                    Code = "Success",
                    Data = response
                });
            },
                ApiResponseHelper.CreateErrorResponse);
        }


        [HttpGet("countries")]
        public IActionResult GetCountries()
        {
            var countries = new[]
            {
                new { Name = "Oman", NameArabic = "عمان" },
                new { Name = "United Arab Emirates", NameArabic = "الامارات العربية المتحدة" },
                new { Name = "South Africa", NameArabic = "جنوب أفريقيا" },
                new { Name = "Republic of Kenya", NameArabic = "جمهورية كينيا" },
                new { Name = "Algeria", NameArabic = "الجزائر" },
                new { Name = "Egypt", NameArabic = "مصر" },
                new { Name = "Jordan", NameArabic = "الاردن" },
                new { Name = "Libya", NameArabic = "ليبيا" },
                new { Name = "Morocco", NameArabic = "المغرب" },
                new { Name = "Syria", NameArabic = "سوريا" },
                new { Name = "Tunisia", NameArabic = "تونس" },
                new { Name = "Brunei", NameArabic = "بروناي" },
                new { Name = "China", NameArabic = "الصين" },
                new { Name = "Hong Kong", NameArabic = "هونغ كونغ" },
                new { Name = "India", NameArabic = "الهند" },
                new { Name = "Iran", NameArabic = "ايران" },
                new { Name = "Japan", NameArabic = "اليابان" },
                new { Name = "Malaysia", NameArabic = "ماليزيا" },
                new { Name = "Pakistan", NameArabic = "باكستان" },
                new { Name = "Singapore", NameArabic = "سنغافورة" },
                new { Name = "South Korea", NameArabic = "كوريا الجنوبية" },
                new { Name = "Taiwan", NameArabic = "تايوان" },
                new { Name = "Austria", NameArabic = "النمسا" },
                new { Name = "Belarus", NameArabic = "بيلاروسيا" },
                new { Name = "Czech Republic", NameArabic = "جمهورية التشيك" },
                new { Name = "Denmark", NameArabic = "الدنمارك" },
                new { Name = "Finland", NameArabic = "فنلندا" },
                new { Name = "France", NameArabic = "فرنسا" },
                new { Name = "Germany", NameArabic = "ألمانيا" },
                new { Name = "Greece", NameArabic = "اليونان" },
                new { Name = "Hungary", NameArabic = "المجر" },
                new { Name = "Ireland", NameArabic = "ايرلندا" },
                new { Name = "Italy", NameArabic = "ايطاليا" },
                new { Name = "Kingdom of Belgium", NameArabic = "ممكلة بلجيكا" },
                new { Name = "Lithuania", NameArabic = "ليتوانيا" },
                new { Name = "Malta", NameArabic = "مالطا" },
                new { Name = "Kingdom of the Netherlands", NameArabic = "ممكلة نيذرلاندز" },
                new { Name = "Norway", NameArabic = "النرويج" },
                new { Name = "Poland", NameArabic = "بولندا" },
                new { Name = "Portugal", NameArabic = "البرتغال" },
                new { Name = "Republic of Slovenia", NameArabic = "جمهورية سلوفينيا" },
                new { Name = "Russia", NameArabic = "روسيا" },
                new { Name = "Spain", NameArabic = "أسبانيا" },
                new { Name = "Sweden", NameArabic = "السويد" },
                new { Name = "Switzerland", NameArabic = "سويسرا" },
                new { Name = "Republic of Cyprus", NameArabic = "جمهورية قبرص" },
                new { Name = "Turkey", NameArabic = "تركيا" },
                new { Name = "United Kingdom", NameArabic = "المملكة المتحدة" },
                new { Name = "Bahrain", NameArabic = "البحرين" },
                new { Name = "Kuwait", NameArabic = "الكويت" },
                new { Name = "Qatar", NameArabic = "قطر" },
                new { Name = "Saudi Arabia", NameArabic = "المملكة العربية السعودية" },
                new { Name = "Australia", NameArabic = "استراليا" },
                new { Name = "New Zealand", NameArabic = "نيوزيلندا" },
                new { Name = "Canada", NameArabic = "كندا" },
                new { Name = "United States of America", NameArabic = "الولايات المتحدة الأمريكية" },
                new { Name = "Brazil", NameArabic = "البرازيل" }
            };
            return Ok(new
            {
                Code = "Success",
                Data = countries
            });
        }
    }
}
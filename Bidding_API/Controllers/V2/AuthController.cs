using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Bidding_API.Common;
using Bidding_API.Helpers;
using Bidding_API.jwtConfigurations;
using Bidding_API.jwtConfigurations.Implementation;
using Bidding_API.jwtConfigurationss.Implementation;
using Bidding_Common.Helpers;
using Bidding_Core.Interfaces;
using Bidding_Core.Repository;
using Bidding_Core.Services.Interfaces;
using Bidding_DLL.Services.Interfaces;
using Bidding_Models;
using Bidding_Models.DatabaseModel;
using Bidding_Models.DTOs;
using Bidding_Models.Jobseekers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Bidding_API.Controllers.V2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IAdminSystemService _adminSystemService;
        private readonly IIdentityAccessAdminService _identityAccessAdminService;


        public AuthController(
            IAuthService authService,
            IConfiguration configuration,
            IUserRepository userRepository,
            IAdminSystemService adminSystemService,
            IIdentityAccessAdminService identityAccessAdminService
        )
        {
            _authService = authService;
            _configuration = configuration;
            _userRepository = userRepository;
            _adminSystemService = adminSystemService;
            _identityAccessAdminService = identityAccessAdminService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                loginRequest ??= new LoginRequestDto();
                loginRequest.StaffOnly = false;
                loginRequest.DisallowPublicPasswordForJobSeekers = await IsSystemFlagEnabledAsync(SystemVariableCodes.PkiEnabled);

                var response = await _authService.LoginAsync(loginRequest);

                if (response == null)
                    return ApiResponseHelper.CreateErrorResponse(
                        "LoginFailed",
                        "Email Address or Password is Incorrect.",
                        StatusCodes.Status400BadRequest);

                return Ok(new
                {
                    Code = "Success",
                    Data = response
                });

            },
                ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("temp-login")]
        public async Task<IActionResult> TempLogin([FromBody] LoginRequestDto loginRequest)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var response = await _authService.TempLoginAsync(loginRequest);

                if (response == null)
                    return ApiResponseHelper.CreateErrorResponse(
                        "LoginFailed",
                        "Email Address or Password is Incorrect.",
                        StatusCodes.Status400BadRequest);

                return Ok(new
                {
                    Code = "Success",
                    Data = response
                });
            },
                ApiResponseHelper.CreateErrorResponse);
        }

        /// <summary>
        /// Internal staff sign-in (Admin / Operator / SME / Staff). Not linked from the public jobseeker login UI.
        /// When LOGIN_OTP_ENABLED = Y, returns EncryptedOtp for verify-login instead of JWT.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("staff-login")]
        public async Task<IActionResult> StaffLogin([FromBody] LoginRequestDto loginRequest)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                loginRequest ??= new LoginRequestDto();
                loginRequest.StaffOnly = true;
                loginRequest.DisallowPublicPasswordForJobSeekers = false;

                var otpEnabled = await IsLoginOtpEnabledAsync();
                if (otpEnabled)
                {
                    var encryptedOtp = await _authService.LoginAsyncWithOtp(loginRequest);
                    if (string.IsNullOrWhiteSpace(encryptedOtp))
                    {
                        return ApiResponseHelper.CreateErrorResponse(
                            "LoginFailed",
                            "Email Address or Password is Incorrect.",
                            StatusCodes.Status400BadRequest);
                    }

                    return Ok(new
                    {
                        Code = "Success",
                        Data = new
                        {
                            RequiresOtp = true,
                            EncryptedOtp = encryptedOtp
                        }
                    });
                }

                var response = await _authService.LoginAsync(loginRequest);
                if (response == null)
                {
                    return ApiResponseHelper.CreateErrorResponse(
                        "LoginFailed",
                        "Email Address or Password is Incorrect.",
                        StatusCodes.Status400BadRequest);
                }

                return Ok(new
                {
                    Code = "Success",
                    Data = response
                });
            },
                ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("LoginWithOtp")]
        public async Task<IActionResult> LoginWithOtp([FromBody] LoginRequestDto loginRequest)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
                {
                    loginRequest ??= new LoginRequestDto();
                    if (!loginRequest.StaffOnly)
                    {
                        loginRequest.DisallowPublicPasswordForJobSeekers =
                            await IsSystemFlagEnabledAsync(SystemVariableCodes.PkiEnabled);
                    }

                    var encryptedOtp = await _authService.LoginAsyncWithOtp(loginRequest);

                    if (string.IsNullOrWhiteSpace(encryptedOtp))
                        return ApiResponseHelper.CreateErrorResponse(
                            "LoginFailed",
                            "Email Address or Password is Incorrect.",
                            StatusCodes.Status400BadRequest);

                    return Ok(new
                    {
                        Code = "Success",
                        Data = new
                        {
                            RequiresOtp = true,
                            EncryptedOtp = encryptedOtp
                        }
                    });

                },
                ApiResponseHelper.CreateErrorResponse);
        }
        [HttpPost("verify-login")]
        public async Task<IActionResult> VerifyLogin([FromBody] LoginVerifyRequestDto loginRequest)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var response = await _authService.LoginVerifyAsync(loginRequest);

                if (response == null)
                    return ApiResponseHelper.CreateErrorResponse(
                        "LoginFailed",
                        "Invalid OTP.",
                        StatusCodes.Status400BadRequest);

                return Ok(new
                {
                    Code = "Success",
                    Data = response
                });

            },
                ApiResponseHelper.CreateErrorResponse);
        }

        private async Task<bool> IsSystemFlagEnabledAsync(string variableKey)
        {
            var raw = (await _adminSystemService.GetSystemVariableValueAsync(variableKey) ?? "N").Trim();
            return string.Equals(raw, "Y", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Staff password OTP requires both <c>Auth:LoginOtpEnabled</c> (appsettings) and
        /// system variable <c>LOGIN_OTP_ENABLED</c> = Y.
        /// </summary>
        private async Task<bool> IsLoginOtpEnabledAsync()
        {
            if (!IsAuthLoginOtpConfigEnabled())
            {
                return false;
            }

            return await IsSystemFlagEnabledAsync(SystemVariableCodes.LoginOtpEnabled);
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

        [HttpPost("send-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] SendVerificationRequest request)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
                {
                    var response = await _authService.SendVerificationCodeAsync(request);

                    if (response != null)
                    {


                        return Ok(new
                        {
                            Code = "Success",
                            Data = new
                            {
                                message = $"OTP sent successfully. Please check your Mobile.",
                                code = response

                            }
                        });
                    }

                    return ApiResponseHelper.CreateErrorResponse(
                        "SendVerificationCodeFailed",
                        "Email Address is Incorrect.",
                        StatusCodes.Status400BadRequest);
                },
                ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                LoginResponseDto response = await _authService.VerifyEmailAsync(email, token);
                if (response != null)
                {
                    var profileType = response.User?.ProfileType ?? 0;
                    if (profileType == (int)Enums.ProfileTypes.JobSeeker)
                    {
                        var jobseekerOrigin = FrontendOriginsConfiguration.GetJobseekersOrigin(_configuration);
                        if (!string.IsNullOrWhiteSpace(jobseekerOrigin))
                        {
                            return Redirect($"{jobseekerOrigin.TrimEnd('/')}/login");
                        }
                    }

                    var redirectOrigin = FrontendOriginsConfiguration.GetRedirectOrigin(_configuration);
                    if (response.User?.Profile != null && response.User.Profile.ProfileType == 4)
                        return Redirect($"{redirectOrigin}/en/Account/TRAVerificationPending");

                    return Redirect($"{redirectOrigin}/en/Account/EmailVerificationDone");
                }

                var failedUser = string.IsNullOrWhiteSpace(email)
                    ? null
                    : await _userRepository.GetUserByEmailAsync(email.Trim());
                if (failedUser?.ProfileType == (int)Enums.ProfileTypes.JobSeeker)
                {
                    var jobseekerOrigin = FrontendOriginsConfiguration.GetJobseekersOrigin(_configuration);
                    if (!string.IsNullOrWhiteSpace(jobseekerOrigin))
                    {
                        return Redirect($"{jobseekerOrigin.TrimEnd('/')}/login");
                    }
                }

                return Redirect($"{FrontendOriginsConfiguration.GetRedirectOrigin(_configuration)}/en/Account/EmailVerificationFailed");
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [Authorize]
        [HttpGet("accounts")]
        public async Task<IActionResult> GetAccounts()
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var rows = await _authService.GetProfileAccountsAsync(userId);
                return Ok(new { Code = "Success", Data = rows });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [Authorize]
        [HttpPost("switch-account")]
        public async Task<IActionResult> SwitchAccount([FromBody] SwitchAccountRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var response = await _authService.SwitchAccountAsync(userId, body?.UserId);
                return Ok(new { Code = "Success", Data = response });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [Authorize]
        [HttpGet("permissions")]
        public async Task<IActionResult> GetPermissions()
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var data = await _identityAccessAdminService.GetCurrentPermissionsAsync(userId);
                return Ok(new { Code = "Success", Data = data });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
                {
                    var response = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    var result = await _authService.UserProfileAsync(response);
                    if (result != null)
                    {
                        return Ok(new
                        {
                            Code = "Success",
                            Data = result
                        });
                    }

                    return ApiResponseHelper.CreateErrorResponse(
                        "LoginFailed",
                        "Model is Empty",
                        StatusCodes.Status400BadRequest);
                },
                ApiResponseHelper.CreateErrorResponse);
        }

        [Authorize]
        [HttpGet("profiles/{profileId}")]
        public async Task<IActionResult> GetProfile(int profileId)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var result = await _authService.GetUserByProfileId(profileId);
                if (result != null)
                {
                    return Ok(new
                    {
                        Code = "Success",
                        Data = result
                    });
                }

                return ApiResponseHelper.CreateErrorResponse(
                    "LoginFailed",
                    "Model is Empty",
                    StatusCodes.Status400BadRequest);
            },
                ApiResponseHelper.CreateErrorResponse);
        }

        [Authorize]
        [HttpGet("me/summary")]
        public async Task<IActionResult> GetProfileSummary()
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var response = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var result = await _authService.GetUserProfileSummary(response);
                if (result != null)
                {
                    return Ok(new
                    {
                        Code = "Success",
                        Data = result
                    });
                }

                return ApiResponseHelper.CreateErrorResponse(
                    "LoginFailed",
                    "Model is Empty",
                    StatusCodes.Status400BadRequest);
            },
                ApiResponseHelper.CreateErrorResponse);
        }

        [Authorize]
        [HttpGet("profiles/{profileId}/summary")]
        public async Task<IActionResult> GetProfileSummaryById(int profileId)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {

                var result = await _authService.GetUserProfileSummaryByProfileId(profileId);
                if (result != null)
                {
                    return Ok(new
                    {
                        Code = "Success",
                        Data = result
                    });
                }

                return ApiResponseHelper.CreateErrorResponse(
                    "LoginFailed",
                    "Model is Empty",
                    StatusCodes.Status400BadRequest);
            },
                ApiResponseHelper.CreateErrorResponse);
        }


        // create method to refresh token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
                {

                    var response = await _authService.RefreshTokenAsync(request);

                    return Ok(new
                    {
                        Code = "Success",
                        Data = response
                    });


                },
                ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("loginByCr")]
        public async Task<IActionResult> loginByCr([FromForm] LoginByCrOrMobileRequestDto loginRequest)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var response = await _authService.LoginByEncryptedCrNumber(HttpUtility.UrlDecode(loginRequest.EncryptedCivilId));

                if (loginRequest == null || string.IsNullOrEmpty(loginRequest.EncryptedCivilId) || response == null)
                    return ApiResponseHelper.CreateErrorResponse(
                        "LoginFailed",
                        "Invalid Data",
                        StatusCodes.Status400BadRequest);

                return Ok(new
                {
                    Code = "Success",
                    Data = response
                });

            },
                ApiResponseHelper.CreateErrorResponse);
        }
        [HttpPost("loginByMobile")]
        public async Task<IActionResult> loginByMobile([FromForm] LoginByCrOrMobileRequestDto loginRequest)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var response = await _authService.LoginByEncryptedMobile(HttpUtility.UrlDecode(loginRequest.EncryptedMobile));

                if (loginRequest == null || string.IsNullOrEmpty(loginRequest.EncryptedMobile) || response == null)
                    return ApiResponseHelper.CreateErrorResponse(
                        "LoginFailed",
                        "Invalid Data",
                        StatusCodes.Status400BadRequest);

                return Ok(new
                {
                    Code = "Success",
                    Data = response
                });

            },
                ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("TempToken")]
        public async Task<IActionResult> PreRegistrationToken([FromBody] LoginByCrOrMobileRequestDto loginRequest)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var response = _authService.GeneratePreRegistrationJwtAccessToken(loginRequest);

                if (response == null)
                    return ApiResponseHelper.CreateErrorResponse(
                        "LoginFailed",
                        "Invalid Data",
                        StatusCodes.Status400BadRequest);

                return Ok(new LoginResponseDto()
                {
                    AccessToken = response.AccessToken
                });

            },
                ApiResponseHelper.CreateErrorResponse);
        }

    }
}
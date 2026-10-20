using Bidding_API.Helpers;
using Bidding_Common;
using Bidding_Common.Helpers;
using Bidding_Core.Helpers;
using Bidding_Core.Interfaces;
using Bidding_Core.Services.Interfaces;
using Bidding_DLL.Services.Interfaces;
using Bidding_Models;
using Bidding_Models.DatabaseModel;
using Bidding_Models.DTOs;
using Bidding_Models.Jobseekers;
using ConstructionHub_Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Bidding_Core.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        
        private readonly IConfiguration _configuration;
       
        private readonly ISMSEmailNotification _sMsEmailNotification;
        
        private readonly INotificationType _notificationTypeRepository;
        
        private readonly ITemplate _templateRepository;
        private readonly IProfileV2Repository _profileV2Repository;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly INafathJobSeekerService _nafathJobSeekerService;

        public SMSSettings SMSSettings { get; }
        public EmailSettings EmailSettings { get; }
        public AuthService(IUserRepository userRepository, IConfiguration configuration, ISMSEmailNotification sMsEmailNotification, 
            INotificationType notificationTypeRepository, ITemplate templateRepository,
            IProfileV2Repository profileV2Repository,
            EmailSettings EmailSettings, SMSSettings smsSettings,
            INotificationDispatchService notificationDispatchService,
            INafathJobSeekerService nafathJobSeekerService)
        {
            this.EmailSettings = EmailSettings;
            this.SMSSettings = smsSettings;
            _userRepository = userRepository;
            _configuration = configuration;
            _sMsEmailNotification = sMsEmailNotification;
            _notificationTypeRepository = notificationTypeRepository;
            _templateRepository = templateRepository;
            _profileV2Repository = profileV2Repository;
            _notificationDispatchService = notificationDispatchService;
            _nafathJobSeekerService = nafathJobSeekerService;
        }

        public async Task<LoginResponseDto> LoginVerifyAsync(LoginVerifyRequestDto loginRequest)
        {
            if (!OtpEncryptionHelper.CompareOtp(loginRequest.Otp, loginRequest.EncryptedOtp))
            {
                throw new SafeExecutionHelper.NotAcceptableException("OTP is not valid.");
            }
            var findUser = await _userRepository.GetPrimaryUserByProfileEmailAsync(loginRequest.Email);
            if (findUser == null)
                throw new SafeExecutionHelper.BadRequestException("Email Address or Password is Incorrect");

            EnforceStaffOnlyIfRequired(findUser, loginRequest?.StaffOnly == true);

            var refreshToken = GenerateRefreshToken();
            findUser.VerificationToken = refreshToken;
            await _userRepository.UpdateUserAsync(findUser);

            var findUserWithProfile = await _userRepository.GetUserByIdAsync(findUser.Id);
            findUser.ProfileType = findUserWithProfile.ProfileType; // Ensure ProfileId is set correctly
            return new LoginResponseDto
            {
                AccessToken = GenerateJwtAccessToken(findUser),
                RefreshToken = refreshToken,
            };
        }
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            var findUser = await _userRepository.GetPrimaryUserByProfileEmailAsync(loginRequest.Email);
            if (findUser == null)
                throw new SafeExecutionHelper.BadRequestException("Email Address or Password is Incorrect");


            if (!findUser.PhoneNumberConfirmed)
                throw new SafeExecutionHelper.BadRequestException("Mobile number is not verified");

            // TEMP: accept any password for local/dev login; restore VerifyHashedPassword before production.
            // if (!await _userRepository.VerifyHashedPassword(findUser, loginRequest.Password))
            //     throw new SafeExecutionHelper.BadRequestException("Email Address or Password is Incorrect");

            // Unverified email: still issue JWT. API gate + UI keep the user on verify-email until confirmed.
            if (!findUser.EmailConfirmed)
            {
                await SendVerificationEmailAsync(findUser);
            }

            if (!findUser.ProfileId.HasValue)
                throw new SafeExecutionHelper.BadRequestException("User profile is not linked.");

            var profile = _profileV2Repository.GetProfileById(findUser.ProfileId.Value);
            if (profile != null && profile.ProfileType == (int)Enums.ProfileTypes.Operator && findUser.IsActive == false)
                throw new SafeExecutionHelper.BadRequestException("Waiting for TRA verification");

            if (findUser.IsActive == false)
                throw new SafeExecutionHelper.BadRequestException("Email Address or Password is Incorrect");

            EnforceStaffOnlyIfRequired(findUser, loginRequest?.StaffOnly == true);
            EnforcePublicPasswordPolicy(findUser, loginRequest);

            var refreshToken = GenerateRefreshToken();
            findUser.VerificationToken = refreshToken;
            await _userRepository.UpdateUserAsync(findUser);

            var findUserWithProfile = await _userRepository.GetUserByIdAsync(findUser.Id);
            findUser.ProfileType = findUserWithProfile.ProfileType; // Ensure ProfileId is set correctly
            return new LoginResponseDto
            {
                AccessToken = GenerateJwtAccessToken(findUser),
                RefreshToken = refreshToken,
            };
        }

        public async Task<LoginResponseDto> TempLoginAsync(LoginRequestDto loginRequest)
        {
            var findUser = await _userRepository.GetUserByEmailAsync(loginRequest.Email);
            if (findUser == null)
                throw new SafeExecutionHelper.BadRequestException("Email Address or Password is Incorrect");

            if (!findUser.EmailConfirmed)
            {
                await SendVerificationEmailAsync(findUser);
                throw new SafeExecutionHelper.BadRequestException("Email is not verified. check your mailbox");
            }

            if (!await _userRepository.VerifyHashedPassword(findUser, loginRequest.Password))
                throw new SafeExecutionHelper.BadRequestException("Email Address or Password is Incorrect");

            if (!findUser.ProfileId.HasValue)
                throw new SafeExecutionHelper.BadRequestException("User profile is not linked.");

            var profile = _profileV2Repository.GetProfileById(findUser.ProfileId.Value);
            if (profile != null && profile.ProfileType == (int)Enums.ProfileTypes.Operator && findUser.IsActive == false)
                throw new SafeExecutionHelper.BadRequestException("Waiting for TRA verification");

            if (findUser.IsActive == false)
                throw new SafeExecutionHelper.BadRequestException("Email Address or Password is Incorrect");

            var refreshToken = GenerateRefreshToken();
            findUser.VerificationToken = refreshToken;
            await _userRepository.UpdateUserAsync(findUser);

            var findUserWithProfile = await _userRepository.GetUserByIdAsync(findUser.Id);
            if (findUserWithProfile != null)
                findUser.ProfileType = findUserWithProfile.ProfileType;

            return new LoginResponseDto
            {
                AccessToken = GenerateJwtAccessToken(findUser),
                RefreshToken = refreshToken,
            };
        }

        public async Task<string> LoginAsyncWithOtp(LoginRequestDto loginRequest)
        {
            var findUser = await _userRepository.GetPrimaryUserByProfileEmailAsync(loginRequest.Email);
            if (findUser == null)
                throw new SafeExecutionHelper.BadRequestException("Email Address or Password is Incorrect");

            
            //if (! findUser.PhoneNumberConfirmed)
            //    throw new SafeExecutionHelper.BadRequestException("Mobile number is not verified");

            // TEMP: accept any password for local/dev login; restore VerifyHashedPassword before production.
            // if (! await _userRepository.VerifyHashedPassword(findUser,loginRequest.Password)) 
            //     throw new SafeExecutionHelper.BadRequestException("Email Address or Password is Incorrect");

            if (!findUser.EmailConfirmed)
            {
                await SendVerificationEmailAsync(findUser);
            }

            if (!findUser.ProfileId.HasValue)
                throw new SafeExecutionHelper.BadRequestException("User profile is not linked.");

            var profile = _profileV2Repository.GetProfileById(findUser.ProfileId.Value);
            if (profile != null && profile.ProfileType == (int)Enums.ProfileTypes.Operator && findUser.IsActive == false)
                throw new SafeExecutionHelper.BadRequestException("Waiting for TRA verification");

            if (findUser.IsActive == false)
                throw new SafeExecutionHelper.BadRequestException("Email Address or Password is Incorrect");

            EnforceStaffOnlyIfRequired(findUser, loginRequest?.StaffOnly == true);
            EnforcePublicPasswordPolicy(findUser, loginRequest);

            if (profile == null || string.IsNullOrWhiteSpace(profile.PhoneNumber))
                throw new SafeExecutionHelper.BadRequestException("Mobile number is not available for OTP.");

            SendVerificationRequest twoFactorAuthenticationRequest = new SendVerificationRequest()
            {
                PhoneNumber = profile.PhoneNumber
            };

            string encryptedCode = (await SendVerificationCodeAsync(twoFactorAuthenticationRequest)) as string;
            return encryptedCode;

            // TO BE MOVED TO NEXT API

            //var refreshToken = GenerateRefreshToken();            
            //findUser.VerificationToken = refreshToken;
            //await _userRepository.UpdateUserAsync(findUser);

            //var findUserWithProfile = await _userRepository.GetUserByIdAsync(findUser.Id);
            //findUser.ProfileType = findUserWithProfile.ProfileType; // Ensure ProfileId is set correctly
            //return new LoginResponseDto
            //{
            //    AccessToken = GenerateJwtAccessToken(findUser),
            //    RefreshToken = refreshToken,
            //};
        }

        private static bool IsStaffLoginProfileType(int profileType)
        {
            return profileType == (int)Enums.ProfileTypes.Admin
                || profileType == (int)Enums.ProfileTypes.SME
                || profileType == (int)Enums.ProfileTypes.Operator
                || profileType == (int)Enums.ProfileTypes.OUser
                || profileType == (int)Enums.ProfileTypes.Staff;
        }

        private void EnforceStaffOnlyIfRequired(AppUser findUser, bool staffOnly)
        {
            if (!staffOnly)
            {
                return;
            }

            var profileType = findUser?.ProfileType ?? -1;
            if (findUser?.ProfileId != null)
            {
                var profile = _profileV2Repository.GetProfileById(findUser.ProfileId.Value);
                if (profile != null)
                {
                    profileType = profile.ProfileType;
                }
            }

            if (!IsStaffLoginProfileType(profileType))
            {
                throw new SafeExecutionHelper.BadRequestException("This account cannot use staff sign-in.");
            }
        }

        private void EnforcePublicPasswordPolicy(AppUser findUser, LoginRequestDto loginRequest)
        {
            if (loginRequest == null || loginRequest.StaffOnly || !loginRequest.DisallowPublicPasswordForJobSeekers)
            {
                return;
            }

            var profileType = findUser?.ProfileType ?? -1;
            if (findUser?.ProfileId != null)
            {
                var profile = _profileV2Repository.GetProfileById(findUser.ProfileId.Value);
                if (profile != null)
                {
                    profileType = profile.ProfileType;
                }
            }

            if (profileType == (int)Enums.ProfileTypes.JobSeeker
                || profileType == (int)Enums.ProfileTypes.FreeLancer)
            {
                throw new SafeExecutionHelper.BadRequestException("Use PKI to sign in.");
            }
        }

        public async Task<object> UserProfileAsync(string response)
        {
            var findUser = await _userRepository.GetUserByIdAsync(response);
            if (findUser == null)
                throw new SafeExecutionHelper.NotFoundException("User not found.");
            try
            {
               var services = await ResolveServicesOfferedAsync(findUser.Id, response);
               ProfileTypeDto vendorType = await _profileV2Repository.GetProfileTypeAsync(findUser.ProfileType);
               var appUser = await _userRepository.GetPrimaryUserByProfileEmailAsync(findUser.EmailAddress);
               if (appUser == null)
                   throw new SafeExecutionHelper.NotFoundException("User not found.");
            
               
                var userProfile = new ProfileUserResponseDto
                {
                    ProfileId = findUser.Id,
                    About = findUser.AboutUs,
                    Website = findUser.ComanyWebsiteAddress,
                    Address = findUser.CityId.HasValue
                        ? findUser.CityId.Value.ToString()
                        : findUser.AddressName,
                    MissionVision = findUser.MissionVision,
                    ProfileImage = findUser.Avatar,
                    IsActive = findUser.IsActive,
                    CrNumber = findUser.CrNumber,
                    CompanyName = findUser.Name,
                    PhoneNumber = findUser.PhoneNumber,
                    EmailAddress = findUser.EmailAddress,
                    PreferredLanguage = NormalizePreferredLanguage(findUser.PreferredLanguage),
                    ServicesOffered = services,
                    VendorType = vendorType,
                    IsEmailVerified = appUser.EmailConfirmed,
                    UserProfileCompleted = findUser.UserProfileCompleted,
                };
                return userProfile;

            }
            catch (Exception e)
            {
              throw new SafeExecutionHelper
                  .InternalServerErrorException($"Error fetching service offerings for this User. Details: {e.Message}");
            }
          
            
            

        }
        public async Task<ProfileUserResponseDto> GetUserByProfileId(int profileId)
        {
            var findUserProfile =  _profileV2Repository.GetProfileById(profileId);
            if (findUserProfile == null)
                throw new SafeExecutionHelper.NotFoundException("User not found.");
            try
            {
                var services = await ResolveServicesOfferedByProfileIdAsync(profileId);
                ProfileTypeDto vendorType = await _profileV2Repository.GetProfileTypeAsync(findUserProfile.ProfileType);
                var appUser = await _userRepository.GetPrimaryUserByProfileEmailAsync(findUserProfile.EmailAddress);
                if (appUser == null)
                    throw new SafeExecutionHelper.NotFoundException("User not found.");


                var userProfile = new ProfileUserResponseDto
                {
                    ProfileId = findUserProfile.Id,
                    About = findUserProfile.AboutUs,
                    Website = findUserProfile.ComanyWebsiteAddress,
                    Address = findUserProfile.CityId.HasValue
                        ? findUserProfile.CityId.Value.ToString()
                        : findUserProfile.AddressName,
                    MissionVision = findUserProfile.MissionVision,
                    ProfileImage = findUserProfile.Avatar,
                    IsActive = findUserProfile.IsActive,
                    CrNumber = findUserProfile.CrNumber,
                    CompanyName = findUserProfile.Name,
                    PhoneNumber = findUserProfile.PhoneNumber,
                    EmailAddress = findUserProfile.EmailAddress,
                    PreferredLanguage = NormalizePreferredLanguage(findUserProfile.PreferredLanguage),
                    ServicesOffered = services,
                    VendorType = vendorType,
                    IsEmailVerified = appUser.EmailConfirmed,
                    UserProfileCompleted = findUserProfile.UserProfileCompleted,
                    AttachmentFileName = findUserProfile.OperatorRegisterationAttachment
                };
                return userProfile;

            }
            catch (Exception e)
            {
                throw new SafeExecutionHelper
                    .InternalServerErrorException($"Error fetching service offerings for this User. Details: {e.Message}");
            }




        }
        public async Task<FreelancerProfileFromMolDto> GetUserProfileSummaryByProfileId(int profileId)
        {
            var userProfile = _profileV2Repository.GetProfileById(profileId);
            if (userProfile == null)
                throw new SafeExecutionHelper.NotFoundException("User not found.");

            try
            {
                var appUser = await _userRepository.GetPrimaryUserByProfileEmailAsync(userProfile.EmailAddress);
                if (appUser == null)
                    throw new SafeExecutionHelper.NotFoundException("User not found.");

                return await BuildProfileSummaryFromNafathAsync(userProfile);
            }
            catch (SafeExecutionHelper.BadRequestException)
            {
                throw;
            }
            catch (SafeExecutionHelper.NotFoundException)
            {
                throw;
            }
            catch (SafeExecutionHelper.NotAcceptableException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new SafeExecutionHelper
                    .InternalServerErrorException($"Error fetching profile summary for this User. Details: {e.Message}");
            }
        }
        public async Task<object> GetUserProfileSummary(string userId)
        {
            var userProfile = await _userRepository.GetUserByIdAsync(userId);
            if (userProfile == null)
                throw new SafeExecutionHelper.NotFoundException("User not found.");

            try
            {
                var appUser = await _userRepository.GetPrimaryUserByProfileEmailAsync(userProfile.EmailAddress);
                if (appUser == null)
                    throw new SafeExecutionHelper.NotFoundException("User not found.");

                return await BuildProfileSummaryFromNafathAsync(userProfile);
            }
            catch (SafeExecutionHelper.BadRequestException)
            {
                throw;
            }
            catch (SafeExecutionHelper.NotFoundException)
            {
                throw;
            }
            catch (SafeExecutionHelper.NotAcceptableException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new SafeExecutionHelper
                    .InternalServerErrorException($"Error fetching profile summary for this User. Details: {e.Message}");
            }
        }

        /// <summary>
        /// Populate sidebar / MOL summary from Nafath JobSeeker details using the profile civil ID (CrNumber).
        /// </summary>
        private async Task<FreelancerProfileFromMolDto> BuildProfileSummaryFromNafathAsync(Profile userProfile)
        {
            var civilId = (userProfile.CrNumber ?? string.Empty).Trim();
            NafathJobSeekerDetailsDto nafath = null;

            if (!string.IsNullOrWhiteSpace(civilId))
            {
                var fetch = await _nafathJobSeekerService.GetDetailsByCivilIdAsync(civilId);
                if (!fetch.Success)
                {
                    throw new SafeExecutionHelper.BadRequestException(
                        NafathJobSeekerErrorCodes.FetchFailed,
                        fetch.ErrorMessage ?? NafathJobSeekerErrorCodes.FetchFailedMessage);
                }

                // No MOL row (HasData=false): fall through with nafath=null, same as an empty civilId —
                // every field below already falls back to the stored userProfile.Mol_* value via `??`.
                // (Previously returned BuildEmptyMolSummary(userProfile) here, which skipped that
                // fallback entirely and discarded legitimately-stored Mol_* data. See root-cause
                // analysis: BuildProfileSummaryFromNafathAsync HasData=false bug.)
                if (fetch.HasData)
                {
                    nafath = fetch.Data;
                }
            }

            var dateOfBirth = NafathJobSeekerService.NormalizeField(nafath?.DateOfBirth) ?? userProfile.Mol_DateOfBirth;
            var fullName =
                NafathJobSeekerService.FormatFullNameEn(nafath)
                ?? NafathJobSeekerService.FormatFullNameAr(nafath)
                ?? userProfile.Name;
            var gender =
                NafathJobSeekerService.NormalizeField(nafath?.SexDescEng)
                ?? NafathJobSeekerService.NormalizeField(nafath?.Sex)
                ?? userProfile.Gender;
            var jobStatus =
                NafathJobSeekerService.NormalizeField(nafath?.JobStatusDesc)
                ?? NafathJobSeekerService.NormalizeField(nafath?.JobStatus)
                ?? userProfile.Mol_JobStatus;

            var summary = new FreelancerProfileFromMolDto
            {
                ProfileId = userProfile.Id,
                FullName = fullName,
                Gender = gender,
                Age = ComputeAge(dateOfBirth),
                DateOfBirth = NafathJobSeekerService.FormatDisplayDate(dateOfBirth),
                Governorate = NafathJobSeekerService.NormalizeField(nafath?.PerRegionDesc)
                              ?? NafathJobSeekerService.NormalizeField(nafath?.PerRegionCode)
                              ?? userProfile.Mol_Governorate,
                Wilayat = NafathJobSeekerService.NormalizeField(nafath?.PerWilayatDescArb)
                          ?? NafathJobSeekerService.NormalizeField(nafath?.PerWilayatCode)
                          ?? userProfile.Mol_Willaya,
                Address = NafathJobSeekerService.FormatAddress(nafath),
                MaritalStatus = NafathJobSeekerService.NormalizeField(nafath?.MaritialDescArb)
                                ?? NafathJobSeekerService.NormalizeField(nafath?.MaritialStatus)
                                ?? userProfile.Mol_MaritalStatus,
                HealthStatus = NafathJobSeekerService.NormalizeField(nafath?.HealthDescArb)
                               ?? NafathJobSeekerService.NormalizeField(nafath?.HealthStatus)
                               ?? userProfile.Mol_HealthStatus,
                AttachmentFileName = userProfile.OperatorRegisterationAttachment,
                SearcherStatus = jobStatus ?? userProfile.Mol_JobSearchStatus,
                JobSearchDuration = userProfile.Mol_JobSearchDuration,
                DateOfFirstRegistrationAsJobSeeker = NafathJobSeekerService.FormatDisplayDate(
                    NafathJobSeekerService.NormalizeField(nafath?.RegistrationDate) ?? userProfile.Mol_DateOfFirstRegistrationAsJobSeeker),
                DateOfLastRegistrationAsJobSeeker = NafathJobSeekerService.FormatDisplayDate(
                    NafathJobSeekerService.NormalizeField(nafath?.ActiveDate) ?? userProfile.Mol_DateOfLastRegistrationAsJobSeeker),
                WorkStatus = jobStatus,
                NameOfOrganizationWorked = NafathJobSeekerService.NormalizeField(nafath?.SponsorNo) ?? userProfile.SponsorName,
                ContractStartDate =
                    NafathJobSeekerService.NormalizeField(nafath?.CurrentEmpDate) ?? userProfile.Mol_CurrentEmploymentDate,
                ContractExpiryDate = NafathJobSeekerService.NormalizeField(nafath?.ExpActiveDate),
                JobSecurityAllowance = userProfile.Mol_Allowance,
            };

            await _nafathJobSeekerService.ApplyEducationToMolSummaryAsync(summary, nafath);
            return summary;
        }

        private static string ComputeAge(string dateOfBirth)
        {
            if (string.IsNullOrWhiteSpace(dateOfBirth) ||
                !DateTime.TryParse(dateOfBirth, out var dob))
            {
                return null;
            }

            var today = DateTime.UtcNow.Date;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age))
            {
                age--;
            }

            return age >= 0 ? age.ToString() : null;
        }
        public async Task<object> SendVerificationCodeAsync(SendVerificationRequest request)
        {
            string otp = GenerateOtp();
            string encryptedOtp = OtpEncryptionHelper.EncryptOtp(otp);
            string message = string.Format(GetOtpMessageTemplate(), otp);
            var destinationPhone = NotificationTestOverrides.ResolveMobile(request.PhoneNumber);
            var notification = new SMSEmailNotification
            {
                InsertedDateTime = DateTime.UtcNow,
                SendingDateTime = DateTime.UtcNow,
                NotificationType = NotificationTypesEnum.SendOTP,
                ContactNo = destinationPhone,
                Email = NotificationTestOverrides.ResolveEmail(request.Email),
                ChannelType = NotiifcationChannelEnum.SMS 
            };
            NewSmscapi smsApi = new NewSmscapi(SMSSettings, _configuration);
            bool isSent = smsApi.SendSms(
                notification,
                _configuration["SMSSettings:SMSUserName"].Decrypt(),
                _configuration["SMSSettings:SMSPassword"].Decrypt(),
                destinationPhone,
                message,
                _configuration["SMSSettings:SID"].Decrypt()
            );
            var param = new SMSEmailNotificationParam
            {
                ParamName = "OTPCode",
                ParamValue = otp,
                Order = 0
            };
            notification.IsSend = isSent;
            notification.SMSEmailNotificationParamList = new List<SMSEmailNotificationParam> { param };
            _sMsEmailNotification.AddRange(new List<SMSEmailNotification>() { notification });
            
            return encryptedOtp;
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
        public async Task<LoginResponseDto> VerifyEmailAsync(string email, string token)
        {
       
            var findUser = await _userRepository.GetPrimaryUserByProfileEmailAsync(email);
            if (findUser == null) return null;
            if (findUser.VerificationToken != token) return null;
           
            if(findUser.EmailConfirmed) return null;
            if(findUser.CreatedDate.AddHours(24) < DateTime.UtcNow) return null;
            var profile = _profileV2Repository.GetProfileById(findUser.ProfileId.Value); 
            if (profile != null && profile.ProfileType == (int)Enums.ProfileTypes.Operator)
            {
                findUser.IsActive = false;
                findUser.Profile = profile;
            }
            findUser.EmailConfirmed = true;
            var refreshToken = GenerateRefreshToken();
            findUser.VerificationToken = refreshToken;
            await _userRepository.UpdateUserAsync(findUser);
            var findUserWithProfile = await _userRepository.GetUserByIdAsync(findUser.Id);
            findUser.ProfileType = findUserWithProfile.ProfileType; // Ensure ProfileId is set correctly
            return new LoginResponseDto
            {
                AccessToken = GenerateJwtAccessToken(findUser),
                RefreshToken = refreshToken,
            };
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
          

            // save the new refresh token in DB
            // find the user by email
            var findUser = await _userRepository.GetPrimaryUserByProfileEmailAsync(request.Email);
            if (findUser == null) throw new SafeExecutionHelper.BadRequestException("User not found.");
            if (findUser.VerificationToken!= request.RefreshToken) throw new SafeExecutionHelper.BadRequestException("Invalid refresh token.");
            if (!findUser.IsActive) throw new SafeExecutionHelper.UnauthorizedException("User is not active.");
            
    
            //create a new refresh token
            var newRefreshToken = GenerateRefreshToken();
            findUser.VerificationToken = newRefreshToken;
            await _userRepository.UpdateUserAsync(findUser);

            // return the new refresh token
            return await Task.FromResult(new LoginResponseDto
            {
                AccessToken = GenerateJwtAccessToken(findUser),
                RefreshToken = newRefreshToken,
            });
        }


        private string GetOtpMessageTemplate()
        {


            Template smsTemplate = null;


            NotificationType type = _notificationTypeRepository.GetById(Convert.ToInt32(NotificationTypesEnum.SendOTP));
            if (type != null && type.NotificationTypeSettings != null)
            {

                if (type.NotificationTypeSettings.IsSendSMS)
                {
                    smsTemplate = _templateRepository.GetbyTypeandCarrieranduserType(Convert.ToInt32(NotificationTypesEnum.SendOTP), Convert.ToInt32(NotiifcationChannelEnum.SMS), false, true, true);
                }

            }
            return smsTemplate?.Text;


        }
        

        /// <summary>
        /// Must match JWT validation (see <c>Bidding_API.jwtConfigurations.JwtBearerTokenSecretKey.AccessKeyTokenSecret</c>).
        /// </summary>
        private string ResolveJwtSigningKey()
        {
            var secret =
                _configuration["JwtBearerTokenSettings:SecretKey"]
                ?? _configuration["JwtBearerTokenSettings:Key"];
            if (!string.IsNullOrWhiteSpace(secret))
                return secret;
            return "TrainingModuleTrainingModuleTrainingModuleTrainingModuleTrainingModuleTrainingModule";
        }

        private string GenerateJwtAccessToken(AppUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ResolveJwtSigningKey()));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var userClaimsList = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user?.Profile?.EmailAddress ?? user?.Email ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user?.Id.ToString() ?? string.Empty),
                // Parity with AccessTokenService — controllers read "UserId" for audit.
                new Claim("UserId", user?.Id?.ToString() ?? string.Empty),
                new Claim("ProfileId", !String.IsNullOrEmpty(user?.ProfileId.ToString()) ? user?.ProfileId.ToString() : "0"),
                new Claim(
                    "ParentProfileId",
                    user?.Profile?.ParentProfileId != null && user.Profile.ParentProfileId > 0
                        ? user.Profile.ParentProfileId.ToString()
                        : "0"),
                new Claim("OrganizationId", user?.OrganizationId?.ToString() ?? string.Empty),
                new Claim("UserGroupId", user?.UserGroupId?.ToString() ?? string.Empty),
                new Claim("UserGroup", user?.UserGroup?.Code ?? string.Empty),
                new Claim("UserType", user?.UserGroup?.UserType?.Code ?? string.Empty),
                // Runtime routing/guards use Profile.ProfileType only (legacy VendorType).
                new Claim("VendorType", ResolveVendorType(user).ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtBearerTokenSettings:Issuer"],
                audience: _configuration["JwtBearerTokenSettings:Audience"],
                claims: userClaimsList,
                expires: DateTime.UtcNow.AddDays(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public LoginResponseDto GeneratePreRegistrationJwtAccessToken(LoginByCrOrMobileRequestDto requestDto)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ResolveJwtSigningKey()));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var userClaimsList = new List<Claim>();

            if (!string.IsNullOrWhiteSpace(requestDto?.EncryptedMobile))
            {
                userClaimsList.Add(new Claim("Mobile", HttpUtility.UrlDecode(requestDto.EncryptedMobile).Decrypt() ?? string.Empty));
            }

            if (!string.IsNullOrWhiteSpace(requestDto?.EncryptedCivilId))
            {
                userClaimsList.Add(new Claim("CivilId", HttpUtility.UrlDecode(requestDto.EncryptedCivilId).Decrypt() ?? string.Empty));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtBearerTokenSettings:Issuer"],
                audience: _configuration["JwtBearerTokenSettings:Audience"],
                claims: userClaimsList,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );
            LoginResponseDto result = new LoginResponseDto()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token)
            };
            return result;
        }

        private string GenerateOtp(int length = 6)
        {
            var random = new Random();
            var otp = "";
            for (int i = 0; i < length; i++)
            {
                otp += random.Next(0, 10); // 0-9
            }
            return otp;

        }
        
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }

        public async Task<LoginResponseDto> LoginByEncryptedCrNumber(string ecryptedCrNumber)
        {
            var findUser = await _userRepository.GetUserByCrNumberAsync(ecryptedCrNumber.Decrypt());
            if (findUser == null)
                throw new SafeExecutionHelper.BadRequestException("User is not registered");

            if (!findUser.PhoneNumberConfirmed)
                throw new SafeExecutionHelper.BadRequestException("Mobile number is not verified");

            // Unverified email: still issue JWT (PKI / Angular handoff → verify-email).
            if (!findUser.EmailConfirmed)
            {
                await SendVerificationEmailAsync(findUser);
            }

            var profile = _profileV2Repository.GetProfileById(findUser.ProfileId.Value);
            if (profile != null && profile.ProfileType == (int)Enums.ProfileTypes.Operator && findUser.IsActive == false)
                throw new SafeExecutionHelper.BadRequestException("Waiting for TRA verification");

            if (findUser.IsActive == false)
                throw new SafeExecutionHelper.BadRequestException("User account is inactivated");

            var refreshToken = GenerateRefreshToken();
            findUser.VerificationToken = refreshToken;
            await _userRepository.UpdateUserAsync(findUser);

            var findUserWithProfile = await _userRepository.GetUserByIdAsync(findUser.Id);
            findUser.ProfileType = findUserWithProfile.ProfileType; // Ensure ProfileId is set correctly
            return new LoginResponseDto
            {
                AccessToken = GenerateJwtAccessToken(findUser),
                RefreshToken = refreshToken,
                User = findUser
            };
        }

        public async Task<LoginResponseDto> LoginByEncryptedMobile(string ecryptedMobile)
        {
            var findUser = await _userRepository.GetUserByMobileAsync(ecryptedMobile.Decrypt());
            if (findUser == null)
                throw new SafeExecutionHelper.BadRequestException("User is not registered");

            //if (!findUser.PhoneNumberConfirmed)
            //    throw new SafeExecutionHelper.BadRequestException("Mobile number is not verified");

            // Unverified email: still issue JWT (PKI / Angular handoff → verify-email).
            if (!findUser.EmailConfirmed)
            {
                await SendVerificationEmailAsync(findUser);
            }

            var profile = _profileV2Repository.GetProfileById(findUser.ProfileId.Value);
            if (profile != null && profile.ProfileType == (int)Enums.ProfileTypes.Operator && findUser.IsActive == false)
                throw new SafeExecutionHelper.BadRequestException("Waiting for TRA verification");

            if (findUser.IsActive == false)
                throw new SafeExecutionHelper.BadRequestException("User account is disabled");

            var refreshToken = GenerateRefreshToken();
            findUser.VerificationToken = refreshToken;
            await _userRepository.UpdateUserAsync(findUser);

            var findUserWithProfile = await _userRepository.GetUserByIdAsync(findUser.Id);
            findUser.ProfileType = findUserWithProfile.ProfileType; // Ensure ProfileId is set correctly
            return new LoginResponseDto
            {
                AccessToken = GenerateJwtAccessToken(findUser),
                RefreshToken = refreshToken,
            };
        }

        public async Task<List<IdentityAccessAccountRowDto>> GetProfileAccountsAsync(string callerUserId)
        {
            var current = await _userRepository.GetUserWithAccessByIdAsync(callerUserId);
            if (current == null || !current.ProfileId.HasValue)
            {
                throw new SafeExecutionHelper.UnauthorizedException("A valid user is required.");
            }

            var siblings = await _userRepository.GetUsersByProfileIdAsync(current.ProfileId.Value);
            return siblings.Select(u => new IdentityAccessAccountRowDto
            {
                Id = u.Id,
                UserGroupId = u.UserGroupId,
                UserGroupCode = u.UserGroup?.Code,
                UserGroupNameEn = u.UserGroup?.NameEn,
                UserGroupNameAr = u.UserGroup?.NameAr,
                UserTypeCode = u.UserGroup?.UserType?.Code,
                UserTypeNameEn = u.UserGroup?.UserType?.NameEn,
                UserTypeNameAr = u.UserGroup?.UserType?.NameAr,
                IsPrimary = u.IsPrimary,
                IsCurrent = string.Equals(u.Id, callerUserId, StringComparison.Ordinal)
            }).ToList();
        }

        public async Task<LoginResponseDto> SwitchAccountAsync(string callerUserId, string targetUserId)
        {
            if (string.IsNullOrWhiteSpace(targetUserId))
            {
                throw new SafeExecutionHelper.BadRequestException("User id is required.");
            }

            var current = await _userRepository.GetUserWithAccessByIdAsync(callerUserId);
            var target = await _userRepository.GetUserWithAccessByIdAsync(targetUserId);
            if (current == null || target == null)
            {
                throw new SafeExecutionHelper.NotFoundException("Account was not found.");
            }

            if (!current.ProfileId.HasValue
                || current.ProfileId != target.ProfileId
                || !target.IsActive)
            {
                throw new SafeExecutionHelper.ForbiddenException("You can only switch to another account on the same profile.");
            }

            // Runtime auth uses Profile.ProfileType (VendorType) only — do not remap from UserGroup.
            target.ProfileType = target.Profile?.ProfileType ?? target.ProfileType;

            var refreshToken = GenerateRefreshToken();
            target.VerificationToken = refreshToken;
            await _userRepository.UpdateUserAsync(target);

            return new LoginResponseDto
            {
                AccessToken = GenerateJwtAccessToken(target),
                RefreshToken = refreshToken
            };
        }

        /// <summary>
        /// Login/JWT VendorType is the legacy ProfileType on the profile.
        /// Identity Access UserGroup remains available for admin user-management only.
        /// </summary>
        private static int ResolveVendorType(AppUser user)
        {
            return IdentityAccessAdminRules.ResolveLoginVendorType(
                user?.Profile?.ProfileType,
                user?.ProfileType ?? 0);
        }

        private class JwtBearerTokenSettings
        {
            public string SecretKey { get; set; }
            public string Audience { get; set; }
            public string Issuer { get; set; }
            public int ExpiryTimeInMinute { get; set; }
            public int RefreshTokenExpiryTimeInMinute { get; set; }
        }
        private class JwtBearerTokenSecretKey
        {
            public static string AccessKeyTokenSecret = "TrainingModule-Local-AccessToken-SigningKey-ChangeMe";
            public static string RefreshKeyTokenSecret = "TrainingModule-Local-RefreshToken-SigningKey-ChangeMe";
        }
        public ClaimsPrincipal VerifyTempToken(VerifyTempTokenRequestDto requestDto)
        {
            var jwtSection = _configuration.GetSection("JwtBearerTokenSettings");
            var jwtBearerTokenSettings = jwtSection.Get<JwtBearerTokenSettings>();
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = jwtBearerTokenSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtBearerTokenSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ResolveJwtSigningKey())),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            ClaimsPrincipal principal = tokenHandler.ValidateToken(requestDto.Token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            Debug.WriteLine(principal);
            return principal;
        }

        private async Task<List<ServiceOfferingGeneralDto>> ResolveServicesOfferedAsync(int profileId, string userId)
        {
            return await _profileV2Repository.GetServiceOfferingsByUserIdAsync(userId);
        }

        private async Task<List<ServiceOfferingGeneralDto>> ResolveServicesOfferedByProfileIdAsync(int profileId)
        {
            return await _profileV2Repository.GetServiceOfferingsByProfileIdAsync(profileId);
        }


        private static string NormalizePreferredLanguage(string value)
        {
            return string.Equals((value ?? string.Empty).Trim(), "ar", StringComparison.OrdinalIgnoreCase)
                ? "ar"
                : "en";
        }

    }
    
}
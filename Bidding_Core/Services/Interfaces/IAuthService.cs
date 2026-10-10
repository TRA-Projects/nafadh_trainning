using Bidding_Models.DatabaseModel;
using Bidding_Models.DTOs;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Bidding_Core.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsyncWithOtp(LoginRequestDto loginRequest);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task<LoginResponseDto> TempLoginAsync(LoginRequestDto loginRequest);
        Task<LoginResponseDto> LoginVerifyAsync(LoginVerifyRequestDto loginRequest);
        Task<object> UserProfileAsync(string response);
        Task<ProfileUserResponseDto> GetUserByProfileId(int profileId);
        Task<object> SendVerificationCodeAsync(SendVerificationRequest request);
        Task<object> GetUserProfileSummary(string userId);
        Task<FreelancerProfileFromMolDto> GetUserProfileSummaryByProfileId(int profileId);
        Task<LoginResponseDto> VerifyEmailAsync(string email, string token);
        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<System.Collections.Generic.List<Bidding_Models.Jobseekers.IdentityAccessAccountRowDto>> GetProfileAccountsAsync(string callerUserId);
        Task<LoginResponseDto> SwitchAccountAsync(string callerUserId, string targetUserId);
        Task<LoginResponseDto> LoginByEncryptedCrNumber(string ecryptedCrNumber);
        Task<LoginResponseDto> LoginByEncryptedMobile(string ecryptedMobile);
        LoginResponseDto GeneratePreRegistrationJwtAccessToken(LoginByCrOrMobileRequestDto requestDto);
        ClaimsPrincipal VerifyTempToken(VerifyTempTokenRequestDto requestDto);

    }
}
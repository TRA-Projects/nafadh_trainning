using System.Threading;
using System.Threading.Tasks;
using Bidding_Models.DTOs;

namespace Bidding_Core.Services.Interfaces
{
    public interface IRegistrationUserService
    {
        Task<object> RegisterSmeAsync(RegisterSmeRequestDto registerSmeRequest);
        Task<object> RegisterSmeWithoutOtpAsync(RegisterSmeWithoutOtpRequestDto registerSmeRequest);
        Task<object> ValidateNewUserRegisteration(ValidateUserDto validateUserDto);
        Task<object> GetRiyadaCompanyData(string crNumber,CancellationToken cancellationToken);
        Task<object> IsValidCivilIdWithMol(string civilId, CancellationToken cancellationToken);
        Task<string> GetCivilIdByCrNumber(string crNumber);
        Task<string> GetCompanyNameByCrNumber(string crNumber);
        Task<object> RegisterFreelancerAsync(RegisterFreelancerRequestDto freelancerRequestDto);
        Task<object> RegisterFreelancerWithoutOtpAsync(RegisterFreelancerWithoutOtpRequestDto freelancerRequestDto);
        Task<object> RegisterOperatorsAsync(RegisterOperatorsRequestDto operatorsRequestDto);
        Task<object> RegisterOperatorsWithtoutOtpAsync(RegisterOperatorsWithoutOtpRequestDto operatorsRequestDto);
        Task<object> RegisterJobseekerAsync(RegisterJobseekerRequestDto jobseekerRequestDto);
        Task<object> ValidateJobseekerRegistrationAsync(RegisterJobseekerRequestDto jobseekerRequestDto);
        Task<object> VerifyJobseekerEmailAsync(string email);
        Task<object> RequestJobseekerRegistrationOtpAsync(JobseekerRegistrationOtpRequestDto request);
        bool IsValidRegistrationOtp(string otp, string otpConfirmed);
        bool IsValidTempTokenForFreelancer(string tempToken, string civilId, string mobile);
    }
}
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bidding_Core.Services.Interfaces;
using Bidding_Models;
using Bidding_Models.DatabaseModel;
using Bidding_Models.DTOs;
using Xunit;

namespace Bidding_Core.Tests
{
    public sealed class MergeCodeLoginRegisterGuardTests
    {
        [Fact]
        public void ProfileType_JobSeeker_StaysDistinctFromFreelancer()
        {
            Assert.Equal(8, (int)Enums.ProfileTypes.JobSeeker);
            Assert.NotEqual(Enums.ProfileTypes.FreeLancer, Enums.ProfileTypes.JobSeeker);
        }

        [Fact]
        public void LoginRequest_KeepsJobseekerPasswordFlags()
        {
            var request = new LoginRequestDto
            {
                StaffOnly = true,
                DisallowPublicPasswordForJobSeekers = true,
            };

            Assert.True(request.StaffOnly);
            Assert.True(request.DisallowPublicPasswordForJobSeekers);
        }

        [Fact]
        public void AuthSurface_KeepsJobseekerAccounts_AndFreelancerTempLogin()
        {
            var names = typeof(IAuthService).GetMethods().Select(m => m.Name).ToHashSet();
            Assert.Contains(nameof(IAuthService.TempLoginAsync), names);
            Assert.Contains(nameof(IAuthService.GetProfileAccountsAsync), names);
            Assert.Contains(nameof(IAuthService.SwitchAccountAsync), names);

            var profile = typeof(Profile);
            Assert.NotNull(profile.GetProperty(nameof(Profile.UserProfileCompleted)));
            Assert.NotNull(profile.GetProperty(nameof(Profile.SubscriptionList)));
            Assert.Equal(typeof(Task<LoginResponseDto>), typeof(IAuthService).GetMethod(nameof(IAuthService.TempLoginAsync)).ReturnType);
        }
    }
}

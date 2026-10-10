using System.Threading.Tasks;
using Bidding_Models.DatabaseModel;
using Microsoft.AspNetCore.Identity;

namespace Bidding_DLL.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser> GetUserByEmailAsync(string email);

        /// <summary>Resolves login by <see cref="Profile.EmailAddress"/>, then the primary <see cref="AppUser"/> for that profile.</summary>
        Task<AppUser> GetPrimaryUserByProfileEmailAsync(string email);

        Task<bool>  VerifyHashedPassword(AppUser user, string password);

        Task<object> CreateUserAsync(AppUser newUser);

        /// <summary>
        /// After create: set <see cref="AppUser.UserGroupId"/> / <see cref="AppUser.IsPrimary"/>
        /// and ensure the Identity Access template role for the legacy profile type.
        /// </summary>
        Task ApplyRegistrationIdentityAccessAsync(string userEmail, int profileType);
        
        Task<int?> CreateProfileAsync(Profile profile);
        
        Task<Profile> GetUserByIdAsync(string response);
        Task<AppUser> GetUserByProfileId(int profileId);

        Task<AppUser> GetUserWithAccessByIdAsync(string userId);

        Task<System.Collections.Generic.List<AppUser>> GetUsersByProfileIdAsync(int profileId);

        Task<IdentityResult> UpdateUserAsync(AppUser findUser);


        Task<object> GetUserProfileByPhoneNumberAsync(string phoneNumber);
        Task<object> GetUserProfileByCrNumberAsync(string crNumber);
        /// <summary>Tracked MOL-imported job seeker profile (no login yet) for the civil ID, or null.</summary>
        Task<Profile> GetUnregisteredJobSeekerProfileByCivilIdAsync(string civilId);
        Task<AppUser> GetUserByCrNumberAsync(string crNumber);
        Task<AppUser> GetUserByMobileAsync(string mobile);
        Task<string> GetUserIdByProfileId(int profileId);
    }
}
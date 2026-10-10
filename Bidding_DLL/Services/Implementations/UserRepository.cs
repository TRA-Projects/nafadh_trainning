using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bidding_Common;
using Bidding_DLL.Data_Access_Layer;
using Microsoft.EntityFrameworkCore;
using Bidding_DLL.Services.Interfaces;
using Bidding_Models;
using Bidding_Models.DatabaseModel;
using Bidding_Models.DatabaseModel.IdentityAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

namespace Bidding_DLL.Services.Implementations
{
    public class UserRepository : IUserRepository
    
    
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly CHDBContext _cHdbContext;
      

        public UserRepository(UserManager<AppUser> userManager,CHDBContext cHdbContext)
        {
            _userManager = userManager;
            _cHdbContext = cHdbContext;
          
        }
       
        public async Task<AppUser> GetUserByEmailAsync(string email)
        {
            if (_userManager != null) return await _userManager.FindByEmailAsync(email);
            return null;
        }

        public async Task<AppUser> GetPrimaryUserByProfileEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var trimmed = email.Trim();
            var profile = await _cHdbContext.Profiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IsRegistered && p.EmailAddress != null && p.EmailAddress.ToLower() == trimmed.ToLower());

            if (profile == null)
            {
                return null;
            }

            var user = await _cHdbContext.Users
                .Include(u => u.UserGroup)
                .ThenInclude(g => g.UserType)
                .Where(u => u.ProfileId == profile.Id)
                .OrderByDescending(u => u.IsPrimary)
                .ThenBy(u => u.CreatedDate)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                user.Profile = profile;
                user.ProfileType = profile.ProfileType;
            }

            return user;
        }

        public async Task<AppUser> GetUserWithAccessByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await _cHdbContext.Users
                .Include(u => u.Profile)
                .Include(u => u.UserGroup)
                    .ThenInclude(g => g.UserType)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<List<AppUser>> GetUsersByProfileIdAsync(int profileId)
        {
            return await _cHdbContext.Users
                .Include(u => u.UserGroup)
                    .ThenInclude(g => g.UserType)
                .Where(u => u.ProfileId == profileId && u.IsActive)
                .OrderByDescending(u => u.IsPrimary)
                .ThenBy(u => u.CreatedDate)
                .ToListAsync();
        }

        public  Task<bool> VerifyHashedPassword(AppUser user, string password)
        {
            var result =  _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return Task.FromResult(result == PasswordVerificationResult.Success);
        }

        public async Task<object> CreateUserAsync(AppUser newUser)
        {
            try
            {
                var createResult = await _userManager.CreateAsync(newUser, newUser.Password);
                if (!createResult.Succeeded)
                {
                    var detail = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    throw new Exception(string.IsNullOrWhiteSpace(detail)
                        ? "An error occurred while saving the user."
                        : detail);
                }

                var roleResult = await _userManager.AddToRoleAsync(newUser, newUser.RoleName);
                if (!roleResult.Succeeded)
                {
                    var detail = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                    throw new Exception(string.IsNullOrWhiteSpace(detail)
                        ? "An error occurred while assigning the user role."
                        : detail);
                }

                if (newUser.Profile != null && newUser.Profile.Id > 0)
                {
                    return newUser.Profile;
                }

                var result = await _cHdbContext.Profiles.FirstOrDefaultAsync(x => x.IsRegistered && newUser.Email == x.EmailAddress);
                return result;
            }
            catch (Exception ex)
            {
                // If you want to log the exception, do so here.
                // For now, just re-throw or wrap it if needed.
                throw new Exception("An error occurred while saving the user.", ex);
            }
        }

        public async Task ApplyRegistrationIdentityAccessAsync(string userEmail, int profileType)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return;
            }

            if (!IdentityAccessLegacyProfileMap.TryMap(
                    profileType,
                    out var typeCode,
                    out var groupCode,
                    out _,
                    out _))
            {
                return;
            }

            var user = await _userManager.FindByEmailAsync(userEmail.Trim());
            if (user == null)
            {
                return;
            }

            var group = await _cHdbContext.UserGroups
                .AsNoTracking()
                .Include(g => g.UserType)
                .FirstOrDefaultAsync(g =>
                    g.IsActive
                    && g.Code == groupCode
                    && g.UserType != null
                    && g.UserType.Code == typeCode);
            if (group == null)
            {
                return;
            }

            user.UserGroupId = group.Id;
            user.IsPrimary = true;
            await _userManager.UpdateAsync(user);

            var templateRoleName = IdentityAccessLegacyProfileMap.TemplateRoleName(typeCode, groupCode);
            if (!await _userManager.IsInRoleAsync(user, templateRoleName))
            {
                var roleExists = await _cHdbContext.Roles.AnyAsync(r => r.Name == templateRoleName);
                if (roleExists)
                {
                    await _userManager.AddToRoleAsync(user, templateRoleName);
                }
            }
        }

        public async Task<int?> CreateProfileAsync(Profile profile)
        {
            try
            {
                var createProfile = await _cHdbContext.Profiles.AddAsync(profile);
                await _cHdbContext.SaveChangesAsync();
                return createProfile.Entity.Id;
            }
            catch (Exception ex)
            {
                // Log the error (use ILogger)
                Console.WriteLine($"Error creating profile: {ex.Message}");
                throw;
            }
        }

        public async Task<Profile> GetUserByIdAsync(string response)
        {
            try
            {
                var existingUser = await _userManager.FindByIdAsync(response);
                var profile = await _cHdbContext.Profiles.FirstOrDefaultAsync(x => existingUser.ProfileId == x.Id);
                return profile;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
          
        }
        public async Task<AppUser> GetUserByProfileId(int profileId)
        {
            try
            {
                var existingUser = await _cHdbContext.Users
                    .Include(x=>x.Profile)
                    .Include(x => x.UserGroup)
                    .ThenInclude(g => g.UserType)
                    .Where(x=>x.ProfileId == profileId)
                    .OrderByDescending(x => x.IsPrimary)
                    .ThenBy(x => x.CreatedDate)
                    .FirstOrDefaultAsync();
                return existingUser;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        public async Task<IdentityResult> UpdateUserAsync(AppUser findUser)
        {
           
            var response =await _userManager.UpdateAsync(findUser);
            if (!response.Succeeded)
            {
                throw new Exception("Error updating user");
                
            }
            return response;
               
        }

        public async Task<object> GetUserProfileByPhoneNumberAsync(string phoneNumber)
        {
            var profile = await _cHdbContext.Profiles.FirstOrDefaultAsync(x => x.IsRegistered && phoneNumber == x.PhoneNumber);
            return profile;
        }

        public async Task<object> GetUserProfileByCrNumberAsync(string crNumber)
        {
            var profile = await _cHdbContext.Profiles.FirstOrDefaultAsync(x => x.IsRegistered && crNumber == x.CrNumber);
            return profile;
        }

        public Task<Profile> GetUnregisteredJobSeekerProfileByCivilIdAsync(string civilId)
        {
            var jobSeekerType = (int)Enums.ProfileTypes.JobSeeker;
            return _cHdbContext.Profiles
                .Include(x => x.Settings)
                .Where(x => !x.IsRegistered && x.ProfileType == jobSeekerType && x.CrNumber == civilId)
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<string> GetUserIdByProfileId(int profileId)
        {
            //get userId by profileId
            var user =await _cHdbContext.Users.FirstOrDefaultAsync(u => u.ProfileId == profileId);
           
            return user.Id;
        }

        public async Task<AppUser> GetUserByCrNumberAsync(string crNumber)
        {
            //get userId by profileId
            var user = await _cHdbContext.Users.Include(x => x.Profile)
                .ThenInclude(p => p.CompanyInfo)
                .ThenInclude(c => c.Signatories)
                .FirstOrDefaultAsync(u =>
                    (u.Profile != null && u.Profile.CrNumber == crNumber) ||
                    (u.Profile != null &&
                     u.Profile.CompanyInfo != null &&
                     u.Profile.CompanyInfo.Signatories != null &&
                     u.Profile.CompanyInfo.Signatories.Any(c => c.CivilId == crNumber)));
            return user;
        }

        public async Task<AppUser> GetUserByMobileAsync(string mobile)
        {
            //get userId by profileId
            var user = await _cHdbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == mobile);

            return user;
        }

    }
}
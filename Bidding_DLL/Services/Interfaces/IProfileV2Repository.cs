using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bidding_Models;
using Bidding_Models.DatabaseModel;
using Bidding_Models.DTOs;

namespace Bidding_DLL.Services.Interfaces
{
    public interface IProfileV2Repository
    {
        Task<List<Profile>> GetProfilesAsync();
        Task<ProfilePagedResultDto> GetProfilesPagedAsync(int pageNumber, int pageSize, string name = null, string phone = null, string email = null, string crNo = null, int? role = null, bool? isActive = null);
        Task<ProfileSocial> CreateSocialProfileAsync(ProfileSocial social);
        Task<ProfilePortfolio> CreateProfilePortfolioAsync(ProfilePortfolio request);
        Task<object> GetProfilePortfolioAsync(int profileId);
        Task<ProfileSocial> UpdateSocialProfileAsync(ProfileSocial social);
        Task<List<ProfileSocial>> GetSocialProfileAsync(int profileId);
        Task<object> UpdatePortfolioAsync(ProfilePortfolio portfolio);
        Task<object> DeletePortfolioAsync(ProfilePortfolio portfolio);
        Profile GetProfileById(int requestProfileId);
        /// <summary>Fresh read of <see cref="Profile.UserProfileCompleted"/> (no tracker). Null when profile missing.</summary>
        Task<bool?> IsUserProfileCompletedAsync(int profileId);
        Task<object> UpdateProfileAsync(Profile profile);
        Task DeleteProfile(int profileId);
        Task<List<Profile>> GetProfilesPendingTraVerification();
        Task<ProfileCertificate> CreateProfileCertificationAsync(ProfileCertificate certificate);
        Task<object> UpdateProfileCertificationAsync(ProfileCertificate certificate);
        Task<object> GetProfileCertificationAsync(int profileId);
        Task<object> DeleteProfileCertificationAsync(ProfileCertificate certificate);
        Task<object> CreateProfileEducationAsync(ProfileEducation education);
        Task<object> UpdateProfileEducationAsync(ProfileEducation education);
        Task<object> GetProfileEducationAsync(int profileId);
        Task<object> DeleteProfileEducationAsync(ProfileEducation education);
        Task<object> CreateProfileWorkExperienceAsync(ProfileWorkExperience workExperience);
        Task<object> UpdateProfileWorkExperienceAsync(ProfileWorkExperience workExperience);
        Task<object> GetProfileWorkExperienceAsync(int profileId);
        Task<object> DeleteProfileWorkExperienceAsync(ProfileWorkExperience workExperience);
        Task<object> CreateProfileSkillAsync(ProfileSkill skill);
        Task<object> UpdateProfileSkillAsync(ProfileSkill skill);
        Task<object> GetProfileSkillAsync(int profileId);
        Task<object> DeleteProfileSkillAsync(ProfileSkill skill);
        Task<ProfilePortfolio> GetPortfolioAsync(int profileId, string portfolioId);
        ProfileCertificate GetProfileCertificationById(int requestProfileId, string toString);
        ProfileEducation GetProfileEducationById(int profileId, string toString);
        ProfileWorkExperience GetProfileWorkExperienceById(int profileId, string toString);
        ProfileSkill GetProfileSkillById(int profileId, string toString);
        Task<List<ServiceOfferingGeneralDto>> GetServiceOfferingAsync();
        Task<object> AddServiceToOffringAsync(VendorCategories vendorCategories);
        Task<List<ServiceOfferingGeneralDto>> GetServiceOfferingsByUserIdAsync(string response);
        Task<List<ServiceOfferingGeneralDto>> GetServiceOfferingsByProfileIdAsync(int profileId);
        Task<VendorCategories> GetVenderOfferingByIdAsync(int id, string userAppId);
        Task<object> DeleteVenderOfferingAsync(VendorCategories venderOffering);
        Task<ProfileTypeDto> GetProfileTypeAsync(int findUserProfileType);
        Task<TendorCategory> GetServicesOfferedByIdAsync(int offering);

        Task<List<VendorCategories>> GetServicesOfferedByVendorIdAsync(string userAppId);
        Task DeleteServicesOfferedAsync(VendorCategories service);

        Task<object> GetCityAsync();
        Task<FreelancerBankAccount> AddBankAccount (FreelancerBankAccount bankAccount);
        Task<FreelancerBankAccount> UpdateBankAccount (FreelancerBankAccount bankAccount);
        Task<FreelancerBankAccount> GetBankAccountByProfileId (int profileId);
        Task DeleteBankAccount(int profileId);


        Task<List<ProfileVideo>> GetVideosByProfileId(int profileId);
        Task<ProfileVideo> AddProfileVideo(ProfileVideo pVideo);
        Task<bool> DeleteProfileVideo(int id);
        Task<Profile> ActivateProfile(int profileId);
    }
}
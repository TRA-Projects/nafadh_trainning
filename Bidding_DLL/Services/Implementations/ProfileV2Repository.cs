using Bidding_DLL.Services.Interfaces;
using Bidding_Models;
using Bidding_Models.DatabaseModel;
using Bidding_Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;



namespace Bidding_DLL.Services.Implementations
{
    public class ProfileV2Repository : IProfileV2Repository

    {

        private readonly CHDBContext _cHdbContext;

        public ProfileV2Repository(CHDBContext cHdbContext)
        {
            _cHdbContext = cHdbContext;
        }

        public async Task<List<Profile>> GetProfilesAsync()
        {
            try
            {
                var profiles = await _cHdbContext.Profiles.Select( d => new Profile
                {
                    Id = d.Id,
                    Name = d.Name??"",
                    NameAr = d.NameAr??"",
                    ProfileType = d.ProfileType,                                        
                    IsActive = d.IsActive,                   
                    CrNumber = d.CrNumber??"",
                    EmailAddress = d.EmailAddress??"",
                    PhoneNumber=d.PhoneNumber??"",                    
                    ProfileStatus = d.ProfileStatus
                }).ToListAsync();

                return profiles;
            }
            catch (Exception e)
            {
                throw new Exception($"Error getting profiles: {e.Message}");
            }
        }

        public async Task<ProfileSocial> CreateSocialProfileAsync(ProfileSocial social)
        {
            try
            {
                _cHdbContext.ProfileSocials.Add(social);
                await _cHdbContext.SaveChangesAsync();
                return social;

            }
            catch (Exception e)
            {
                throw new Exception($"Error creating social profile: {e.Message}");
            }


        }

        public async Task<ProfilePortfolio> CreateProfilePortfolioAsync(ProfilePortfolio profilePortfolio)
        {
            try
            {
                profilePortfolio.Slno = _cHdbContext.ProfilePortfolios.Count(d => d.ProfileId == profilePortfolio.ProfileId) + 1;
                _cHdbContext.ProfilePortfolios.Add(profilePortfolio);
                await _cHdbContext.SaveChangesAsync();
                return profilePortfolio;
            }
            catch (Exception e)
            {
                throw new Exception($"Error creating portfolio: {e.Message}");
            }

        }



        public Task<object> GetProfilePortfolioAsync(int profileId)
        {
            try
            {
                var portfolioList = _cHdbContext.ProfilePortfolios.Where(d => d.ProfileId == profileId).ToList()
                    .Select((p, index) => new ProfilePortfolio
                    {
                        ProfileId = p.ProfileId,
                        Profile = null,
                        PortfolioTitle = p.PortfolioTitle,
                        PortfolioDescription = p.PortfolioDescription,
                        PortfolioType = p.PortfolioType,
                        Cost = p.Cost,
                        Participation = p.Participation,
                        Sector = p.Sector,
                        Duration = p.Duration,
                        Id = p.Id,
                        Slno = index + 1,
                        PortfolioLink = p.PortfolioLink,
                    })
                    .ToList();
                return Task.FromResult<object>(portfolioList);
            }
            catch (Exception e)
            {
                throw new Exception($"Error getting portfolio: {e.Message}");
            }

        }

        public async Task<ProfileSocial> UpdateSocialProfileAsync(ProfileSocial social)
        {
            try
            {
                _cHdbContext.ProfileSocials.Update(social);
                await _cHdbContext.SaveChangesAsync();
                return social;

            }
            catch (Exception e)
            {
                throw new Exception($"Error updating social profile: {e.Message}");
            }

        }

        public async Task<List<ProfileSocial>> GetSocialProfileAsync(int profileId)
        {
            try
            {
                var socialList = _cHdbContext.ProfileSocials
                    .Where(d => d.ProfileId == profileId)
                    .Select(p => new ProfileSocial
                    {
                        ProfileId = p.ProfileId,
                        Profile = null,
                        Id = p.Id,
                        SocialProfileName = p.SocialProfileName,
                        SocialProfileType = p.SocialProfileType,
                        SocialProfileUrl = p.SocialProfileUrl,
                    })
                    .ToList();

                return socialList;
            }
            catch (Exception e)
            {
                throw new Exception($"Error getting social profiles for profile with id {profileId}. Details: {e.Message}");
            }

        }

        public async Task<object> UpdatePortfolioAsync(ProfilePortfolio portfolio)
        {
            try
            {
                _cHdbContext.ProfilePortfolios.Update(portfolio);
                await _cHdbContext.SaveChangesAsync();
                var response = new { Updated = true, Message = $"Portfolio Id: {portfolio.Id} has been updated successfully" };
                return response;
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating portfolio with id {portfolio.Id}. Details: {e.Message}");
            }

        }

        public async Task<object> DeletePortfolioAsync(ProfilePortfolio portfolio)
        {
            try
            {
                _cHdbContext.ProfilePortfolios.Remove(portfolio);
                await _cHdbContext.SaveChangesAsync();
                var response = new { Deleted = true, Message = $"Portfolio Id: {portfolio.Id} has been deleted successfully" };
                return response;

            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting portfolio with id {portfolio.Id}. Details: {e.Message}");
            }

        }



        public async Task<ProfilePagedResultDto> GetProfilesPagedAsync(int pageNumber, int pageSize, string name = null, string phone = null, string email = null, string crNo = null, int? role = null, bool? isActive = null)
        {
            try
            {
                var query = _cHdbContext.Profiles.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(d => (d.Name ?? "").Contains(name));
                }

                if (!string.IsNullOrWhiteSpace(phone))
                {
                    query = query.Where(d => (d.PhoneNumber ?? "").Contains(phone));
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    query = query.Where(d => (d.EmailAddress ?? "").Contains(email));
                }

                if (!string.IsNullOrWhiteSpace(crNo))
                {
                    query = query.Where(d => (d.CrNumber ?? "").Contains(crNo));
                }

                if (role.HasValue)
                {
                    query = query.Where(d => d.ProfileType == role.Value);
                }

                if (isActive.HasValue)
                {
                    query = query.Where(d => d.IsActive == isActive.Value);
                }

                var totalCount = await query.CountAsync();

                var profiles = await query
                    .OrderBy(d => d.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(d => new Profile
                    {
                        Id = d.Id,
                        Name = d.Name ?? "",
                        NameAr = d.NameAr ?? "",
                        ProfileType = d.ProfileType,
                        IsActive = d.IsActive,
                        CrNumber = d.CrNumber ?? "",
                        EmailAddress = d.EmailAddress ?? "",
                        PhoneNumber = d.PhoneNumber ?? "",
                        ProfileStatus = d.ProfileStatus
                    })
                    .ToListAsync();

                return new ProfilePagedResultDto
                {
                    Items = profiles,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Error getting paged profiles: {e.Message}");
            }
        }

        public Profile GetProfileById(int requestProfileId)
        {
            try
            {
                return _cHdbContext.Profiles.FirstOrDefault(d => d.Id == requestProfileId);
            }
            catch (Exception e)
            {
                throw new Exception($"Error getting profile with id {requestProfileId}. Details: {e.Message}");
            }

        }

        public async Task<bool?> IsUserProfileCompletedAsync(int profileId)
        {
            if (profileId <= 0)
            {
                return null;
            }

            return await _cHdbContext.Profiles.AsNoTracking()
                .Where(p => p.Id == profileId)
                .Select(p => (bool?)p.UserProfileCompleted)
                .FirstOrDefaultAsync();
        }

        public async Task<object> UpdateProfileAsync(Profile profile)
        {
            try
            {
                _cHdbContext.Profiles.Update(profile);
                await _cHdbContext.SaveChangesAsync();
                var response = new { Updated = true, Message = $"Profile Id: {profile.Id} has been updated successfully" };
                return response;

            }
            catch (Exception e)
            {
                throw new Exception($"Error updating profile with id {profile.Id}. Details: {e.Message}");
            }

        }

        public async Task<ProfileCertificate> CreateProfileCertificationAsync(ProfileCertificate certificate)
        {
            try
            {
                certificate.Slno = _cHdbContext.ProfileCertificates.Count(d => d.ProfileId == certificate.ProfileId) + 1;
                _cHdbContext.ProfileCertificates.Add(certificate);
                await _cHdbContext.SaveChangesAsync();
                return certificate;
            }
            catch (Exception e)
            {
                throw new Exception($"Error creating profile certification: {e.Message}");
            }

        }

        public async Task<object> UpdateProfileCertificationAsync(ProfileCertificate certificate)
        {
            try
            {
                _cHdbContext.ProfileCertificates.Update(certificate);
                await _cHdbContext.SaveChangesAsync();
                var response = new { Updated = true, Message = $"Profile Certificate Id: {certificate.Id} has been updated successfully" };
                return response;

            }
            catch (Exception e)
            {
                throw new Exception($"Error updating profile certification with id {certificate.Id}. Details: {e.Message}");
            }
        }

        public Task<object> GetProfileCertificationAsync(int profileId)
        {
            try
            {
                var certificateList = _cHdbContext.ProfileCertificates.Where(d => d.ProfileId == profileId).ToList()
                    .Select((c, index) => new ProfileCertificate
                    {
                        Id = c.Id,
                        Slno = index + 1,
                        CertificateName = c.CertificateName,
                        CertificateUrl = c.CertificateUrl,
                        ProfileId = c.ProfileId,
                        Provider = c.Provider,
                        CertificateId = c.CertificateId,
                        Profile = null,
                        DateExpired = c.DateExpired?.ToLocalTime(),
                        DateIssued = c.DateIssued.ToLocalTime(),
                        FileName = c.FileName
                    })
                    .ToList();
                return Task.FromResult<object>(certificateList);
            }
            catch (Exception e)
            {
                throw new Exception($"Error getting profile certifications for profile with id {profileId}. Details: {e.Message}");
            }

        }

        public async Task<object> DeleteProfileCertificationAsync(ProfileCertificate certificate)
        {
            try
            {
                _cHdbContext.ProfileCertificates.Remove(certificate);
                await _cHdbContext.SaveChangesAsync();
                var response = new { Deleted = true, Message = $"Profile Certificate Id: {certificate.Id} has been deleted successfully" };
                return Task.FromResult<object>(response);

            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting profile certification with id {certificate.Id}. Details: {e.Message}");
            }

        }

        public async Task<object> CreateProfileEducationAsync(ProfileEducation education)
        {
            try
            {
                education.Slno = _cHdbContext.ProfileEducations.Count(d => d.ProfileId == education.ProfileId) + 1;
                _cHdbContext.ProfileEducations.Add(education);
                await _cHdbContext.SaveChangesAsync();
                return education;

            }
            catch (Exception e)
            {
                throw new Exception($"Error creating profile education: {e.Message}");
            }

        }

        public async Task<object> UpdateProfileEducationAsync(ProfileEducation education)
        {

            try
            {
                _cHdbContext.ProfileEducations.Update(education);
                await _cHdbContext.SaveChangesAsync();
                var response = new { Updated = true, Message = $"Education Id: {education.Id} has been updated successfully" };
                return response;

            }
            catch (Exception e)
            {

                throw new Exception($"Error updating profile education with id {education.Id}. Details: {e.Message}");
            }

        }



        public Task<object> GetProfileEducationAsync(int profileId)
        {
            try
            {
                var educationList = _cHdbContext.ProfileEducations.Where(d => d.ProfileId == profileId).ToList()
                    .Select((e, index) => new ProfileEducation
                    {
                        Id = e.Id,
                        Slno = index + 1,
                        Degree = e.Degree,
                        ProfileId = e.ProfileId,
                        Profile = null,
                        YearOfPassing = e.YearOfPassing,
                        Country = e.Country,
                        City = e.City,
                        InstituteName = e.InstituteName,
                        MajorSpecialization = e.MajorSpecialization,
                        Grade = e.Grade,
                        FileName = e.FileName
                    })
                    .ToList();
                return Task.FromResult<object>(educationList);
            }
            catch (Exception e)
            {
                throw new Exception($"Error getting profile educations for profile with id {profileId}. Details: {e.Message}");
            }

        }

        public async Task<object> DeleteProfileEducationAsync(ProfileEducation education)
        {
            try
            {
                _cHdbContext.ProfileEducations.Remove(education);
                await _cHdbContext.SaveChangesAsync();
                var response = new { Deleted = true, Message = $"Education Id: {education.Id} has been deleted successfully" };
                return response;

            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting profile education with id {education.Id}. Details: {e.Message}");
            }

        }

        public async Task<object> CreateProfileWorkExperienceAsync(ProfileWorkExperience workExperience)
        {
            try
            {
                workExperience.Slno = _cHdbContext.ProfileWorkExperiences.Count(d => d.ProfileId == workExperience.ProfileId) + 1;
                _cHdbContext.ProfileWorkExperiences.Add(workExperience);
                await _cHdbContext.SaveChangesAsync();
                return workExperience;
            }
            catch (Exception e)
            {
                throw new Exception($"Error creating profile work experience: {e.Message}");
            }
        }

        public async Task<object> UpdateProfileWorkExperienceAsync(ProfileWorkExperience workExperience)
        {
            try
            {
                _cHdbContext.ProfileWorkExperiences.Update(workExperience);
                await _cHdbContext.SaveChangesAsync();
                var response = new { Updated = true, Message = $"Work Experience Id: {workExperience.Id} has been updated successfully" };
                return response;

            }
            catch (Exception e)
            {
                throw new Exception($"Error updating profile work experience with id {workExperience.Id}. Details: {e.Message}");
            }
        }

        public Task<object> GetProfileWorkExperienceAsync(int profileId)
        {
            try
            {
                var workExperienceList = _cHdbContext.ProfileWorkExperiences
                    .Where(d => d.ProfileId == profileId).ToList()
                    .Select((w, index) => new ProfileWorkExperience
                    {
                        Id = w.Id,
                        Slno = index + 1,
                        ProfileId = w.ProfileId,
                        Profile = null,
                        StartDate = w.StartDate,
                        EndDate = w.EndDate,
                        IsCurrent = w.IsCurrent,
                        EmploymentType = w.EmploymentType,
                        DesignationTitle = w.DesignationTitle,
                        OrganizationName = w.OrganizationName,
                        Location = w.Location,
                        ModeOfWork = w.ModeOfWork,
                    })
                    .ToList();
                return Task.FromResult<object>(workExperienceList);
            }
            catch (Exception e)
            {
                throw new Exception($"Error getting profile work experiences for profile with id {profileId}. Details: {e.Message}");
            }

        }

        public async Task<object> DeleteProfileWorkExperienceAsync(ProfileWorkExperience workExperience)
        {
            try
            {
                _cHdbContext.ProfileWorkExperiences.Remove(workExperience);
                await _cHdbContext.SaveChangesAsync();
                var response = new { Deleted = true, Message = $"Work Experience Id: {workExperience.Id} has been deleted successfully" };
                return response;

            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting profile work experience with id {workExperience.Id}. Details: {e.Message}");
            }

        }

        public async Task<object> CreateProfileSkillAsync(ProfileSkill skill)
        {
            try
            {
                skill.Slno = _cHdbContext.ProfileSkills.Count(d => d.ProfileId == skill.ProfileId) + 1;
                _cHdbContext.ProfileSkills.Add(skill);
                await _cHdbContext.SaveChangesAsync();
                return skill;
            }
            catch (Exception e)
            {
                throw new Exception($"Error creating profile skill: {e.Message}");
            }

        }

        public async Task<object> UpdateProfileSkillAsync(ProfileSkill skill)
        {
            try
            {
                _cHdbContext.ProfileSkills.Update(skill);
                await _cHdbContext.SaveChangesAsync();
                var response = new { Updated = true, Message = $"Skill Id: {skill.Id} has been updated successfully" };
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating profile skill with id {skill.Id}. Details: {e.Message}");
            }


        }

        public Task<object> GetProfileSkillAsync(int profileId)
        {
            try
            {
                var skillList = _cHdbContext.ProfileSkills
                    .Where(d => d.ProfileId == profileId).ToList()
                    .Select((s, index) => new ProfileSkill
                    {
                        ProfileId = s.ProfileId,
                        SkillName = s.SkillName,
                        SkillLevel = s.SkillLevel,
                        Profile = null,
                        Id = s.Id,
                        Slno = index + 1
                    })
                    .ToList();
                return Task.FromResult<object>(skillList);
            }
            catch (Exception e)
            {
                throw new Exception($"Error getting profile skills for profile with id {profileId}. Details: {e.Message}");
            }

        }

        public async Task<object> DeleteProfileSkillAsync(ProfileSkill skill)
        {
            try
            {
                _cHdbContext.ProfileSkills.Remove(skill);
                await _cHdbContext.SaveChangesAsync();
                var response = new { Deleted = true, Message = $"Skill Id: {skill.Id} has been deleted successfully" };
                return Task.FromResult(response);

            }
            catch (Exception e)
            {

                throw new Exception($"Error deleting profile skill with id {skill.Id}. Details: {e.Message}");
            }

        }

        public Task<ProfilePortfolio> GetPortfolioAsync(int profileId, string portfolioId)
        {
            var portfolio = _cHdbContext.ProfilePortfolios.FirstOrDefault(d => d.ProfileId == profileId && d.Id.ToString() == portfolioId);
            return Task.FromResult(portfolio);
        }

        public ProfileCertificate GetProfileCertificationById(int requestProfileId, string toString)
        {
            try
            {
                return _cHdbContext.ProfileCertificates.FirstOrDefault(d => d.ProfileId == requestProfileId && d.Id.ToString() == toString);
            }
            catch (Exception e)
            {
                throw new Exception($"Error getting profile certification by id: {e.Message}");
            }
        }

        public ProfileEducation GetProfileEducationById(int profileId, string toString)
        {
            var result = _cHdbContext.ProfileEducations.FirstOrDefault(d => d.ProfileId == profileId && d.Id.ToString() == toString);
            return result;
        }

        public ProfileWorkExperience GetProfileWorkExperienceById(int profileId, string toString)
        {
            var result = _cHdbContext.ProfileWorkExperiences.FirstOrDefault(d => d.ProfileId == profileId && d.Id.ToString() == toString);
            return result;
        }

        public ProfileSkill GetProfileSkillById(int profileId, string toString)
        {
            var result = _cHdbContext.ProfileSkills.FirstOrDefault(d => d.ProfileId == profileId && d.Id.ToString() == toString);
            return result;
        }

        public Task<List<ServiceOfferingGeneralDto>> GetServiceOfferingAsync()
        {
            var services = _cHdbContext.TendorCategories
                .Select(s => new ServiceOfferingGeneralDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    ArabicName = s.ArabicName,
                })
                .ToList();

            return Task.FromResult(services);
        }

        public async Task<object> AddServiceToOffringAsync(VendorCategories vendorCategories)
        {
            try
            {
                _cHdbContext.VendorCategories.Add(vendorCategories);
                await _cHdbContext.SaveChangesAsync();
                return vendorCategories;
            }
            catch (Exception e)
            {
                throw new Exception($"Error adding service to vendor categories: {e.Message}");
            }

        }

        public Task<List<ServiceOfferingGeneralDto>> GetServiceOfferingsByUserIdAsync(string response)
        {
            try
            {
                var venderServices = _cHdbContext.VendorCategories
                    .Where(d => d.VendorId == response)
                    .ToList();
                //get service offerings
                var tenderCategories = _cHdbContext.TendorCategories
                    .Where(d => venderServices.Select(s => s.TendorCategoryId).Contains(d.Id))
                    .Select(s => new ServiceOfferingGeneralDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        ArabicName = s.ArabicName,
                    })
                    .ToList();
                return Task.FromResult(tenderCategories);

            }
            catch (Exception e)
            {
                throw new Exception($"Error getting service offerings by user id: {e.Message}");
            }
        }
        public async Task<List<ServiceOfferingGeneralDto>> GetServiceOfferingsByProfileIdAsync(int profileId)
        {
            try
            {
                var user = await _cHdbContext.Users.FirstOrDefaultAsync(i => i.ProfileId == profileId);
                if (user == null)
                    throw new Exception("User not found");
                var venderServices = _cHdbContext.VendorCategories
                    .Where(d => d.VendorId == user.Id)
                    .ToList();
                //get service offerings
                var tenderCategories = _cHdbContext.TendorCategories
                    .Where(d => venderServices.Select(s => s.TendorCategoryId).Contains(d.Id))
                    .Select(s => new ServiceOfferingGeneralDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        ArabicName = s.ArabicName,
                    })
                    .ToList();
                return tenderCategories;

            }
            catch (Exception e)
            {
                throw new Exception($"Error getting service offerings by user id: {e.Message}");
            }
        }
        public async Task<VendorCategories> GetVenderOfferingByIdAsync(int id, string userAppId)
        {
            try
            {
                var venderOffering = _cHdbContext.VendorCategories
                    .FirstOrDefault(d => d.TendorCategoryId == id && d.VendorId == userAppId);
                return await Task.FromResult(venderOffering);
            }
            catch (Exception e)
            {
                throw new Exception($"Error getting vender offering by id: {e.Message}");
            }

        }

        public async Task<object> DeleteVenderOfferingAsync(VendorCategories venderOffering)
        {
            try
            {
                _cHdbContext.VendorCategories.Remove(venderOffering);
                await _cHdbContext.SaveChangesAsync();
                return new { Deleted = true, Message = $"Vendor Offering  has been deleted successfully" };
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting vender offering: {e.Message}");
            }


        }

        public async Task<ProfileTypeDto> GetProfileTypeAsync(int findUserProfileType)
        {
            var profileTypes = new[]
            {
                new { Id = 0, Name = "Admin", ArabicName = "مسؤو النظام" },
                new { Id = 1, Name = "FreeLancer", ArabicName = "مستقل" },
                new { Id = 2, Name = "SME", ArabicName = "الشركات الصغيرة والمتوسطة" },
                new { Id = 3, Name = "Operator", ArabicName = "المشغلين" },
                new { Id = 4, Name = "Operator", ArabicName = "المشغلين" },
                new { Id = 8, Name = "JobSeeker", ArabicName = "باحث عن عمل" },
            };
            var result = profileTypes.FirstOrDefault(d => d.Id == findUserProfileType);
            if (result != null)
                return await Task.FromResult(new ProfileTypeDto
                { Id = result.Id, Name = result.Name, ArabicName = result.ArabicName });
            else
            {
                throw new Exception("Profile type not found");
            }
        }

        public Task<TendorCategory> GetServicesOfferedByIdAsync(int offering)
        {
            var servicesOffered = _cHdbContext.TendorCategories.FirstOrDefault(d => d.Id == offering);
            if (servicesOffered == null)
                throw new Exception("Service offering not found. Please check the id provided.  " + offering);
            return Task.FromResult(servicesOffered);




        }

        public Task<List<VendorCategories>> GetServicesOfferedByVendorIdAsync(string userAppId)
        {
            var servicesOffered = _cHdbContext.VendorCategories
                .Where(d => d.VendorId == userAppId)
                .ToList();
            return Task.FromResult(servicesOffered);
        }

        public Task DeleteServicesOfferedAsync(VendorCategories service)
        {
            var vendorCategory = _cHdbContext.VendorCategories.FirstOrDefault(d => d.VendorId == service.VendorId && d.TendorCategoryId == service.TendorCategoryId);
            if (vendorCategory == null) return Task.CompletedTask;
            _cHdbContext.VendorCategories.Remove(vendorCategory);
            _cHdbContext.SaveChanges();
            return Task.CompletedTask;
        }

        public async Task<object> GetCityAsync()
        {
            var cities = _cHdbContext.Cities
                .Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name,
                    ArabicName = c.ArabicName,
                    WilayatId = c.WilayatId,
                })
                .ToList();
            return await Task.FromResult<object>(cities);
        }

        public async Task<FreelancerBankAccount> AddBankAccount(FreelancerBankAccount bankAccount)
        {
            _cHdbContext.FreelancerBankAccounts.Add(bankAccount);
            await _cHdbContext.SaveChangesAsync();
            return bankAccount;
        }

        public async Task<FreelancerBankAccount> UpdateBankAccount(FreelancerBankAccount bankAccount)
        {
            _cHdbContext.FreelancerBankAccounts.Update(bankAccount);
            await _cHdbContext.SaveChangesAsync();
            return bankAccount;
        }

        public async Task<FreelancerBankAccount> GetBankAccountByProfileId(int profileId)
        {
            try
            {
                return await _cHdbContext.FreelancerBankAccounts
                    .Include(d => d.Bank)
                    .FirstOrDefaultAsync(d => d.ProfileId == profileId);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ProfileVideo>> GetVideosByProfileId(int profileId)
        {
           
            var result = _cHdbContext.ProfileVideos.Where(a => a.ProfileId == profileId);
        
            return await result.ToListAsync();
        }
        public async Task<ProfileVideo> AddProfileVideo(ProfileVideo pVideo)
        {
            try
            {

                var result = _cHdbContext.ProfileVideos.Add(pVideo);
                await _cHdbContext.SaveChangesAsync();

                return result.Entity;
            }
            catch (Exception ex)
            {
                // You can log the error here if desired, then rethrow or return a fallback.
                // Re-throw with more context, preserving the original exception as the inner exception:
                throw new Exception("Error creating the Payment.", ex);
            }
        }
        public async Task<bool> DeleteProfileVideo(int id)
        {
            try
            {
                var item = _cHdbContext.ProfileVideos.Where(a => a.Id==id).FirstOrDefault();

                 _cHdbContext.ProfileVideos.Remove(item);
                 await _cHdbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // You can log the error here if desired, then rethrow or return a fallback.
                // Re-throw with more context, preserving the original exception as the inner exception:
               
                throw new Exception("Error Delete Profile.", ex);
            }

        }

        public async Task DeleteBankAccount(int profileId)
        {
            var bankAccount = _cHdbContext.FreelancerBankAccounts.FirstOrDefault(d => d.ProfileId == profileId);
            if (bankAccount != null)
            {
                _cHdbContext.FreelancerBankAccounts.Remove(bankAccount);
                await _cHdbContext.SaveChangesAsync();
            }
            return;
        }

        public async Task<List<Profile>> GetProfilesPendingTraVerification()
        {
            var users = await _cHdbContext.Users
                .Include(x => x.Profile)
                .Where(x => x.Profile.ProfileType == (int)Enums.ProfileTypes.Operator && x.IsActive == false)
                .Select(x => x.Profile)
                .ToListAsync();
            return users ?? new List<Profile>();
        }

        public async Task<Profile> ActivateProfile(int profileId)
        {
            try
            {

                var fetched = await _cHdbContext.Users.Include(x=>x.Profile).FirstOrDefaultAsync(x => x.ProfileId == profileId);
                if (fetched == null) throw new Exception("Profile not found");
                fetched.IsActive = true;
                _cHdbContext.Update(fetched);
                await _cHdbContext.SaveChangesAsync();
                return fetched.Profile;

            }
            catch (Exception e)
            {
                throw new Exception($"Error updating social profile: {e.Message}");
            }
        }
        public async Task DeleteProfile(int profileId)
        {
            try
            {

                var user = await _cHdbContext.Users.FirstOrDefaultAsync(x => x.ProfileId == profileId);
                if (user != null)
                {
                    var userRoles = _cHdbContext.UserRoles.Where(i => i.UserId == user.Id).ToList();
                    _cHdbContext.UserRoles.RemoveRange(userRoles);
                    _cHdbContext.Users.Remove(user);
                    await _cHdbContext.SaveChangesAsync();
                }

                var profile = await _cHdbContext.Profiles.FirstOrDefaultAsync(x => x.Id == profileId);
                if (profile != null)
                {
                    _cHdbContext.Profiles.Remove(profile);
                    await _cHdbContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating profile: {e.Message}");
            }
        }
    }
}
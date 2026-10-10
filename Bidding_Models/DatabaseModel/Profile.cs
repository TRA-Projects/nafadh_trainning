using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text;
using static Bidding_Models.Enums;

namespace Bidding_Models.DatabaseModel
{
  public  class Profile : EntityTrail
    {
        public Profile()
        {
            ProfileDocumentList = new List<ProfileDocument>();

            Country = new Country();
         Address = new Address();
            Contact = new Contact();
            LegalStatus = new LegalStatus();
            RegistrationStatus = new RegistrationStatus();

            Capital = new Capital();
            //Governorate = new Governorate();
            //Wilayat = new Wilayat();
            //City = new City();
            
        }

  
        public CompanyInfo CompanyInfo { get; set; }
        public List<ProfileSocial> ProfileSocialList { get; set; }
        public List<ProfilePortfolio> ProfilePortfolioList { get; set; }

        public List<ProfileWorkExperience> ProfileWorkExperienceList { get; set; }
       
        public List<ProfileEducation> ProfileEducationList { get; set; }
        
        public List<ProfileCertificate> ProfileCertificateList { get; set; }
        
        public List<ProfileSkill> ProfileSkillList { get; set; }
        public int Id { get; set; }
        public string Avatar { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[StringLength(40)]
        [NotMapped]
        public string EncryptedId { get; set; }
        public string Name { get; set; }
        public bool IsVerifiedInMol { get; set; } = false;
        public string NameAr { get; set; }
        public string RyadaNo { get; set; }
        public string CrNumber { get; set; }
        public string OperatorRegisterationAttachment { get; set; }
        public string CompanyNameArabic { get; set; }
        public string CompanyNameEnglish { get; set; }
        public string CompanyNameWithLegalArabic { get; set; }
        public string CompanyNameWithLegalEnglish { get; set; }
        public string RegistrationDate { get; set; }
        public string EstablishmentDate { get; set; }
        public string SubjectToForeignInvestment { get; set; }
        public string ExpiryDate { get; set; }
        public string IsExpired { get; set; }
        public Country Country { get; set; }
        public Address Address { get; set; }
        public Contact Contact { get; set; }
        public LegalStatus LegalStatus { get; set; }
        public RegistrationStatus RegistrationStatus { get; set; }
        public List<DeclaredActivity> DeclaredActivities { get; set; }
        public List<Payment> Payments { get; set; }
        public List<UserSpecalization> UserSpecalizationList { get; set; }
        
        public Capital Capital { get; set; }
        public ICollection<AppUser> User { get; set; }
        //public string UserId { get; set; }
        public string AboutUs { get; set; }

        public string MissionVision { get; set; }
        //public int? Region { get; set; }
        //public int? Wilayat { get; set; }

        //public int? City { get; set; }


        public Governorate Governorate { get; set; }
        public int? GovernorateId { get; set; }

        public Wilayat Wilayat { get; set; }
        public int? WilayatId { get; set; }

        public City City { get; set; }
        public int? CityId { get; set; }

        public string Brochure{ get; set; }

        public string Registration { get; set; }

        // New From Makeen
        public string Gender { get; set; } = null;
        public string Education { get; set; } = null;
        public string Occupation { get; set; } = null;
        public string SponsorName { get; set; } = null;
        //
        public string AddressName { get; set; }

        [NotMapped]
        public string RegistrationName
        {
            get
            {

                if (!String.IsNullOrEmpty(Registration))
                {

                    return Registration.Substring(Registration.IndexOf("_") + 1);
                  
                }

                return String.Empty;
            }
        }

        public bool IsVerified { get; set; }
        /// <summary>
        /// Start New Updates
        /// </summary>
        public int ProfileType { get; set; }
       
        [NotMapped]
        public bool? IsVerifiedProfile { get; set; } = null;
        [NotMapped]
        public bool? IsActiveAccount { get; set; } = null;
        
        [NotMapped]
        public string ProfileTypeName
        {
            get
            {
                string ProfileTypeName = String.Empty;

                switch (ProfileType)
                {
                    case 0:
                        ProfileTypeName = "Admin";
                        break;
                    case 1:
                        ProfileTypeName = "Freelancer";
                        break;
                    case 2:
                        ProfileTypeName = "SME";
                        break;
                    case 3:
                        ProfileTypeName = "Contractor";
                        break;
                    case 4:
                        ProfileTypeName = "Operator";
                        break;
                    case 5:
                        ProfileTypeName = "Operator User";
                        break;
                    case 6:
                        ProfileTypeName = "Staff";
                        break;
                    case 7:
                        ProfileTypeName = "Non SME";
                        break;
                    default:
                        break;
                }
       

                return ProfileTypeName;
            }
        }

        public string ShortName { get; set; }

        public string ComanyEmail { get; set; }
        public string ComanyPhone { get; set; }
        public string ComanyLogo { get; set; }
        public string ComanyWebsiteAddress { get; set; }
        public string ComanyAbout { get; set; }
        [InverseProperty("Profile")]
        public List<Project> ProjectsList { get; set; }

        public List<Certificate> CertificatesList { get; set; }

        public List<Subscriptions> SubscriptionList { get; set; }

        public List<PaymentsTransactions> PaymentsTarsnactionsList { get; set; }

        public List<Staff> StaffList { get; set; }

        /// <summary>
        /// End New Updates
        /// </summary>
        public ProfileSetting Settings { get; set; }
        public AccountDetails AccountDetails { get; set; }
        
        public Profile ParentProfile { get; set; }

        public int? ParentProfileId { get; set; }
        public List<Profile> ParentProfilesList { get; set; }

        public List<Department> DepartmentList { get; set; }

        public List<ProfileDocument> ProfileDocumentList { get; set; }
        public List<OperatorUser> OperatorUsers { get; set; }

        public List<ProfileApprovalHistory> ProfileApprovalHistoryList { get; set; }
        public bool IsHead { get; set; }

        public bool IsPostOpertunity { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }

        /// <summary>Preferred notification language: <c>en</c> or <c>ar</c>.</summary>
        [MaxLength(8)]
        public string PreferredLanguage { get; set; } = "en";

        public string DesignationEn { get; set; }
        public string DesignationAr { get; set; }

        public Department Department { get; set; }
        public int? DepartmentId { get; set; }
        public bool IsOmani { get; set; }
        public bool IsOnline { get; set; } = false;

        public string StaffNumber { get; set; }

        public bool IsActive { get; set; }

        [Column("user_profile_completed")]
        public bool UserProfileCompleted { get; set; }

        /// <summary>
        /// False for job seekers pre-loaded from the MOL import who have no login yet;
        /// set to true when the person registers and claims the profile by civil number.
        /// </summary>
        public bool IsRegistered { get; set; } = true;

        public DateTime JoinDate { get; set; }

        public int TotalExperience { get; set; }
   
        public DateTime? CRExpiryDate { get; set; } = null;

   
        public DateTime? RCExpiryDate { get; set; } = null;

        public ProfileStatus ProfileStatus { get; set; }

        public string ProfileRejectionReason { get; set; }
        public string RiyadaCard { get; set; }
        /// <summary>
        /// For fetchin from MOL
        /// </summary>
        /// 
        public string Mol_JobSearchStatus { get; set; }
        public string Mol_JobSearchDuration { get; set; }
        public string Mol_DateOfFirstRegistrationAsJobSeeker { get; set; }
        public string Mol_DateOfLastRegistrationAsJobSeeker { get; set; }
        public string Mol_DateOfBirth { get; set; }
        public string Mol_MaritalStatus { get; set; }
        public string Mol_HealthStatus { get; set; }
        public string Mol_Governorate { get; set; }
        public string Mol_Willaya { get; set; }
        public string Mol_Nationality { get; set; }
        public string Mol_JobStatus { get; set; }
        public string Mol_CurrentEmploymentDate { get; set; }
        public string Mol_Allowance { get; set; }
        

        
        /////////////
        ////
        //////

        [NotMapped]
        public string RiyadaCardName { get {

                if (!String.IsNullOrEmpty(RiyadaCard))
                {

                   return RiyadaCard.Substring(RiyadaCard.IndexOf("_") + 1);
          
                }

                return String.Empty;
            } }


        public string AffiliationCertificate { get; set; }

        [NotMapped]
        public string AffiliationCertificateName
        {
            get
            {

                if (!String.IsNullOrEmpty(AffiliationCertificate))
                {

                    return AffiliationCertificate.Substring(AffiliationCertificate.IndexOf("_") + 1);

                }

                return String.Empty;
            }
        }


        public string TaxCertificate { get; set; }

        [NotMapped]
        public string TaxCertificateName
        {
            get
            {

                if (!String.IsNullOrEmpty(TaxCertificate))
                {

                    return TaxCertificate.Substring(TaxCertificate.IndexOf("_") + 1);

                }

                return String.Empty;
            }
        }

        public string OmanizationPercentage { get; set; }

        [NotMapped]
        public string OmanizationPercentageName
        {
            get
            {

                if (!String.IsNullOrEmpty(OmanizationPercentage))
                {

                    return OmanizationPercentage.Substring(OmanizationPercentage.IndexOf("_") + 1);

                }

                return String.Empty;
            }
        }

        public string TeamworkExperience { get; set; }

        [NotMapped]
        public string TeamworkExperienceName
        {
            get
            {

                if (!String.IsNullOrEmpty(TeamworkExperience))
                {

                    return TeamworkExperience.Substring(TeamworkExperience.IndexOf("_") + 1);

                }

                return String.Empty;
            }
        }

        public string CivilFrontCard { get; set; }

        [NotMapped]
        public string CivilFrontCardName
        {
            get
            {

                if (!String.IsNullOrEmpty(CivilFrontCard))
                {

                    return CivilFrontCard.Substring(CivilFrontCard.IndexOf("_") + 1);

                }

                return String.Empty;
            }
        }

        public string CivilBackCard { get; set; }

        [NotMapped]
        public string CivilBackCardName
        {
            get
            {

                if (!String.IsNullOrEmpty(CivilBackCard))
                {

                    return CivilBackCard.Substring(CivilBackCard.IndexOf("_") + 1);

                }

                return String.Empty;
            }
        }

        [NotMapped]
        public List<DocumentWrapper> DocumentWrapperList { get; set; }
        [NotMapped]
        public IFormFile UserPicFile { get; set; }

        [NotMapped]
        public IFormFile RiyadaCardFile { get; set; }

        [NotMapped]
        public IFormFile AffiliationCertificateFile { get; set; }

        [NotMapped]
        public IFormFile TaxCertificateFile { get; set; }

        [NotMapped]
        public IFormFile OmanizationPercentageFile { get; set; }

        [NotMapped]
        public IFormFile TeamworkExperienceFile { get; set; }

        [NotMapped]
        public IFormFile CivilFrontCardFile { get; set; }

        [NotMapped]
        public IFormFile CivilBackCardFile { get; set; }

        [NotMapped]
        public IFormFile CompanyPicFile { get; set; }
        [NotMapped]
        public IFormFile BrochureFile { get; set; }
        [NotMapped]
        public IFormFile RegistrationFile { get; set; }

        [NotMapped]
        public string AvatarImg { get; set; }
        [NotMapped]
        public string PortfolioDocumentString { get; set; }

        [NotMapped]
        public string CertificationsDocumentString { get; set; }

        [NotMapped]
        public List<DocumentWrapper> PortfolioWrapperList { get; set; }

        [NotMapped]
        public List<DocumentWrapper> CertificationsWrapperList { get; set; }

        [NotMapped]
        public bool issubmit { get; set; }
        [NotMapped]
        public List<SelectListItem> CityDD { get; set; }
        [NotMapped]
        public List<SelectListItem> WilayatDD { get; set; }
        [NotMapped]
        public List<SelectListItem> GovernoratesDD { get; set; }

        [NotMapped]
        public int ParentProfileCount { get; set; }

        [NotMapped]
        public int DepartmentCount { get; set; }
        [NotMapped]
        public int CRDocumentCount { get; set; }
        [NotMapped]
        public int RiyadaCardCount { get; set; }

        [NotMapped]
        public string Note { get; set; }

    }
}

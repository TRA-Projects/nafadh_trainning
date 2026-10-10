using System.Collections.Generic;
using Bidding_Models.Jobseekers;

namespace Bidding_Models.DTOs
{
    public class ProfileUserResponseDto
    {
        public int ProfileId { get; set; }
        public string About { get; set; }
        public string Website { get; set; }
        public string Address { get; set; }

        public string MissionVision { get; set; }
        public string ProfileImage { get; set; }
        public bool IsActive { get; set; }
        public string CrNumber { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool IsEmailVerified { get; set; }
        /// <summary>Whether the job seeker finished the profile-completion wizard.</summary>
        public bool UserProfileCompleted { get; set; }
        /// <summary>Preferred notification language: <c>en</c> or <c>ar</c>.</summary>
        public string PreferredLanguage { get; set; }
        public string AttachmentFileName { get; set; }
        public ProfileTypeDto VendorType { get; set; }
        public List<ServiceOfferingGeneralDto> ServicesOffered { get; set; }
        public JobSeekerCvAttachmentDto CvAttachment { get; set; }
       
        
    }
}



using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace Bidding_Models.DatabaseModel
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            Roles = new List<Roles>();
            
        
        }

        public string OTP { get; set; }

        public string OldPassword { get; set; }

        public bool isRegisterbyAdmin { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string VerificationToken  { get; set; }

        public virtual ICollection<ChatMessage> SenderChatMessage { get; set; }

        public virtual ICollection<ChatMessage> ReceiverChatMessage { get; set; }

        [ForeignKey(nameof(ProfileId))]
        public Profile Profile { get; set; }
        public int? ProfileId { get; set; }

        public int? UserGroupId { get; set; }

        [ForeignKey(nameof(UserGroupId))]
        public IdentityAccess.UserGroup UserGroup { get; set; }

        public int? OrganizationId { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public IdentityAccess.Organization Organization { get; set; }

        /// <summary>When multiple AspNetUsers share a Profile, login uses the primary row.</summary>
        public bool IsPrimary { get; set; }

        public bool IsActive { get; set; } = true;
        public virtual  List<Request> UserRequestList { get; set; }

        public virtual List<Request> VendorAwardedList { get; set; }
        public List<Notification> NotificationList { get; set; }
        public List<Bid> BidList { get; set; }
        public List<NDA> NDAList { get; set; }
        

        public List<RequestViewHistory> RequestViewHistoryList { get; set; }

        public List<VendorSavedRequest> VendorSavedRequestList { get; set; }

        public List<VendorCategories> VendorCategoriesList { get; set; }

        public List<Feedback> FeedbackFromUserList { get; set; }

        public List<Feedback> FeedbackForUserList { get; set; }

        public List<VendorBidding> VendorBiddingList { get; set; }

        public List<VendorExtensionRequest> VendorExtensionRequestList { get; set; }

        public List<RequestParticipents> RequestParticipentsList { get; set; }
        
        [NotMapped]
        public List<Roles> Roles { get; set; }
        [NotMapped]
        public List<SelectListItem> Clientslist { get; set; }

        public List<AppUserRoles> AppUserRoles { get; set; }

        public List<UserSkills> UserSkillsList { get; set; }

       

        [NotMapped]
        public string  RoleName { get; set; }

        [NotMapped]
        [DataType(DataType.Password, ErrorMessage = "The {0} must be minimum 8 characters and contains a combination of (uppercase and lowercase letter And number And special characters)")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8}$",
        // ErrorMessage = "Password must meet requirements")]
        public string Password { get; set; }

        [NotMapped]
        public bool IsRemember { get; set; }
        [NotMapped]
        public string ReturnUrl { get; set; }
        [NotMapped]
        public bool IsVendor { get; set; } 
        [NotMapped]
        public bool IsClient { get; set; }
        [NotMapped]
        public int ProfileType { get; set; }
        

        [NotMapped]
        public Notification LastNotification { get; set; }


        [NotMapped]
        public ChatMessage LastMessage { get; set; }


        [NotMapped]
        public int UnreadMessages { get; set; }
        [NotMapped]
        public string EmailVerificationUrl { get; set; }


        [NotMapped]
        public string Action { get; set; }
        [NotMapped]
        public string Controller { get; set; }
        [NotMapped]
        public string Host { get; set; }
        [NotMapped]
        public string Protocol { get; set; }


        [NotMapped]
        public string EmailVerificationToken { get; set; }

        [NotMapped]
        public string ResetPasswordToken { get; set; }

        [NotMapped]

        public PagedResult<AppUser> PagedResult { get; set; }

        [NotMapped]
        public int AverageRating { get; set; }
        [NotMapped]
        public string ReceiverUserId { get; set; }

        [NotMapped]
        public List<AppUser> FeedbackforProfile { get; set; }
        [NotMapped]
        public List<SelectListItem> TendorCategoryList { get; set; }

        [NotMapped]
        public List<SelectListItem> SkillsList { get; set; }


        [NotMapped]
        public List<SelectListItem> SpecializationList { get; set; }

        [NotMapped]
        public List<int> SelectedTendorCategories { get; set; }

        [NotMapped]
        public List<int> SelectedSkills { get; set; }

        [NotMapped]
        public List<int> SelectedSpecializations { get; set; }

        [NotMapped]
        public string TendorCategoryId { get; set; }
        [NotMapped]
        public string FromDate { get; set; }
        [NotMapped]
        public string ToDate { get; set; }
        [NotMapped]
        public string isRegisterbyAdminString { get; set; }

        [NotMapped]
        public string NewPassword { get; set; }

        [NotMapped]
        public List<DeclaredActivity> Activities { get; set; }

        [NotMapped]
        public List<SelectListItem> DepartmentsList { get; set; }

        [NotMapped]
        public List<SelectListItem> BankList { get; set; }
        [NotMapped]
        public string Token { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }


        [NotMapped]
        public int ProposalCount { get; set; }

        [NotMapped]
        public int AwardedOpportunitiesCount { get; set; }

        [NotMapped]
        public int AttendedTrainingCount { get; set; }

        [NotMapped]
        public bool IsEncryptedEmail { get; set; } = false;

    }
}

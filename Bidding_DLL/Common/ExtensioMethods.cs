
using Bidding_Models;
using Bidding_Models.DatabaseModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding_DLL.Common
{
    public static class ExtensioMethods
    {
        static UserManager<AppUser> manager;




        public static void Seed(this ModelBuilder builder)
        {
            //modelBuilder.Entity<Roles>().HasData(new Roles("Candidate"), new Roles("Employer"), new Roles("Admin"));

            const string ADMIN_ID = "a18be9c0-aa65-4af8-bd17-00bd9344e575";
            const string User_ID = "a18be9c0-aa65-4af8-bd17-00bd9344e576";
            // const string VENDOR_ID = "a18be9c0-aa65-4af8-bd17-00bd9344e577";

            const string USER_ROLE_ID = "5eba9226-5f58-4a2d-8c04-3126aefd3698";
            const string ADMIN_ROLE_ID = "690f679c-4431-41ad-b098-82fc4f8c4dea";
            const string VENDOR_ROLE_ID = "690f679c-4431-41ad-b098-82fc4f8c4deb";


            builder.Entity<Roles>().HasData(new Roles
            {
                Id = ADMIN_ROLE_ID,
                Name = "Admin",
                NormalizedName = "Admin",
                IsTemplate = false,
                IsActive = true
            });

            builder.Entity<Roles>().HasData(new Roles
            {
                Id = USER_ROLE_ID,
                Name = "User",
                NormalizedName = "User",
                IsTemplate = false,
                IsActive = true
            });

            builder.Entity<Roles>().HasData(new Roles
            {
                Id = VENDOR_ROLE_ID,
                Name = "Vendor",
                NormalizedName = "Vendor",
                IsTemplate = false,
                IsActive = true
            });


            var hasher = new PasswordHasher<IdentityUser>();
            builder.Entity<AppUser>().HasData(new AppUser
            {
                Id = ADMIN_ID,
                UserName = "admin@constructionhub.om",
                NormalizedUserName = "admin@constructionhub.om",
                Email = "admin@constructionhub.om",
                NormalizedEmail = "admin@constructionhub.om",
                PhoneNumberConfirmed = true,
                PhoneNumber = "97487434",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "784qt8g$E"),
                SecurityStamp = string.Empty,
                IsPrimary = true,
                IsActive = true
            });

            builder.Entity<AppUser>().HasData(new AppUser
            {
                Id = User_ID,
                UserName = "duser@gmail.com",
                NormalizedUserName = "duser@gmail.com",
                Email = "duser@gmail.com",
                NormalizedEmail = "duser@gmail.com",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "123"),
                SecurityStamp = string.Empty,
                IsPrimary = true,
                IsActive = true
            });



            builder.Entity<AppUserRoles>().HasData(new AppUserRoles
            {
                RoleId = ADMIN_ROLE_ID,
                //  RoleId = 3.ToString(),
                UserId = ADMIN_ID

            });

            builder.Entity<AppUserRoles>().HasData(new AppUserRoles
            {
                RoleId = USER_ROLE_ID,
                //  RoleId = 3.ToString(),
                UserId = User_ID

            });


            #region Profile Section

            //builder.Entity<Profile>().HasData(new Profile
            //{
            //    UserId = ADMIN_ID,
            //    Id = 1,
            //    Name = "Super Admin",
            //    InsertedDate = DateTime.Now
            //});

            //builder.Entity<Profile>().HasData(new Profile
            //{
            //    UserId = User_ID,
            //    Id = 2,
            //    Name = "Default User",
            //    InsertedDate = DateTime.Now
            //});
            #endregion


            #region Profile Setting Section

            builder.Entity<ProfileSetting>().HasData(new ProfileSetting
            {
                Id = 1,
                ProfileId = 1,
                SendEmail = true,
                SendSms = true,
                ShowEmail = true,
                ShowMobileNo = true
            });

            builder.Entity<ProfileSetting>().HasData(new ProfileSetting
            {
                Id = 2,
                ProfileId = 2,
                SendEmail = true,
                SendSms = true,
                ShowEmail = true,
                ShowMobileNo = true
            });
            #endregion



            //builder.Entity<Profile>().HasData(new Profile
            //{
            //    UserId = VENDOR_ID,
            //    Id = 3,
            //    Name = "Super Vendor"
            //});






            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 1,
                Name = "Vendor Registration",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 2,
                Name = "Requirement Posted",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 3,
                Name = "Requirement Update",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 4,
                Name = "Bid Placed",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 5,
                Name = "Bid Update",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 6,
                Name = "Bid Rejected",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 7,
                Name = "Opportunity Awarded",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 8,
                Name = "Project Completed",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 9,
                Name = "Project Finish",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 10,
                Name = "Requirement Accepted",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 11,
                Name = "Requirement Rejected",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 12,
                Name = "Client Registration",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 13,
                Name = "Send OTP",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 14,
                Name = "Forgot Password",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 15,
                Name = "Contact Us",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 16,
                Name = "Vendor Reminder For Request Expiry",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 17,
                Name = "Client Reminder For Request Expiry",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 18,
                Name = "Extended Expiry Date",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 19,
                Name = "Requirement Expired",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 20,
                Name = "Project Accepted",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 21,
                Name = "Feedback for Vendor",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 22,
                Name = "Feedback for User",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 23,
                Name = "Cancedled Request",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 24,
                Name = "Ask Question",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 25,
                Name = "Answer Submitted",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 26,
                Name = "Technical Reject",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 27,
                Name = "Technical Approve",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 28,
                Name = "Financial Reject",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 29,
                Name = "Financial Approve",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 30,
                Name = "Email Verification",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 31,
                Name = "Missing Documents",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 32,
                Name = "Added Sub Category",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 33,
                Name = "Assign Permission",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 34,
                Name = "Remove Permission",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 35,
                Name = "Oprator User Registration",
            });


            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 36,
                Name = "Extention Request By Vendor",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 37,
                Name = "Post Proposal Clarification",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 38,
                Name = "Post Proposal Clarification Answer",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 39,
                Name = "Oppertunity Awarded",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 40,
                Name = "Training Registration",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 41,
                Name = "Before Training Registration",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 42,
                Name = "2 Weeks Before the Closed Date",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 43,
                Name = "1 Weeks Before the Closed Date",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 44,
                Name = "3 Days Before the Closed Date",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 45,
                Name = "Delete User",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 46,
                Name = "Reminder Email to Vendors Before Last Submission Date",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 47,
                Name = "Update Profile",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 48,
                Name = "Profile Approved",
            });
            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 49,
                Name = "Profile Rejected",
            });

            builder.Entity<NotificationType>().HasData(new NotificationType
            {
                Id = 50,
                Name = "Reset Password Notification For All Users",
            });

            builder.Entity<AppUserRoles>()
             .HasOne(p => p.Role)
             .WithMany(b => b.AppUserRolesList)
             .HasForeignKey(p => p.RoleId).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AppUserRoles>()
            .HasOne(p => p.User)
            .WithMany(b => b.AppUserRoles)
            .HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AppUserRoles>()
     .HasIndex(p => new { p.RoleId, p.UserId }).IsUnique();


            builder.Entity<ChatMessage>()
            .HasOne(p => p.Sender)
            .WithMany(b => b.SenderChatMessage)
            .HasForeignKey(p => p.SenderId);


            builder.Entity<ChatMessage>()
          .HasOne(p => p.Receiver)
          .WithMany(b => b.ReceiverChatMessage)
          .HasForeignKey(p => p.ReceiverId);

            builder.Entity<Request>()
            .HasOne(p => p.User)
            .WithMany(b => b.UserRequestList)
            .HasForeignKey(p => p.UserId);



            builder.Entity<Request>()
            .HasOne(p => p.Vendor)
            .WithMany(b => b.VendorAwardedList)
            .HasForeignKey(p => p.VendorId);

            builder.Entity<Request>()
          .HasOne(p => p.City)
          .WithMany(b => b.RequestList)
          .HasForeignKey(p => p.CityId);

            builder.Entity<Request>()
          .HasOne(p => p.Wilayat)
          .WithMany(b => b.RequestList)
          .HasForeignKey(p => p.WilayatId);


            builder.Entity<Request>()
   .HasOne(p => p.City)
   .WithMany(b => b.RequestList)
   .HasForeignKey(p => p.CityId);

            builder.Entity<RequestEvaluationCriteria>()
.HasOne(p => p.Request)
.WithMany(b => b.RequestEvaluationCriteriaList)
.HasForeignKey(p => p.RequestId);

            builder.Entity<RequestPrerequiste>()
.HasOne(p => p.Request)
.WithMany(b => b.RequestPrerequisteList)
.HasForeignKey(p => p.RequestId);

            builder.Entity<RequestRequiredDocument>()
.HasOne(p => p.Request)
.WithMany(b => b.RequestRequiredDocumentList)
.HasForeignKey(p => p.RequestId);

            builder.Entity<RequestNDADocument>()
.HasOne(p => p.Request)
.WithMany(b => b.RequestNDADocumentList)
.HasForeignKey(p => p.RequestId);

            builder.Entity<TendorCategory>()
            .HasOne(p => p.ParentCategory)
            .WithMany(b => b.ChildCategoryList)
            .HasForeignKey(p => p.ParentCategoryId);


            builder.Entity<Question>()
            .HasOne(p => p.TendorCategory)
            .WithMany(b => b.QuestionsList)
            .HasForeignKey(p => p.TendorCategoryId);

            builder.Entity<RequestQuestion>()
            .HasOne(p => p.Request)
            .WithMany(b => b.RequestQuestionList)
            .HasForeignKey(p => p.RequestId);

            builder.Entity<RequestQuestion>()
            .HasOne(p => p.Question)
            .WithMany(b => b.RequestQuestionList)
            .HasForeignKey(p => p.QuestionId);


            builder.Entity<RequestQuestion>()
            .HasKey(rq => new { rq.QuestionId, rq.RequestId });

            builder.Entity<RequestQuestion>()
           .HasOne(bc => bc.Request)
           .WithMany(b => b.RequestQuestionList)
           .HasForeignKey(bc => bc.RequestId).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<RequestQuestion>()
            .HasOne(bc => bc.Question)
            .WithMany(c => c.RequestQuestionList)
            .HasForeignKey(bc => bc.QuestionId).OnDelete(DeleteBehavior.NoAction);


            builder.Entity<Bid>()
            .HasOne(p => p.Request)
            .WithMany(b => b.BidList)
            .HasForeignKey(p => p.RequestId);

            builder.Entity<Bid>()
               .HasOne(p => p.Vendor)
               .WithMany(b => b.BidList)
               .HasForeignKey(p => p.VendorId);


            builder.Entity<NDA>()
         .HasOne(p => p.Request)
         .WithMany(b => b.NDAList)
         .HasForeignKey(p => p.RequestId);

            builder.Entity<NDA>()
               .HasOne(p => p.Vendor)
               .WithMany(b => b.NDAList)
               .HasForeignKey(p => p.VendorId);

            builder.Entity<BidDocument>()
                    .HasOne(p => p.Bid)
                    .WithMany(b => b.BidDocumentList)
                    .HasForeignKey(p => p.BidId);

            builder.Entity<NDADocument>()
               .HasOne(p => p.NDA)
               .WithMany(b => b.NDADocumentList)
               .HasForeignKey(p => p.NDAId);


            builder.Entity<BidEvaluation>()
               .HasOne(p => p.Bid)
               .WithMany(b => b.BidEvaluationList)
               .HasForeignKey(p => p.BidId);

            builder.Entity<BidEvaluation>()
              .HasOne(p => p.RequestEvaluationCriteria)
              .WithMany(b => b.BidEvaluationList)
              .HasForeignKey(p => p.RequestEvaluationCriteriaId).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AppUser>()
            .HasOne(p => p.Profile)
            .WithMany(b => b.User)
            .HasForeignKey(p => p.ProfileId);

            builder.Entity<Profile>()
          .HasOne(p => p.Settings)
          .WithOne(b => b.Profile)
          .HasForeignKey<ProfileSetting>(p => p.ProfileId);

            builder.Entity<Profile>()
        .HasOne(p => p.AccountDetails)
        .WithOne(b => b.Profile)
        .HasForeignKey<AccountDetails>(p => p.ProfileId);

            builder.Entity<AccountDetails>()
.HasOne(p => p.Bank)
.WithMany(b => b.AccountDetails)
.HasForeignKey(p => p.BankId);




            builder.Entity<Profile>().OwnsOne(p => p.Country);
            builder.Entity<Profile>().OwnsOne(p => p.Address);

            builder.Entity<Profile>().OwnsOne(p => p.Contact);
            builder.Entity<Profile>().OwnsOne(p => p.LegalStatus);
            builder.Entity<Profile>().OwnsOne(p => p.RegistrationStatus);

            builder.Entity<DeclaredActivity>()
            .HasOne(p => p.Profile)
            .WithMany(b => b.DeclaredActivities)
            .HasForeignKey(p => p.ProfileId);



            #region VendorCatergories
            builder.Entity<VendorCategories>()
        .HasKey(bc => new { bc.TendorCategoryId, bc.VendorId });
            builder.Entity<VendorCategories>()
                .HasOne(bc => bc.Vendor)
                .WithMany(b => b.VendorCategoriesList)
                .HasForeignKey(bc => bc.VendorId);
            builder.Entity<VendorCategories>()
                .HasOne(bc => bc.TendorCategory)
                .WithMany(c => c.VendorCategoriesList)
                .HasForeignKey(bc => bc.TendorCategoryId);
            builder.Entity<Request>()
                .HasOne(bc => bc.TendorCategory)
                .WithMany()
                .HasForeignKey(bc => bc.TendorCategoryId);
            #endregion


            #region Notifications

            builder.Entity<Notification>()
            .HasOne(p => p.Request)
            .WithMany(b => b.NotificationList)
            .HasForeignKey(p => p.RequestId);


            builder.Entity<Notification>()
           .HasOne(p => p.ForUser)
           .WithMany(b => b.NotificationList)
           .HasForeignKey(p => p.ForUserId);


            builder.Entity<Notification>()
         .HasOne(p => p.Bid)
         .WithMany(b => b.NotificationList)
         .HasForeignKey(p => p.BidId);


            builder.Entity<Notification>()
     .HasOne(p => p.NDA)
     .WithMany(b => b.NotificationList)
     .HasForeignKey(p => p.NDAId);

            builder.Entity<Notification>()
     .HasOne(p => p.RequestClarificationQuestion)
     .WithMany(b => b.NotificationList)
     .HasForeignKey(p => p.RequestClarificationQuestionId);





            builder.Entity<Notification>()
    .HasOne(p => p.ProposalClarification)
    .WithMany(b => b.NotificationList)
    .HasForeignKey(p => p.ProposalClarificationId);
            #endregion


            #region Profile Document

            builder.Entity<ProfileDocument>()
          .HasOne(p => p.Profile)
          .WithMany(b => b.ProfileDocumentList)
          .HasForeignKey(p => p.ProfileId);
            #endregion

            #region NotificationTypes
            builder.Entity<Notification>()
         .HasOne(p => p.NotificationType)
         .WithMany(b => b.NotificationList)
         .HasForeignKey(p => p.NotificationTypeId);

            #endregion



            #region Vendor Saved Request
            builder.Entity<VendorSavedRequest>()
        .HasKey(bc => new { bc.RequestId, bc.VendorId });

            builder.Entity<VendorSavedRequest>()
                .HasOne(bc => bc.Vendor)
                .WithMany(b => b.VendorSavedRequestList)
                .HasForeignKey(bc => bc.VendorId);
            builder.Entity<VendorSavedRequest>()
                .HasOne(bc => bc.Request)
                .WithMany(c => c.VendorSavedRequestList)
                .HasForeignKey(bc => bc.RequestId);
            #endregion

            #region NotificationType Settings

            builder.Entity<Profile>()
    .HasOne(p => p.Settings)
    .WithOne(b => b.Profile)
    .HasForeignKey<ProfileSetting>(p => p.ProfileId);

            builder.Entity<Profile>()
     .HasOne(p => p.Department)
     .WithMany(b => b.ProfileList)
     .HasForeignKey(p => p.DepartmentId);


            builder.Entity<NotificationType>()
.HasOne(p => p.NotificationTypeSettings)
.WithOne(b => b.NotificationType)
.HasForeignKey<NotificationTypeSetting>(p => p.NotificationTypeId);
            #endregion


            #region FeedBack



            builder.Entity<Feedback>()
        .HasOne(p => p.ForUser)
        .WithMany(b => b.FeedbackForUserList)
        .HasForeignKey(p => p.ForUserId);



            builder.Entity<Feedback>()
     .HasOne(p => p.FromUser)
     .WithMany(b => b.FeedbackFromUserList)
     .HasForeignKey(p => p.FromUserId);





            builder.Entity<Feedback>()
.HasOne(p => p.Bid)
.WithMany(b => b.FeedbackList)
.HasForeignKey(p => p.BidId);
            #endregion


            #region    SMSEmailNotification


            builder.Entity<SMSEmailNotificationParam>()
            .HasOne(p => p.SMSEmailNotification)
            .WithMany(b => b.SMSEmailNotificationParamList)
            .HasForeignKey(p => p.SMSEmailNotificationId);
            #endregion


            #region BidQuotation
            builder.Entity<BidQuotation>()
            .HasOne(p => p.Bid)
            .WithMany(b => b.BidQuotationList)
            .HasForeignKey(p => p.BidId);
            #endregion

            #region Bidding
            builder.Entity<Bidding>()
            .HasOne(p => p.Request)
            .WithOne(b => b.Bidding)
            .HasForeignKey<Bidding>(p => p.RequestId);
            #endregion


            #region Vendor Bidding
            builder.Entity<VendorBidding>()
            .HasOne(p => p.Bidding)
            .WithMany(b => b.VendorBiddingList)
            .HasForeignKey(p => p.BiddingId);

            builder.Entity<VendorBidding>()
         .HasOne(p => p.Vendor)
         .WithMany(b => b.VendorBiddingList)
         .HasForeignKey(p => p.VendorId);
            #endregion


            builder.Entity<RolesClaims>()
             .HasOne(p => p.Role)
             .WithMany(b => b.RolesClaimsList)
             .HasForeignKey(p => p.RoleId);

            builder.Entity<RolesClaims>()
            .HasOne(p => p.ClaimMaster)
            .WithMany(b => b.RolesClaimsList)
            .HasForeignKey(p => p.ClaimMasterId);

            builder.Entity<UserSkills>()
         .HasOne(p => p.User)
         .WithMany(b => b.UserSkillsList)
         .HasForeignKey(p => p.UserId);

            builder.Entity<UserSkills>()
    .HasOne(p => p.Skill)
    .WithMany(b => b.UserSkillsList)
    .HasForeignKey(p => p.SkillId);


            builder.Entity<UserSpecalization>()
     .HasOne(p => p.Profile)
     .WithMany(b => b.UserSpecalizationList)
     .HasForeignKey(p => p.ProfileId);

            builder.Entity<UserSpecalization>()
    .HasOne(p => p.Specalization)
    .WithMany(b => b.UserSpecalizationList)
    .HasForeignKey(p => p.SpecalizationId);


            builder.Entity<Request>().Property(x => x.EncryptedId).HasDefaultValueSql("NEWID()");

            builder.Entity<Request>().HasIndex(b => b.EncryptedId);

            builder.Entity<Bid>().Property(x => x.EncryptedId).HasDefaultValueSql("NEWID()");
            builder.Entity<Bid>().HasIndex(b => b.EncryptedId);


            builder.Entity<NDA>().Property(x => x.EncryptedId).HasDefaultValueSql("NEWID()");
            builder.Entity<NDA>().HasIndex(b => b.EncryptedId);
            //builder.Entity<Profile>().Property(x => x.EncryptedId).HasDefaultValueSql("NEWID()");


            builder.Entity<RequestViewHistory>()
            .HasOne(p => p.Request)
            .WithMany(b => b.RequestViewHistoryList)
            .HasForeignKey(p => p.RequestId);

            builder.Entity<RequestViewHistory>()
            .HasOne(p => p.SeenBy)
            .WithMany(b => b.RequestViewHistoryList)
            .HasForeignKey(p => p.SeenById);


            builder.Entity<Profile>()
              .HasOne(x => x.ParentProfile).WithMany(b => b.ParentProfilesList)
               .HasForeignKey(g => g.ParentProfileId);

            builder.Entity<Department>()
           .HasOne(x => x.Profile).WithMany(b => b.DepartmentList)
            .HasForeignKey(g => g.ProfileId);


            builder.Entity<Staff>()
     .HasOne(x => x.Department).WithMany(b => b.StaffList)
      .HasForeignKey(g => g.DepartmentId);


            builder.Entity<BidDocument>()
    .HasOne(x => x.RequestRequiredDocument).WithMany(b => b.BidDocumentList)
     .HasForeignKey(g => g.RequestRequiredDocumentId);


            builder.Entity<NDADocument>()
.HasOne(x => x.RequestNDADocument).WithMany(b => b.NDADocumentList)
.HasForeignKey(g => g.RequestNDADocumentId);



            builder.Entity<RequestRequiredDocument>()
  .HasOne(p => p.DocumentTypes)
  .WithMany(b => b.RequestRequiredDocumentList)
  .HasForeignKey(p => p.DocumentTypesId);


            builder.Entity<RequestNDADocument>()
.HasOne(p => p.DocumentTypes)
.WithOne(b => b.RequestNDADocumentList)
.HasForeignKey<RequestNDADocument>(p => p.DocumentTypesId);


            builder.Entity<ProposalClarification>()
               .HasOne(p => p.Bid)
               .WithMany(b => b.ProposalClarificationList)
               .HasForeignKey(p => p.BidId);

            builder.Entity<ProposalClarificationDocuments>()
              .HasOne(p => p.ProposalClarification)
              .WithMany(b => b.ProposalClarificationDocumentsList)
              .HasForeignKey(p => p.ProposalClarificationId);

            builder.Entity<VendorExtensionRequest>()
          .HasOne(p => p.Request)
          .WithMany(b => b.VendorExtensionRequestList)
          .HasForeignKey(p => p.RequestId);

            builder.Entity<VendorExtensionRequest>()
         .HasOne(p => p.Vendor)
         .WithMany(b => b.VendorExtensionRequestList)
         .HasForeignKey(p => p.VendorId);


            builder.Entity<RequestParticipents>()
       .HasOne(p => p.Request)
       .WithMany(b => b.RequestParticipentsList)
       .HasForeignKey(p => p.RequestId);

            builder.Entity<RequestParticipents>()
         .HasOne(p => p.User)
         .WithMany(b => b.RequestParticipentsList)
         .HasForeignKey(p => p.UserId);


            builder.Entity<RequestDocument>()
       .HasOne(p => p.OpportunityDocumentType)
       .WithMany(b => b.RequestDocumentList)
       .HasForeignKey(p => p.OpportunityDocumentTypeId);


            //            builder.Entity<TRATraining>()
            //.HasOne(p => p.TrainingMaster)
            //.WithMany(b => b.TRATrainingList)
            //.HasForeignKey(p => p.TrainingMasterId);


/*
 * New Entity Added
 * 
 */            
            
            // builder.Entity<DeclaredActivity>()
            //     .HasOne(p => p.Profile)
            //     .WithMany(b => b.DeclaredActivities)
            //     .HasForeignKey(p => p.ProfileId);

            #region new entity added
               
            builder.Entity<ProfileSocial>()
                .HasOne(p => p.Profile)
                .WithMany(b => b.ProfileSocialList)
                .HasForeignKey(p => p.ProfileId);
            builder.Entity<ProfilePortfolio>()
                .HasOne(p => p.Profile)
                .WithMany(b => b.ProfilePortfolioList)
                .HasForeignKey(p => p.ProfileId);

            builder.Entity<ProfileWorkExperience>()
                .HasOne(p => p.Profile)
                .WithMany(b => b.ProfileWorkExperienceList)
                .HasForeignKey(p => p.ProfileId);
            builder.Entity<ProfileCertificate>()
                .HasOne(p => p.Profile)
                .WithMany(b => b.ProfileCertificateList)
                .HasForeignKey(p => p.ProfileId);
            builder.Entity<ProfileSkill>()
                .HasOne(p => p.Profile)
                .WithMany(b => b.ProfileSkillList)
                .HasForeignKey(p => p.ProfileId);
            builder.Entity<ProfileEducation>()
                .HasOne(p => p.Profile)
                .WithMany(b => b.ProfileEducationList)
                .HasForeignKey(p => p.ProfileId);
            #endregion

            builder.Entity<OpportunityDocumentType>().HasData(new OpportunityDocumentType
            {
                Id = 1,
                Name = "BID Invitation",
                Code = "BID_Invitation",
                IsActive = true
            });

            builder.Entity<OpportunityDocumentType>().HasData(new OpportunityDocumentType
            {
                Id = 2,
                Name = "Prerequisites",
                Code = "Prerequisites",
                IsActive = true
            });

            builder.Entity<OpportunityDocumentType>().HasData(new OpportunityDocumentType
            {
                Id = 3,
                Name = "Opportunity Document",
                Code = "Opportunity_Document",
                IsActive = true
            });


        }




        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}

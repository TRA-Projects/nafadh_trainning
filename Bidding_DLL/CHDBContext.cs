
using Bidding_Models;
using Bidding_Models.DatabaseModel;
using Bidding_Models.DatabaseModel.SystemCatalog;
using Bidding_Models.DatabaseModel.JobseekerProfile;
using Bidding_Models.DatabaseModel.JobOpportunities;
using Bidding_Models.DatabaseModel.Credentials;
using Bidding_Models.DatabaseModel.Assessments;
using Bidding_Models.DatabaseModel.Interviews;
using Bidding_Models.DatabaseModel.IdentityAccess;
using Bidding_Models.DatabaseModel.Employment;
using Bidding_Models.DatabaseModel.Notifications;
using Bidding_DLL.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Reflection.Metadata;
using System.Xml;
//using System.Data.Entity;

namespace Bidding_DLL
{
    public class CHDBContext : IdentityDbContext<
        AppUser, Roles, string,
        IdentityUserClaim<string>, AppUserRoles, IdentityUserLogin<string>,
        RolesClaims, IdentityUserToken<string>>
    {
        //public class OptionBuild {

        //    public OptionBuild()
        //    {
        //        settings = new ContextConfigurations();
        //        opsBuilder = new DbContextOptionsBuilder<CHDBContext>();
        //        opsBuilder.UseSqlServer(settings.SQLConnectionString);
        //        dbOptions = opsBuilder.Options;
        //    }

        //    public DbContextOptionsBuilder<CHDBContext> opsBuilder { get; set; }
        //    public DbContextOptions<CHDBContext> dbOptions { get; set; }

        //    public ContextConfigurations settings { get; set; }
        //}


        //public static OptionBuild ops = new OptionBuild();


        public CHDBContext(DbContextOptions<CHDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ChatConversation>()
                .HasMany(p => p.Messages)
                .WithOne()
                .HasForeignKey(q => q.ChatConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ChatConversation>()
                .HasOne(p => p.Party1Profile)
                .WithMany()
                .HasForeignKey(q => q.Party1ProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ChatConversation>()
                .HasOne(p => p.Party2Profile)
                .WithMany()
                .HasForeignKey(q => q.Party2ProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ChatConversationMessage>()
                .HasOne(p => p.SenderProfile)
                .WithMany()
                .HasForeignKey(q => q.SenderProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ChatConversationMessage>()
                .HasOne(p => p.ReceiverProfile)
                .WithMany()
                .HasForeignKey(q => q.ReceiverProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Engagement>()
                .HasMany(p => p.Attachments)
                .WithOne()
                .HasForeignKey(q => q.EngagementId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define one-to-many relationships
            builder.Entity<ProfileProject>()
                .HasMany(p => p.RequiredDocuments)
                .WithOne()
                .HasForeignKey(q => q.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define one-to-many relationships
            builder.Entity<ProfileProject>()
                .HasMany(p => p.Questions)
                .WithOne(q => q.Project)
                .HasForeignKey(q => q.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define one-to-many relationships
            builder.Entity<ProfileProjectContract>()
                .HasOne(p => p.Project)
                .WithMany()
                .HasForeignKey(q => q.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProfileProject>()
                .HasMany(p => p.Attachments)
                .WithOne(a => a.Project)
                .HasForeignKey(a => a.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProfileProjectContract>()
           .HasOne(m => m.Project)
           .WithOne()
           .HasForeignKey<ProfileProjectContract>(m => m.ProjectId)
           .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProfileProject>()
                .HasOne(p => p.BudgetTimeline)
                .WithOne(b => b.Project)
                .HasForeignKey<BudgetTimeline>(b => b.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(builder);
            builder.Seed();

            ConfigureIdentityAccess(builder);

            builder.Entity<ProfileProjectMilestone>()
                .HasOne(m => m.Contract)
                .WithMany(c => c.Milestones)
                .HasForeignKey(m => m.ContractId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<ProfileProjectMilestone>()
                .HasMany(m => m.MilestoneOperations)
                .WithOne()
                .HasForeignKey(m => m.MilestoneId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProfileProjectMilestoneOperation>()
                .HasMany(m => m.Attachments)
                .WithOne()
                .HasForeignKey(m => m.ProfileProjectMilestoneOperationsId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProjectPhase>()
                .HasOne(p => p.Project)
                .WithMany(pp => pp.Phases)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Dispute>()
                .HasMany(pp => pp.ReponseAttachments)
                .WithOne()
                .HasForeignKey(p => p.DisputeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FavoriteProject>()
                .HasKey(fp => new { fp.ProfileId, fp.ProjectId });

            builder.Entity<FavoriteProject>()
                .HasOne(x => x.Profile)
                .WithMany()
                .HasForeignKey(x => x.ProfileId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<FavoriteProject>()
                .HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Article>()
                .HasOne(a => a.ArticleCategory)
                .WithMany()
                .HasForeignKey(a => a.ArticleCategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Article>()
                .HasOne(a => a.Profile)
                .WithMany()
                .HasForeignKey(a => a.ProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<LikedArticle>()
                .HasKey(fp => new { fp.ProfileId, fp.ArticleId });

            builder.Entity<LikedArticle>()
                .HasOne<Profile>()
                .WithMany()
                .HasForeignKey(x => x.ProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<LikedArticle>()
                .HasOne<Article>()
                .WithMany(l => l.Likes)
                .HasForeignKey(x => x.ArticleId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FavoriteArticle>()
               .HasKey(fp => new { fp.ProfileId, fp.ArticleId });

            builder.Entity<FavoriteArticle>()
                .HasOne<Profile>()
                .WithMany()
                .HasForeignKey(x => x.ProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FavoriteArticle>()
                .HasOne(x => x.Article)
                .WithMany(l => l.Favorites)
                .HasForeignKey(x => x.ArticleId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ArticleComment>()
                .HasOne<Article>()
                .WithMany(a => a.Comments)
                .HasForeignKey(a => a.ArticleId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ArticleComment>()
                .HasOne(x => x.Author)
                .WithMany()
                .HasForeignKey(a => a.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Payment>()
              .HasOne(x => x.Profile)
              .WithMany(l => l.Payments)
              .HasForeignKey(x => x.ProfileId)
              .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Payment>()
            .Property(e => e.CratedOn)
            .HasDefaultValueSql("GETDATE()");

            builder.Entity<Payment>()
             .HasOne(m => m.ProfileProjectMilestone)
             .WithMany(m => m.Payment)
             .HasForeignKey(m => m.RequestId)
             .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProfileRating>()
                .HasOne(m => m.Contract)
                .WithMany()
                .HasForeignKey(m => m.ContractId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProfileRating>()
                .HasOne(m => m.SenderProfile)
                .WithMany()
                .HasForeignKey(m => m.SenderProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProfileRating>()
                .HasOne(m => m.ReceiverProfile)
                .WithMany()
                .HasForeignKey(m => m.ReceiverProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FreelancerEndorsement>()
                .HasOne(m => m.SenderProfile)
                .WithMany()
                .HasForeignKey(m => m.SenderProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FreelancerEndorsement>()
                .HasOne(m => m.ReceiverProfile)
                .WithMany()
                .HasForeignKey(m => m.ReceiverProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FreelancerEndorsement>()
                .HasOne(m => m.Project)
                .WithOne()
                .HasForeignKey<FreelancerEndorsement>(m => m.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FreelancerEndorsement>()
                .HasMany(m => m.Skills)
                .WithOne(x => x.Endorsement)
                .HasForeignKey(m => m.FreelancerEndorsementId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<FreelancerEndorsementSkill>()
                .HasKey(fp => new { fp.FreelancerEndorsementId, fp.Skill });

            builder.Entity<FreelancerBankAccount>()
                .HasOne(m => m.Profile)
                .WithOne()
                .HasForeignKey<FreelancerBankAccount>(m => m.ProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FreelancerBankAccount>()
                .HasOne(m => m.Bank)
                .WithMany()
                .HasForeignKey(m => m.BankId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProfileProject>()
                .HasMany(m => m.AuditLogs)
                .WithOne()
                .HasForeignKey(m => m.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Engagement>()
                .HasMany(m => m.AuditLogs)
                .WithOne()
                .HasForeignKey(m => m.EngagementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProfileProjectContract>()
                .HasMany(m => m.AuditLogs)
                .WithOne()
                .HasForeignKey(m => m.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProviderFreelancer>()
               .HasOne(m => m.Providers)
               .WithMany(p => p.ProviderFreelancer)
               .HasForeignKey(m => m.ProviderId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Profile>()
               .HasOne(x=>x.CompanyInfo)
               .WithOne()
               .HasForeignKey<CompanyInfo>(m => m.ProfileId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<CompanyInfo>()
              .HasMany(m => m.Signatories)
              .WithOne()
              .HasForeignKey(m => m.CompanyInfoId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CompanyInfo>()
                .HasMany(m => m.DeclaredActivities)
                .WithOne()
                .HasForeignKey(m => m.CompanyInfoId);

            ConfigureSystemCatalog(builder);
            ConfigureJobseekerProfile(builder);
            ConfigureJobOpportunities(builder);
            ConfigureCredentials(builder);
            ConfigureInterviews(builder);
            ConfigureAssessments(builder);
            ConfigureEmployment(builder);
        }

        private static void ConfigureSystemCatalog(ModelBuilder builder)
        {
            builder.Entity<SystemVariable>()
                .HasIndex(v => v.VariableKey)
                .IsUnique();

            builder.Entity<SystemConfigGroup>()
                .HasIndex(g => g.Code)
                .IsUnique();

            builder.Entity<SystemConfigValue>()
                .HasIndex(v => new { v.ConfigGroupId, v.ValueKey })
                .IsUnique();

            builder.Entity<SystemConfigValue>()
                .HasOne(v => v.ConfigGroup)
                .WithMany(g => g.Values)
                .HasForeignKey(v => v.ConfigGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CatalogSkill>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Skills)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<NotificationTemplate>()
                .HasIndex(t => t.ActionCode)
                .IsUnique();

            builder.Entity<NotificationTransactionLog>()
                .HasIndex(t => t.CreatedAtUtc);

            builder.Entity<NotificationTransactionLog>()
                .HasIndex(t => new { t.ChannelType, t.CreatedAtUtc });

            builder.Entity<NotificationTransactionLog>()
                .HasIndex(t => t.Receiver);

            builder.Entity<NotificationTransactionLog>()
                .HasIndex(t => t.CustomNotificationId);

            builder.Entity<CustomNotification>()
                .HasIndex(n => new { n.Status, n.ScheduledAtUtc });

            builder.Entity<CustomNotificationTarget>()
                .HasOne(t => t.CustomNotification)
                .WithMany(n => n.Targets)
                .HasForeignKey(t => t.CustomNotificationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserNotification>()
                .HasOne(u => u.CustomNotification)
                .WithMany()
                .HasForeignKey(u => u.CustomNotificationId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<UserNotification>()
                .HasIndex(u => new { u.UserId, u.IsRead, u.CreatedAtUtc });

            builder.Entity<CatalogSkill>()
                .HasIndex(s => s.Kind);
        }

        private static void ConfigureJobseekerProfile(ModelBuilder builder)
        {
            builder.Entity<Attachment>()
                .HasOne(a => a.UploadedByProfile)
                .WithMany()
                .HasForeignKey(a => a.UploadedByProfileId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ProfileAttachment>()
                .HasOne(l => l.Profile)
                .WithMany()
                .HasForeignKey(l => l.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProfileAttachment>()
                .HasOne(l => l.Attachment)
                .WithMany()
                .HasForeignKey(l => l.AttachmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProfileAttachment>()
                .HasIndex(l => new { l.ProfileId, l.Purpose, l.IsPrimary })
                .IsUnique()
                .HasFilter("[IsPrimary] = 1");

            builder.Entity<ProfileWorkScope>()
                .HasOne(w => w.Profile)
                .WithMany()
                .HasForeignKey(w => w.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProfileWorkScope>()
                .HasIndex(w => new { w.ProfileId, w.WorkScopeId })
                .IsUnique();

            builder.Entity<ProfileMolSummary>()
                .HasIndex(s => s.ProfileId)
                .IsUnique();

            builder.Entity<ProfileMolSummary>()
                .HasOne(s => s.Profile)
                .WithMany()
                .HasForeignKey(s => s.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerEducationSummary>()
                .HasIndex(s => s.ProfileId)
                .IsUnique();

            builder.Entity<JobSeekerEducationSummary>()
                .HasOne(s => s.Profile)
                .WithMany()
                .HasForeignKey(s => s.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerDiplomaMarksVerification>()
                .HasIndex(s => s.ProfileId)
                .IsUnique();

            builder.Entity<JobSeekerDiplomaMarksVerification>()
                .HasOne(s => s.Profile)
                .WithMany()
                .HasForeignKey(s => s.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerEducation>()
                .HasOne(e => e.Profile)
                .WithMany()
                .HasForeignKey(e => e.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerEducationAttachment>()
                .HasOne(l => l.JobSeekerEducation)
                .WithMany(e => e.Attachments)
                .HasForeignKey(l => l.JobSeekerEducationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerEducationAttachment>()
                .HasOne(l => l.Attachment)
                .WithMany()
                .HasForeignKey(l => l.AttachmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobSeekerExperience>()
                .HasOne(e => e.Profile)
                .WithMany()
                .HasForeignKey(e => e.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerExperienceAttachment>()
                .HasOne(l => l.JobSeekerExperience)
                .WithMany(e => e.Attachments)
                .HasForeignKey(l => l.JobSeekerExperienceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerExperienceAttachment>()
                .HasOne(l => l.Attachment)
                .WithMany()
                .HasForeignKey(l => l.AttachmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobSeekerExperienceSkill>()
                .HasIndex(x => new { x.JobSeekerExperienceId, x.CatalogSkillId })
                .IsUnique();

            builder.Entity<JobSeekerExperienceSkill>()
                .HasOne(x => x.JobSeekerExperience)
                .WithMany(e => e.Skills)
                .HasForeignKey(x => x.JobSeekerExperienceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerExperienceSkill>()
                .HasOne(x => x.CatalogSkill)
                .WithMany()
                .HasForeignKey(x => x.CatalogSkillId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobSeekerExperienceTechStack>()
                .HasIndex(x => new { x.JobSeekerExperienceId, x.CatalogTechStackId })
                .IsUnique();

            builder.Entity<JobSeekerExperienceTechStack>()
                .HasOne(x => x.JobSeekerExperience)
                .WithMany(e => e.TechStacks)
                .HasForeignKey(x => x.JobSeekerExperienceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerExperienceTechStack>()
                .HasOne(x => x.CatalogTechStack)
                .WithMany()
                .HasForeignKey(x => x.CatalogTechStackId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobSeekerCertificate>()
                .HasOne(e => e.Profile)
                .WithMany()
                .HasForeignKey(e => e.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerCertificateAttachment>()
                .HasOne(l => l.JobSeekerCertificate)
                .WithMany(e => e.Attachments)
                .HasForeignKey(l => l.JobSeekerCertificateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerCertificateAttachment>()
                .HasOne(l => l.Attachment)
                .WithMany()
                .HasForeignKey(l => l.AttachmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobSeekerTraining>()
                .HasOne(e => e.Profile)
                .WithMany()
                .HasForeignKey(e => e.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerTrainingAttachment>()
                .HasOne(l => l.JobSeekerTraining)
                .WithMany(e => e.Attachments)
                .HasForeignKey(l => l.JobSeekerTrainingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerTrainingAttachment>()
                .HasOne(l => l.Attachment)
                .WithMany()
                .HasForeignKey(l => l.AttachmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobSeekerTrainingSkill>()
                .HasIndex(x => new { x.JobSeekerTrainingId, x.CatalogSkillId })
                .IsUnique();

            builder.Entity<JobSeekerTrainingSkill>()
                .HasOne(x => x.JobSeekerTraining)
                .WithMany(t => t.Skills)
                .HasForeignKey(x => x.JobSeekerTrainingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerTrainingSkill>()
                .HasOne(x => x.CatalogSkill)
                .WithMany()
                .HasForeignKey(x => x.CatalogSkillId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobSeekerTrainingTechStack>()
                .HasIndex(x => new { x.JobSeekerTrainingId, x.CatalogTechStackId })
                .IsUnique();

            builder.Entity<JobSeekerTrainingTechStack>()
                .HasOne(x => x.JobSeekerTraining)
                .WithMany(t => t.TechStacks)
                .HasForeignKey(x => x.JobSeekerTrainingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerTrainingTechStack>()
                .HasOne(x => x.CatalogTechStack)
                .WithMany()
                .HasForeignKey(x => x.CatalogTechStackId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobSeekerPortfolioItem>()
                .HasOne(e => e.Profile)
                .WithMany()
                .HasForeignKey(e => e.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerPortfolioItemAttachment>()
                .HasOne(l => l.JobSeekerPortfolioItem)
                .WithMany(e => e.Attachments)
                .HasForeignKey(l => l.JobSeekerPortfolioItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerPortfolioItemAttachment>()
                .HasOne(l => l.Attachment)
                .WithMany()
                .HasForeignKey(l => l.AttachmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobSeekerPortfolioItemSkill>()
                .HasIndex(x => new { x.JobSeekerPortfolioItemId, x.CatalogSkillId })
                .IsUnique();

            builder.Entity<JobSeekerPortfolioItemSkill>()
                .HasOne(x => x.JobSeekerPortfolioItem)
                .WithMany(p => p.Skills)
                .HasForeignKey(x => x.JobSeekerPortfolioItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerPortfolioItemSkill>()
                .HasOne(x => x.CatalogSkill)
                .WithMany()
                .HasForeignKey(x => x.CatalogSkillId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobSeekerPortfolioItemTechStack>()
                .HasIndex(x => new { x.JobSeekerPortfolioItemId, x.CatalogTechStackId })
                .IsUnique();

            builder.Entity<JobSeekerPortfolioItemTechStack>()
                .HasOne(x => x.JobSeekerPortfolioItem)
                .WithMany(p => p.TechStacks)
                .HasForeignKey(x => x.JobSeekerPortfolioItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerPortfolioItemTechStack>()
                .HasOne(x => x.CatalogTechStack)
                .WithMany()
                .HasForeignKey(x => x.CatalogTechStackId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureJobOpportunities(ModelBuilder builder)
        {
            builder.Entity<JobOpportunity>()
                .HasIndex(o => o.JobReference)
                .IsUnique();

            builder.Entity<JobOpportunity>()
                .HasOne(o => o.OwnerProfile)
                .WithMany()
                .HasForeignKey(o => o.OwnerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobOpportunity>()
                .Property(o => o.TargetGenderValueKey)
                .HasMaxLength(64);

            builder.Entity<JobOpportunity>()
                .Property(o => o.InterviewModeValueKey)
                .HasMaxLength(64);

            builder.Entity<JobOpportunityApplicationMethod>()
                .HasIndex(x => new { x.JobOpportunityId, x.ApplicationMethodValueKey })
                .IsUnique();

            builder.Entity<JobOpportunityApplicationMethod>()
                .HasOne(x => x.JobOpportunity)
                .WithMany(o => o.ApplicationMethods)
                .HasForeignKey(x => x.JobOpportunityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityLocation>()
                .HasIndex(x => x.JobOpportunityId)
                .IsUnique();

            builder.Entity<JobOpportunityLocation>()
                .HasOne<JobOpportunity>()
                .WithMany()
                .HasForeignKey(x => x.JobOpportunityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityCompensation>()
                .HasIndex(x => x.JobOpportunityId)
                .IsUnique();

            builder.Entity<JobOpportunityCompensation>()
                .HasOne<JobOpportunity>()
                .WithMany()
                .HasForeignKey(x => x.JobOpportunityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityCompensationBenefit>()
                .HasIndex(x => new { x.JobOpportunityCompensationId, x.BenefitValueKey })
                .IsUnique();

            builder.Entity<JobOpportunityCompensationBenefit>()
                .HasOne(x => x.Compensation)
                .WithMany(c => c.Benefits)
                .HasForeignKey(x => x.JobOpportunityCompensationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityRequirement>()
                .HasIndex(x => x.JobOpportunityId)
                .IsUnique();

            builder.Entity<JobOpportunityRequirement>()
                .HasOne(x => x.JobOpportunity)
                .WithMany()
                .HasForeignKey(x => x.JobOpportunityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityRequirementSkill>()
                .HasIndex(x => new { x.JobOpportunityRequirementId, x.CatalogSkillId, x.IsSecondary })
                .IsUnique();

            builder.Entity<JobOpportunityRequirementSkill>()
                .HasOne(x => x.Requirement)
                .WithMany(r => r.Skills)
                .HasForeignKey(x => x.JobOpportunityRequirementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityRequirementSkill>()
                .HasOne(x => x.CatalogSkill)
                .WithMany()
                .HasForeignKey(x => x.CatalogSkillId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobOpportunityRequirementTechStack>()
                .HasIndex(x => new { x.JobOpportunityRequirementId, x.CatalogTechStackId })
                .IsUnique();

            builder.Entity<JobOpportunityRequirementTechStack>()
                .HasOne(x => x.Requirement)
                .WithMany(r => r.TechStacks)
                .HasForeignKey(x => x.JobOpportunityRequirementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityRequirementTechStack>()
                .HasOne(x => x.CatalogTechStack)
                .WithMany()
                .HasForeignKey(x => x.CatalogTechStackId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobOpportunityRequirementFieldOfStudy>()
                .HasIndex(x => new { x.JobOpportunityRequirementId, x.FieldOfStudyValueKey })
                .IsUnique();

            builder.Entity<JobOpportunityRequirementFieldOfStudy>()
                .HasOne(x => x.Requirement)
                .WithMany(r => r.FieldOfStudies)
                .HasForeignKey(x => x.JobOpportunityRequirementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityRequirementLanguage>()
                .HasIndex(x => new { x.JobOpportunityRequirementId, x.LanguageValueKey })
                .IsUnique();

            builder.Entity<JobOpportunityRequirementLanguage>()
                .HasOne(x => x.Requirement)
                .WithMany(r => r.Languages)
                .HasForeignKey(x => x.JobOpportunityRequirementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityRequirementCertification>()
                .HasOne(x => x.Requirement)
                .WithMany(r => r.Certifications)
                .HasForeignKey(x => x.JobOpportunityRequirementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityRequiredDocument>()
                .HasOne(x => x.JobOpportunity)
                .WithMany()
                .HasForeignKey(x => x.JobOpportunityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityScreeningQuestion>()
                .HasOne(x => x.JobOpportunity)
                .WithMany()
                .HasForeignKey(x => x.JobOpportunityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityScreeningQuestionOption>()
                .HasOne(x => x.ScreeningQuestion)
                .WithMany(q => q.Options)
                .HasForeignKey(x => x.ScreeningQuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityAttachment>()
                .HasOne(x => x.JobOpportunity)
                .WithMany(o => o.Attachments)
                .HasForeignKey(x => x.JobOpportunityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityAttachment>()
                .HasOne(x => x.Attachment)
                .WithMany()
                .HasForeignKey(x => x.AttachmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobOpportunityParticipation>()
                .HasIndex(x => new { x.ProfileId, x.JobOpportunityId })
                .IsUnique();

            builder.Entity<JobOpportunityParticipation>()
                .HasOne(x => x.Profile)
                .WithMany()
                .HasForeignKey(x => x.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobOpportunityParticipation>()
                .HasOne(x => x.JobOpportunity)
                .WithMany(o => o.Participations)
                .HasForeignKey(x => x.JobOpportunityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityParticipationAnswer>()
                .HasOne(x => x.Participation)
                .WithMany(p => p.Answers)
                .HasForeignKey(x => x.ParticipationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityParticipationAnswer>()
                .HasOne<JobOpportunityScreeningQuestion>()
                .WithMany()
                .HasForeignKey(x => x.ScreeningQuestionId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.Entity<JobOpportunityParticipationAttachment>()
                .HasOne(x => x.Participation)
                .WithMany(p => p.Attachments)
                .HasForeignKey(x => x.ParticipationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobOpportunityParticipationAttachment>()
                .HasOne(x => x.Attachment)
                .WithMany()
                .HasForeignKey(x => x.AttachmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobOpportunityParticipationAttachment>()
                .HasOne(x => x.RequiredDocument)
                .WithMany()
                .HasForeignKey(x => x.RequiredDocumentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobSeekerSavedOpportunity>()
                .HasIndex(x => new { x.ProfileId, x.JobOpportunityId })
                .IsUnique();

            builder.Entity<JobSeekerSavedOpportunity>()
                .HasOne(x => x.Profile)
                .WithMany()
                .HasForeignKey(x => x.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerSavedOpportunity>()
                .HasOne(x => x.JobOpportunity)
                .WithMany()
                .HasForeignKey(x => x.JobOpportunityId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureCredentials(ModelBuilder builder)
        {
            builder.Entity<CredentialDefinition>()
                .HasIndex(d => d.Reference)
                .IsUnique();

            builder.Entity<CredentialDefinitionSkill>()
                .HasIndex(x => new { x.CredentialDefinitionId, x.CatalogSkillId })
                .IsUnique();

            builder.Entity<CredentialDefinitionSkill>()
                .HasOne(x => x.CredentialDefinition)
                .WithMany(d => d.Skills)
                .HasForeignKey(x => x.CredentialDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CredentialDefinitionSkill>()
                .HasOne(x => x.CatalogSkill)
                .WithMany()
                .HasForeignKey(x => x.CatalogSkillId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CredentialDefinitionBasicSkill>()
                .HasIndex(x => new { x.CredentialDefinitionId, x.CatalogBasicSkillId })
                .IsUnique();

            builder.Entity<CredentialDefinitionBasicSkill>()
                .HasOne(x => x.CredentialDefinition)
                .WithMany(d => d.BasicSkills)
                .HasForeignKey(x => x.CredentialDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CredentialDefinitionBasicSkill>()
                .HasOne(x => x.CatalogBasicSkill)
                .WithMany()
                .HasForeignKey(x => x.CatalogBasicSkillId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CredentialDefinitionTechStack>()
                .HasIndex(x => new { x.CredentialDefinitionId, x.CatalogTechStackId })
                .IsUnique();

            builder.Entity<CredentialDefinitionTechStack>()
                .HasOne(x => x.CredentialDefinition)
                .WithMany(d => d.TechStacks)
                .HasForeignKey(x => x.CredentialDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CredentialDefinitionTechStack>()
                .HasOne(x => x.CatalogTechStack)
                .WithMany()
                .HasForeignKey(x => x.CatalogTechStackId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CredentialEnrollment>()
                .HasIndex(e => e.Reference)
                .IsUnique();

            builder.Entity<CredentialEnrollment>()
                .HasIndex(e => new { e.CredentialDefinitionId, e.ProfileId })
                .IsUnique();

            builder.Entity<CredentialEnrollment>()
                .HasOne(e => e.CredentialDefinition)
                .WithMany(d => d.Enrollments)
                .HasForeignKey(e => e.CredentialDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CredentialEnrollment>()
                .HasOne(e => e.Profile)
                .WithMany()
                .HasForeignKey(e => e.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureInterviews(ModelBuilder builder)
        {
            builder.Entity<InterviewRecord>()
                .HasIndex(r => r.Reference)
                .IsUnique();

            builder.Entity<InterviewRecordSkill>()
                .HasIndex(x => new { x.InterviewRecordId, x.CatalogSkillId })
                .IsUnique();

            builder.Entity<InterviewRecordSkill>()
                .HasOne(x => x.InterviewRecord)
                .WithMany(r => r.Skills)
                .HasForeignKey(x => x.InterviewRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<InterviewRecordSkill>()
                .HasOne(x => x.CatalogSkill)
                .WithMany()
                .HasForeignKey(x => x.CatalogSkillId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InterviewRecordBasicSkill>()
                .HasIndex(x => new { x.InterviewRecordId, x.CatalogBasicSkillId })
                .IsUnique();

            builder.Entity<InterviewRecordBasicSkill>()
                .HasOne(x => x.InterviewRecord)
                .WithMany(r => r.BasicSkills)
                .HasForeignKey(x => x.InterviewRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<InterviewRecordBasicSkill>()
                .HasOne(x => x.CatalogBasicSkill)
                .WithMany()
                .HasForeignKey(x => x.CatalogBasicSkillId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InterviewRecordTechStack>()
                .HasIndex(x => new { x.InterviewRecordId, x.CatalogTechStackId })
                .IsUnique();

            builder.Entity<InterviewRecordTechStack>()
                .HasOne(x => x.InterviewRecord)
                .WithMany(r => r.TechStacks)
                .HasForeignKey(x => x.InterviewRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<InterviewRecordTechStack>()
                .HasOne(x => x.CatalogTechStack)
                .WithMany()
                .HasForeignKey(x => x.CatalogTechStackId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InterviewEnrollment>()
                .HasIndex(e => e.Reference)
                .IsUnique();

            // One seat per profile per attempt (round). Reschedule closes round N and opens N+1.
            builder.Entity<InterviewEnrollment>()
                .HasIndex(e => new { e.InterviewRecordId, e.ProfileId, e.RoundNumber })
                .IsUnique();

            builder.Entity<InterviewEnrollment>()
                .Property(e => e.RoundNumber)
                .HasDefaultValue(1);

            builder.Entity<InterviewEnrollment>()
                .HasOne(e => e.InterviewRecord)
                .WithMany(r => r.Enrollments)
                .HasForeignKey(e => e.InterviewRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<InterviewEnrollment>()
                .HasOne(e => e.Profile)
                .WithMany()
                .HasForeignKey(e => e.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureAssessments(ModelBuilder builder)
        {
            builder.Entity<AssessmentResult>()
                .HasIndex(r => r.CredentialEnrollmentId)
                .IsUnique()
                .HasFilter("[CredentialEnrollmentId] IS NOT NULL");

            builder.Entity<AssessmentResult>()
                .HasIndex(r => new { r.InterviewRecordId, r.ProfileId })
                .IsUnique()
                .HasFilter("[InterviewRecordId] IS NOT NULL");

            builder.Entity<AssessmentResult>()
                .HasOne(r => r.Profile)
                .WithMany()
                .HasForeignKey(r => r.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AssessmentResult>()
                .HasOne(r => r.CredentialEnrollment)
                .WithMany()
                .HasForeignKey(r => r.CredentialEnrollmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AssessmentResult>()
                .HasOne(r => r.InterviewRecord)
                .WithMany()
                .HasForeignKey(r => r.InterviewRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AssessmentResultBehavior>()
                .HasIndex(b => new { b.AssessmentResultId, b.ValueKey })
                .IsUnique();

            builder.Entity<AssessmentResultBehavior>()
                .HasOne(b => b.AssessmentResult)
                .WithMany(r => r.Behaviors)
                .HasForeignKey(b => b.AssessmentResultId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AssessmentRejectedOffer>()
                .HasIndex(r => r.CredentialEnrollmentId);

            builder.Entity<AssessmentRejectedOffer>()
                .HasOne(r => r.CredentialEnrollment)
                .WithMany()
                .HasForeignKey(r => r.CredentialEnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AssessmentRejectedOffer>()
                .HasOne(r => r.Profile)
                .WithMany()
                .HasForeignKey(r => r.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AssessmentProfessionalSkillRating>()
                .HasIndex(x => new { x.AssessmentResultId, x.CatalogSkillId })
                .IsUnique();

            builder.Entity<AssessmentProfessionalSkillRating>()
                .HasOne(x => x.AssessmentResult)
                .WithMany(r => r.ProfessionalSkillRatings)
                .HasForeignKey(x => x.AssessmentResultId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AssessmentProfessionalSkillRating>()
                .HasOne(x => x.CatalogSkill)
                .WithMany()
                .HasForeignKey(x => x.CatalogSkillId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AssessmentProfessionalSkillRating>()
                .HasOne(x => x.InterviewRecordSkill)
                .WithMany()
                .HasForeignKey(x => x.InterviewRecordSkillId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AssessmentProfessionalSkillRating>()
                .HasOne(x => x.CredentialDefinitionSkill)
                .WithMany()
                .HasForeignKey(x => x.CredentialDefinitionSkillId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AssessmentBasicSkillRating>()
                .HasIndex(x => new { x.AssessmentResultId, x.CatalogBasicSkillId })
                .IsUnique();

            builder.Entity<AssessmentBasicSkillRating>()
                .HasOne(x => x.AssessmentResult)
                .WithMany(r => r.BasicSkillRatings)
                .HasForeignKey(x => x.AssessmentResultId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AssessmentBasicSkillRating>()
                .HasOne(x => x.CatalogBasicSkill)
                .WithMany()
                .HasForeignKey(x => x.CatalogBasicSkillId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AssessmentBasicSkillRating>()
                .HasOne(x => x.InterviewRecordBasicSkill)
                .WithMany()
                .HasForeignKey(x => x.InterviewRecordBasicSkillId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AssessmentBasicSkillRating>()
                .HasOne(x => x.CredentialDefinitionBasicSkill)
                .WithMany()
                .HasForeignKey(x => x.CredentialDefinitionBasicSkillId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AssessmentTechStackSkillRating>()
                .HasIndex(x => new { x.AssessmentResultId, x.CatalogTechStackId })
                .IsUnique();

            builder.Entity<AssessmentTechStackSkillRating>()
                .HasOne(x => x.AssessmentResult)
                .WithMany(r => r.TechStackSkillRatings)
                .HasForeignKey(x => x.AssessmentResultId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AssessmentTechStackSkillRating>()
                .HasOne(x => x.CatalogTechStack)
                .WithMany()
                .HasForeignKey(x => x.CatalogTechStackId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AssessmentTechStackSkillRating>()
                .HasOne(x => x.InterviewRecordTechStack)
                .WithMany()
                .HasForeignKey(x => x.InterviewRecordTechStackId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AssessmentTechStackSkillRating>()
                .HasOne(x => x.CredentialDefinitionTechStack)
                .WithMany()
                .HasForeignKey(x => x.CredentialDefinitionTechStackId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<JobSeekerSkillSummary>()
                .HasIndex(x => new
                {
                    x.ProfileId,
                    x.SkillKind,
                    x.CatalogSkillId,
                    x.CatalogBasicSkillId,
                    x.CatalogTechStackId,
                })
                .IsUnique();

            builder.Entity<JobSeekerSkillSummary>()
                .HasOne(x => x.Profile)
                .WithMany()
                .HasForeignKey(x => x.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobSeekerSkillSummary>()
                .HasOne(x => x.LastAssessmentResult)
                .WithMany()
                .HasForeignKey(x => x.LastAssessmentResultId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        private static void ConfigureEmployment(ModelBuilder builder)
        {
            builder.Entity<EmploymentProcedureRecord>()
                .HasIndex(r => r.Reference)
                .IsUnique();

            builder.Entity<EmploymentProcedureRecord>()
                .HasIndex(r => new { r.InterviewRecordId, r.ProfileId })
                .IsUnique()
                .HasFilter("[InterviewRecordId] IS NOT NULL");

            builder.Entity<EmploymentProcedureRecord>()
                .HasOne(r => r.Profile)
                .WithMany()
                .HasForeignKey(r => r.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EmploymentProcedureRecord>()
                .HasOne(r => r.JobOpportunity)
                .WithMany()
                .HasForeignKey(r => r.JobOpportunityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EmploymentProcedureRecord>()
                .HasOne(r => r.InterviewRecord)
                .WithMany()
                .HasForeignKey(r => r.InterviewRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EmploymentProcedureRecord>()
                .HasOne(r => r.OfferAttachment)
                .WithMany()
                .HasForeignKey(r => r.OfferAttachmentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<EmploymentProcedureRecord>()
                .HasOne(r => r.ContractAttachment)
                .WithMany()
                .HasForeignKey(r => r.ContractAttachmentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<EmploymentProcedureRecord>()
                .HasIndex(r => r.Phase);

            builder.Entity<ContractWageLine>()
                .HasIndex(x => new { x.ContractId, x.AllowanceValueKey })
                .IsUnique();

            builder.Entity<ContractWageLine>()
                .HasOne(x => x.Contract)
                .WithMany(r => r.WageLines)
                .HasForeignKey(x => x.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ContractBenefitLine>()
                .HasIndex(x => new { x.ContractId, x.BenefitValueKey })
                .IsUnique();

            builder.Entity<ContractBenefitLine>()
                .HasOne(x => x.Contract)
                .WithMany(r => r.BenefitLines)
                .HasForeignKey(x => x.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EmploymentAcceptanceDocument>()
                .HasOne(x => x.EmploymentProcedureRecord)
                .WithMany(r => r.AcceptanceDocuments)
                .HasForeignKey(x => x.EmploymentProcedureRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EmploymentAcceptanceDocument>()
                .HasOne(x => x.Attachment)
                .WithMany()
                .HasForeignKey(x => x.AttachmentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<EmploymentInquiryMessage>()
                .HasOne(x => x.EmploymentProcedureRecord)
                .WithMany(r => r.InquiryMessages)
                .HasForeignKey(x => x.EmploymentProcedureRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EmploymentInquiryMessageAttachment>()
                .HasOne(x => x.EmploymentInquiryMessage)
                .WithMany(m => m.Attachments)
                .HasForeignKey(x => x.EmploymentInquiryMessageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EmploymentInquiryMessageAttachment>()
                .HasOne(x => x.Attachment)
                .WithMany()
                .HasForeignKey(x => x.AttachmentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PostEmploymentRecord>()
                .HasIndex(r => r.Reference)
                .IsUnique();

            builder.Entity<PostEmploymentRecord>()
                .HasIndex(r => r.EmploymentProcedureRecordId)
                .IsUnique();

            builder.Entity<PostEmploymentRecord>()
                .HasOne(r => r.EmploymentProcedureRecord)
                .WithOne(ep => ep.PostEmploymentRecord)
                .HasForeignKey<PostEmploymentRecord>(r => r.EmploymentProcedureRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PostEmploymentRecord>()
                .HasOne(r => r.Profile)
                .WithMany()
                .HasForeignKey(r => r.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WithdrawalRequest>()
                .HasIndex(r => r.Reference)
                .IsUnique();

            builder.Entity<WithdrawalRequest>()
                .HasOne(r => r.Profile)
                .WithMany()
                .HasForeignKey(r => r.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WithdrawalRequest>()
                .HasOne(r => r.EmploymentProcedureRecord)
                .WithMany(ep => ep.WithdrawalRequests)
                .HasForeignKey(r => r.EmploymentProcedureRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WithdrawalRequest>()
                .HasOne(r => r.PostEmploymentRecord)
                .WithMany(pep => pep.WithdrawalRequests)
                .HasForeignKey(r => r.PostEmploymentRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WithdrawalRequestAttachment>()
                .HasOne(x => x.WithdrawalRequest)
                .WithMany(r => r.Attachments)
                .HasForeignKey(x => x.WithdrawalRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WithdrawalRequestAttachment>()
                .HasOne(x => x.Attachment)
                .WithMany()
                .HasForeignKey(x => x.AttachmentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<WithdrawalRequestOperatorResponse>()
                .HasIndex(x => x.WithdrawalRequestId)
                .IsUnique();

            builder.Entity<WithdrawalRequestOperatorResponse>()
                .HasOne(x => x.WithdrawalRequest)
                .WithOne(r => r.OperatorResponse)
                .HasForeignKey<WithdrawalRequestOperatorResponse>(x => x.WithdrawalRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WithdrawalRequestOperatorResponse>()
                .HasOne(x => x.RespondedByOperatorProfile)
                .WithMany()
                .HasForeignKey(x => x.RespondedByOperatorProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<EmploymentContractChangeHistory>()
                .HasOne(x => x.PostEmploymentRecord)
                .WithMany(r => r.ContractChangeHistory)
                .HasForeignKey(x => x.PostEmploymentRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EmploymentContractChangeHistory>()
                .HasOne(x => x.EmploymentProcedureRecord)
                .WithMany()
                .HasForeignKey(x => x.EmploymentProcedureRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EmploymentContractChangeHistory>()
                .HasOne(x => x.Profile)
                .WithMany()
                .HasForeignKey(x => x.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PostEmploymentEvaluation>()
                .HasIndex(e => e.PostEmploymentRecordId)
                .IsUnique();

            builder.Entity<PostEmploymentEvaluation>()
                .HasOne(e => e.PostEmploymentRecord)
                .WithOne(r => r.Evaluation)
                .HasForeignKey<PostEmploymentEvaluation>(e => e.PostEmploymentRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PostEmploymentEvaluationSkillRating>()
                .HasOne(s => s.PostEmploymentEvaluation)
                .WithMany(e => e.SkillRatings)
                .HasForeignKey(s => s.PostEmploymentEvaluationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PostEmploymentPeriodicEvaluation>()
                .HasOne(e => e.PostEmploymentRecord)
                .WithMany(r => r.PeriodicEvaluations)
                .HasForeignKey(e => e.PostEmploymentRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PostEmploymentPeriodicEvaluation>()
                .HasOne(e => e.AssessmentResult)
                .WithMany()
                .HasForeignKey(e => e.AssessmentResultId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<UserRefreshToken> UserRefreshToken { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ContactUsMessage> ContactUsMessages { get; set; }
        public DbSet<TendorCategory> TendorCategories { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestQuestion> RequestQuestions { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<BidDocument> BidDocuments { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<ProfileSetting> ProfileSettings { get; set; }
        public DbSet<ProfileSocial> ProfileSocials { get; set; }
        public DbSet<ProfileEducation> ProfileEducations { get; set; }
        public DbSet<ProfileWorkExperience> ProfileWorkExperiences { get; set; }
        public DbSet<ProfileCertificate> ProfileCertificates { get; set; }
        public DbSet<ProfileSkill> ProfileSkills { get; set; }
        public DbSet<ProfilePortfolio> ProfilePortfolios { get; set; }
        public DbSet<ProjectPhase> ProjectPhases { get; set; }
        public DbSet<ProfileProject> ProfileProjects { get; set; }
        public DbSet<ProfileProjectAudit> ProfileProjectAudits { get; set; }
        public DbSet<EngagementAudit> EngagementAudits { get; set; }
        public DbSet<ContractAudit> contractAudits { get; set; }
        public DbSet<BudgetTimeline> BudgetTimelines { get; set; }
        public DbSet<ProjectQuestion> ProjectQuestions { get; set; }
        public DbSet<ProjectRequiredDocument> ProjectRequiredDocument { get; set; }
        public DbSet<ProfileProjectContract> ProfileProjectContracts { get; set; }
        public DbSet<ProfileProjectMilestone> ProfileProjectMilestones { get; set; }
        public DbSet<ProfileProjectMilestoneOperation> ProfileProjectMilestoneOperations { get; set; }
        public DbSet<MilestoneAttachment> MilestoneAttachments { get; set; }
        public DbSet<ProfileRating> ProfileRatings { get; set; }
        public DbSet<SignedContract> SignedContracts { get; set; }
        public DbSet<ProjectQuestionAnswer> ProjectQuestionAnswers { get; set; }
        public DbSet<ProjectAttachment> ProjectAttachments { get; set; }
        public DbSet<Engagement> Engagements { get; set; }
        public DbSet<EngagementAttachment> EngagementAttachments { get; set; }
        public DbSet<VendorCategories> VendorCategories { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ProfileDocument> ProfileDocuments { get; set; }
        public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<VendorSavedRequest> VendorSavedRequest { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<NotificationTypeSetting> NotificationTypeSettings { get; set; }
        public DbSet<ProfileVerification> ProfileVerifications { get; set; }
        public DbSet<ActivityDivision> ActivityDivisions { get; set; }
        public DbSet<ActivityGroup> ActivityGroups { get; set; }
        public DbSet<ActivityClass> ActivityClasses { get; set; }
        public DbSet<ActivityLookup> Activities { get; set; }
        public DbSet<Governorate> Governorates { get; set; }
        public DbSet<Wilayat> Wilayats { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<ClaimMaster> ClaimMaster { get; set; }
        public DbSet<SMSEmailNotificationParam> SMSEmailNotificationParams { get; set; }
        public DbSet<SMSEmailNotification> SMSEmailNotifications { get; set; }
        public DbSet<BidQuotation> BidQuotations { get; set; }
        public DbSet<Bidding> Bidding { get; set; }
        public DbSet<VendorBidding> VendorBidding { get; set; }
        public DbSet<DeclaredActivity> DeclaredActivities { get; set; }
        public DbSet<RequestEvaluationCriteria> RequestEvaluationCriteria { get; set; }
        public DbSet<RequestRequiredDocument> RequestRequiredDocuments { get; set; }
        public DbSet<RequestPrerequiste> RequestPrerequistes { get; set; }
        public DbSet<RequestClarificationQuestion> RequestClarificationQuestions { get; set; }
        public DbSet<BidEvaluation> BidEvaluation { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Certificate> Certificates { get; set; }

        public DbSet<Staff> Staffs { get; set; }

        public DbSet<DocumentType> DocumentType { get; set; }
        public DbSet<OperatorUser> OperatorUsers { get; set; }

        public DbSet<UserTraining> UserTrainings { get; set; }


        public DbSet<RequestViewHistory> RequestViewHistory { get; set; }

        public DbSet<Department> Departments { get; set; }
        public DbSet<ProposalClarification> ProposalClarification { get; set; }

        public DbSet<ProposalClarificationDocuments> ProposalClarificationDocuments { get; set; }

        public DbSet<VendorExtensionRequest> VendorExtensionRequest { get; set; }
        public DbSet<TRATraining> TRATraining { get; set; }

        public DbSet<RequestParticipents> RequestParticipents { get; set; }

        public DbSet<NDA> NDA { get; set; }

        public DbSet<NDADocument> NDADocument { get; set; }

        public DbSet<OpportunityDocumentType> OpportunityDocumentType { get; set; }

        public DbSet<TrainingMaster> TrainingMaster { get; set; }

        //public DbSet<CompanyOverview> CompanyOverview { get; set; }

        public DbSet<Skills> Skills { get; set; }

        public DbSet<UserSkills> UserSkills { get; set; }
        public DbSet<ProfileApprovalHistory> ProfileApprovalHistory { get; set; }

        public DbSet<Specalization> Specalization { get; set; }
        public DbSet<UserSpecalization> UserSpecalization { get; set; }

        public DbSet<AccountDetails> AccountDetails { get; set; }

        public DbSet<Bank> Banks { get; set; }

        public DbSet<Banner> Banners { get; set; }

        public DbSet<Advertisement> advertisements { get; set; }
        public DbSet<Dispute> Disputes { get; set; }
        public DbSet<ChatConversation> ChatConversations { get; set; }
        public DbSet<ChatConversationMessage> ChatConversationMessages { get; set; }
        public DbSet<FavoriteProject> FavoriteProjects { get; set; }
        public DbSet<ArticleCategory> ArticleCategories { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<LikedArticle> LikedArticles { get; set; }
        public DbSet<FavoriteArticle> FavoriteArticles { get; set; }
        public DbSet<ArticleComment> ArticleComments { get; set; }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<ProfileVideo> ProfileVideos { get; set; }
        public DbSet<FreelancerBankAccount> FreelancerBankAccounts { get; set; }
        public DbSet<FreelancerEndorsement> FreelancerEndorsements { get; set; }
        public DbSet<FreelancerEndorsementSkill> FreelancerEndorsementSkills { get; set; }
        public DbSet<Providers> Providers { get; set; }
        public DbSet<ProviderFreelancer> ProviderFreelancer { get; set; }
        public DbSet<CompanyInfo> CompanyInfo { get; set; }
        public DbSet<PaymentsTransactions> PaymentsTransactions { get; set; }
        public DbSet<SubscriptionPayments> SubscriptionPayments { get; set; }
        public DbSet<SubscriptionPackagesLog> SubscriptionPackagesLog { get; set; }
        public DbSet<Subscriptions> Subscriptions { get; set; }
        public DbSet<SubscriptionPackages> SubscriptionPackages { get; set; }

        public DbSet<SystemVariable> SystemVariables { get; set; }
        public DbSet<SystemConfigGroup> SystemConfigGroups { get; set; }
        public DbSet<SystemConfigValue> SystemConfigValues { get; set; }
        public DbSet<CatalogSkillCategory> CatalogSkillCategories { get; set; }
        public DbSet<CatalogSkill> CatalogSkills { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<NotificationTransactionLog> NotificationTransactionLogs { get; set; }
        public DbSet<CustomNotification> CustomNotifications { get; set; }
        public DbSet<CustomNotificationTarget> CustomNotificationTargets { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<SystemAuditTrail> SystemAuditTrails { get; set; }

        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<ProfileAttachment> ProfileAttachments { get; set; }
        public DbSet<ProfileWorkScope> ProfileWorkScopes { get; set; }
        public DbSet<ProfileMolSummary> ProfileMOLSummaries { get; set; }
        public DbSet<JobSeekerEducationSummary> JobSeekerEducationSummaries { get; set; }
        public DbSet<JobSeekerDiplomaMarksVerification> JobSeekerDiplomaMarksVerifications { get; set; }
        public DbSet<JobSeekerEducation> JobSeekerEducations { get; set; }
        public DbSet<JobSeekerEducationAttachment> JobSeekerEducationAttachments { get; set; }
        public DbSet<JobSeekerExperience> JobSeekerExperiences { get; set; }
        public DbSet<JobSeekerExperienceAttachment> JobSeekerExperienceAttachments { get; set; }
        public DbSet<JobSeekerExperienceSkill> JobSeekerExperienceSkills { get; set; }
        public DbSet<JobSeekerExperienceTechStack> JobSeekerExperienceTechStacks { get; set; }
        public DbSet<JobSeekerCertificate> JobSeekerCertificates { get; set; }
        public DbSet<JobSeekerCertificateAttachment> JobSeekerCertificateAttachments { get; set; }
        public DbSet<JobSeekerTraining> JobSeekerTrainings { get; set; }
        public DbSet<JobSeekerTrainingAttachment> JobSeekerTrainingAttachments { get; set; }
        public DbSet<JobSeekerTrainingSkill> JobSeekerTrainingSkills { get; set; }
        public DbSet<JobSeekerTrainingTechStack> JobSeekerTrainingTechStacks { get; set; }
        public DbSet<JobSeekerPortfolioItem> JobSeekerPortfolioItems { get; set; }
        public DbSet<JobSeekerPortfolioItemAttachment> JobSeekerPortfolioItemAttachments { get; set; }
        public DbSet<JobSeekerPortfolioItemSkill> JobSeekerPortfolioItemSkills { get; set; }
        public DbSet<JobSeekerPortfolioItemTechStack> JobSeekerPortfolioItemTechStacks { get; set; }

        public DbSet<JobOpportunity> JobOpportunities { get; set; }
        public DbSet<JobOpportunityApplicationMethod> JobOpportunityApplicationMethods { get; set; }
        public DbSet<JobOpportunityLocation> JobOpportunityLocations { get; set; }
        public DbSet<JobOpportunityCompensation> JobOpportunityCompensations { get; set; }
        public DbSet<JobOpportunityCompensationBenefit> JobOpportunityCompensationBenefits { get; set; }
        public DbSet<JobOpportunityRequirement> JobOpportunityRequirements { get; set; }
        public DbSet<JobOpportunityRequirementSkill> JobOpportunityRequirementSkills { get; set; }
        public DbSet<JobOpportunityRequirementTechStack> JobOpportunityRequirementTechStacks { get; set; }
        public DbSet<JobOpportunityRequirementFieldOfStudy> JobOpportunityRequirementFieldOfStudies { get; set; }
        public DbSet<JobOpportunityRequirementLanguage> JobOpportunityRequirementLanguages { get; set; }
        public DbSet<JobOpportunityRequirementCertification> JobOpportunityRequirementCertifications { get; set; }
        public DbSet<JobOpportunityRequiredDocument> JobOpportunityRequiredDocuments { get; set; }
        public DbSet<JobOpportunityScreeningQuestion> JobOpportunityScreeningQuestions { get; set; }
        public DbSet<JobOpportunityScreeningQuestionOption> JobOpportunityScreeningQuestionOptions { get; set; }
        public DbSet<JobOpportunityAttachment> JobOpportunityAttachments { get; set; }
        public DbSet<JobOpportunityParticipation> JobOpportunityParticipations { get; set; }
        public DbSet<JobOpportunityParticipationAnswer> JobOpportunityParticipationAnswers { get; set; }
        public DbSet<JobOpportunityParticipationAttachment> JobOpportunityParticipationAttachments { get; set; }
        public DbSet<JobSeekerSavedOpportunity> JobSeekerSavedOpportunities { get; set; }

        public DbSet<CredentialDefinition> CredentialDefinitions { get; set; }
        public DbSet<CredentialDefinitionSkill> CredentialDefinitionSkills { get; set; }
        public DbSet<CredentialDefinitionBasicSkill> CredentialDefinitionBasicSkills { get; set; }
        public DbSet<CredentialDefinitionTechStack> CredentialDefinitionTechStacks { get; set; }
        public DbSet<CredentialEnrollment> CredentialEnrollments { get; set; }

        public DbSet<InterviewRecord> InterviewRecords { get; set; }
        public DbSet<InterviewRecordSkill> InterviewRecordSkills { get; set; }
        public DbSet<InterviewRecordBasicSkill> InterviewRecordBasicSkills { get; set; }
        public DbSet<InterviewRecordTechStack> InterviewRecordTechStacks { get; set; }
        public DbSet<InterviewEnrollment> InterviewEnrollments { get; set; }

        public DbSet<AssessmentResult> AssessmentResults { get; set; }
        public DbSet<AssessmentRejectedOffer> AssessmentRejectedOffers { get; set; }
        public DbSet<AssessmentProfessionalSkillRating> AssessmentProfessionalSkillRatings { get; set; }
        public DbSet<AssessmentBasicSkillRating> AssessmentBasicSkillRatings { get; set; }
        public DbSet<AssessmentTechStackSkillRating> AssessmentTechStackSkillRatings { get; set; }
        public DbSet<AssessmentResultBehavior> AssessmentResultBehaviors { get; set; }
        public DbSet<JobSeekerSkillSummary> JobSeekerSkillSummaries { get; set; }

        public DbSet<EmploymentProcedureRecord> EmploymentProcedureRecords { get; set; }

        public DbSet<ContractWageLine> ContractWageLines { get; set; }

        public DbSet<ContractBenefitLine> ContractBenefitLines { get; set; }
        public DbSet<EmploymentAcceptanceDocument> EmploymentAcceptanceDocuments { get; set; }
        public DbSet<EmploymentInquiryMessage> EmploymentInquiryMessages { get; set; }
        public DbSet<EmploymentInquiryMessageAttachment> EmploymentInquiryMessageAttachments { get; set; }
        public DbSet<PostEmploymentRecord> PostEmploymentRecords { get; set; }
        public DbSet<WithdrawalRequest> WithdrawalRequests { get; set; }
        public DbSet<WithdrawalRequestAttachment> WithdrawalRequestAttachments { get; set; }
        public DbSet<WithdrawalRequestOperatorResponse> WithdrawalRequestOperatorResponses { get; set; }
        public DbSet<EmploymentContractChangeHistory> EmploymentContractChangeHistory { get; set; }
        public DbSet<PostEmploymentEvaluation> PostEmploymentEvaluations { get; set; }
        public DbSet<PostEmploymentEvaluationSkillRating> PostEmploymentEvaluationSkillRatings { get; set; }
        public DbSet<PostEmploymentPeriodicEvaluation> PostEmploymentPeriodicEvaluations { get; set; }

        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<AppPage> AppPages { get; set; }
        public DbSet<RolePage> RolePages { get; set; }

        private static void ConfigureIdentityAccess(ModelBuilder builder)
        {
            builder.Entity<UserType>(e =>
            {
                e.HasIndex(x => x.Code).IsUnique();
            });

            builder.Entity<UserGroup>(e =>
            {
                e.HasIndex(x => new { x.UserTypeId, x.Code }).IsUnique();
                e.HasOne(x => x.UserType)
                    .WithMany(t => t.UserGroups)
                    .HasForeignKey(x => x.UserTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Organization>(e =>
            {
                e.HasIndex(x => x.CrNumber);
            });

            builder.Entity<AppPage>(e =>
            {
                e.HasIndex(x => x.Code).IsUnique();
                e.HasIndex(x => x.SystemCode);
            });

            builder.Entity<RolePage>(e =>
            {
                e.HasIndex(x => new { x.RoleId, x.AppPageId }).IsUnique();
                e.HasOne(x => x.Role)
                    .WithMany(r => r.RolePagesList)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.AppPage)
                    .WithMany()
                    .HasForeignKey(x => x.AppPageId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Roles>(e =>
            {
                e.HasOne(x => x.UserGroup)
                    .WithMany()
                    .HasForeignKey(x => x.UserGroupId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Organization)
                    .WithMany()
                    .HasForeignKey(x => x.OrganizationId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.ParentRole)
                    .WithMany()
                    .HasForeignKey(x => x.ParentRoleId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(x => new { x.OrganizationId, x.NormalizedName });
            });

            builder.Entity<AppUser>(e =>
            {
                e.HasOne(x => x.UserGroup)
                    .WithMany()
                    .HasForeignKey(x => x.UserGroupId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Organization)
                    .WithMany()
                    .HasForeignKey(x => x.OrganizationId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(x => new { x.ProfileId, x.UserGroupId, x.OrganizationId });
                e.HasIndex(x => x.IsPrimary);
            });

            builder.Entity<ClaimMaster>(e =>
            {
                e.Property(x => x.ClaimName).HasMaxLength(100);
                e.HasIndex(x => x.ClaimName);
            });
        }
    }
}

using System.Linq;
using Bidding_Core.Helpers;
using Bidding_Models.DatabaseModel.Notifications;
using Bidding_Models.Jobseekers;
using Xunit;

namespace Bidding_Core.Tests
{
    /// <summary>Custom notifications: Registered / Not registered / All audience for job seekers and organizations.</summary>
    public sealed class CustomNotificationAudienceTests
    {
        [Fact]
        public void UnregisteredJobSeeker_GetsArabicEmailAndSms_SystemSkipped_AndDuplicateContactsDropped()
        {
            var notification = new CustomNotification
            {
                Title_en = "Hello", Title_ar = "مرحبا", Body_en = "Body", Body_ar = "نص",
                SendEmail = true, SendSms = true, SendSystem = true,
            };
            var users = new[] { new CustomNotificationRules.RecipientUser { UserId = "u1", Email = "taken@x.om", Phone = "9911", PreferredLanguage = "en" } };
            var jobSeekers = new[]
            {
                new CustomNotificationRules.RecipientUnregisteredJobSeeker { ProfileId = 10, Email = "js@x.om", Phone = "95844441" },
                new CustomNotificationRules.RecipientUnregisteredJobSeeker { ProfileId = 11, Email = "taken@x.om", Phone = "9911" },
            };

            var plan = CustomNotificationRules.Plan(notification, users, null, jobSeekers);

            Assert.Equal(2, plan.TotalRecipients);
            var js = plan.Deliveries.Where(d => d.ProfileId == 10).ToList();
            var email = js.Single(d => d.Channel == CustomNotificationChannels.Email);
            Assert.False(email.Skipped);
            Assert.Equal("مرحبا", email.Subject);
            Assert.DoesNotContain("Hello", email.Body);
            Assert.False(js.Single(d => d.Channel == CustomNotificationChannels.Sms).Skipped);
            Assert.True(js.Single(d => d.Channel == CustomNotificationChannels.System).Skipped);
            Assert.DoesNotContain(plan.Deliveries, d => d.ProfileId == 11);
        }

        [Fact]
        public void NormalizeTargets_FiltersDefaultToAll_AndFilteredAllOrganizationsKeepsIndividualPicks()
        {
            var filtered = CustomNotificationRules.NormalizeTargets(new[]
            {
                new CustomNotificationTargetDto { TargetType = CustomNotificationTargetTypes.JobSeekers, RegistrationFilter = "notregistered" },
                new CustomNotificationTargetDto { TargetType = CustomNotificationTargetTypes.AllOrganizations, RegistrationFilter = "Registered" },
                new CustomNotificationTargetDto { TargetType = CustomNotificationTargetTypes.Organization, OrganizationId = 5 },
            });
            var unfiltered = CustomNotificationRules.NormalizeTargets(new[]
            {
                new CustomNotificationTargetDto { TargetType = CustomNotificationTargetTypes.JobSeekers, RegistrationFilter = "bogus" },
                new CustomNotificationTargetDto { TargetType = CustomNotificationTargetTypes.AllOrganizations },
                new CustomNotificationTargetDto { TargetType = CustomNotificationTargetTypes.Organization, OrganizationId = 5 },
            });

            Assert.Equal(CustomNotificationRegistrationFilters.NotRegistered,
                filtered.Single(t => t.TargetType == CustomNotificationTargetTypes.JobSeekers).RegistrationFilter);
            Assert.Equal(CustomNotificationRegistrationFilters.Registered,
                filtered.Single(t => t.TargetType == CustomNotificationTargetTypes.AllOrganizations).RegistrationFilter);
            Assert.Contains(filtered, t => t.TargetType == CustomNotificationTargetTypes.Organization);

            Assert.Equal(CustomNotificationRegistrationFilters.All,
                unfiltered.Single(t => t.TargetType == CustomNotificationTargetTypes.JobSeekers).RegistrationFilter);
            Assert.DoesNotContain(unfiltered, t => t.TargetType == CustomNotificationTargetTypes.Organization);
        }

        [Fact]
        public void Reachability_UnregisteredJobSeekersOnly_NeedEmailOrSms()
        {
            var preview = new CustomNotificationRecipientPreviewDto { RegisteredUsers = 0, UnregisteredJobSeekers = 3, Total = 3 };

            Assert.NotNull(CustomNotificationRules.ValidateReachability(preview, sendEmail: false, sendSms: false));
            Assert.Null(CustomNotificationRules.ValidateReachability(preview, sendEmail: false, sendSms: true));
            Assert.Null(CustomNotificationRules.ValidateReachability(preview, sendEmail: true, sendSms: false));
        }
    }
}

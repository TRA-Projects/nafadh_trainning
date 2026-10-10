using System;
using System.Collections.Generic;
using System.Linq;
using Bidding_Core.Helpers;
using Bidding_Models.DatabaseModel.Notifications;
using Bidding_Models.Jobseekers;
using Xunit;

namespace Bidding_Core.Tests
{
    public sealed class CustomNotificationRulesTests
    {
        private static CustomNotification AllChannels() => new CustomNotification
        {
            Id = 1,
            Title_en = "Hello",
            Title_ar = "مرحبا",
            Body_en = "Body",
            Body_ar = "نص",
            SendEmail = true,
            SendSms = true,
            SendSystem = true,
        };

        [Fact]
        public void Plan_SameUserFromSeveralTargets_IsNotifiedOnce_InPreferredLanguage()
        {
            var users = new[]
            {
                new CustomNotificationRules.RecipientUser { UserId = "u1", Email = "a@x.om", Phone = "9911", PreferredLanguage = "ar" },
                new CustomNotificationRules.RecipientUser { UserId = "u1", Email = "a@x.om", Phone = "9911", PreferredLanguage = "ar" },
                new CustomNotificationRules.RecipientUser { UserId = "u2", Email = "A@X.om", Phone = "99-11", PreferredLanguage = null },
            };

            var plan = CustomNotificationRules.Plan(AllChannels(), users, null);

            Assert.Equal(2, plan.TotalRecipients);
            var emails = plan.Deliveries.Where(d => d.Channel == CustomNotificationChannels.Email && !d.Skipped).ToList();
            Assert.Single(emails);
            Assert.Equal("مرحبا", emails[0].Subject);
            Assert.Single(plan.Deliveries.Where(d => d.Channel == CustomNotificationChannels.Sms && !d.Skipped));
            var system = plan.Deliveries.Where(d => d.Channel == CustomNotificationChannels.System).ToList();
            Assert.Equal(2, system.Count);
            Assert.Equal("Hello", system.Single(d => d.UserId == "u2").Subject);
        }

        [Fact]
        public void Plan_UnregisteredOrganization_GetsBilingualEmailOnly_OtherChannelsSkipped()
        {
            var orgs = new[] { new CustomNotificationRules.RecipientOrganization { OrganizationId = 7, Email = "org@x.om", Name = "Org" } };

            var plan = CustomNotificationRules.Plan(AllChannels(), null, orgs);

            var email = plan.Deliveries.Single(d => d.Channel == CustomNotificationChannels.Email);
            Assert.False(email.Skipped);
            Assert.Contains("مرحبا", email.Subject);
            Assert.Contains("Hello", email.Subject);
            Assert.Contains("نص", email.Body);
            Assert.Contains("Body", email.Body);
            Assert.True(plan.Deliveries.Single(d => d.Channel == CustomNotificationChannels.Sms).Skipped);
            Assert.True(plan.Deliveries.Single(d => d.Channel == CustomNotificationChannels.System).Skipped);
        }

        [Fact]
        public void CanEdit_OnlyWhileScheduled_AndInboxRowsAreOwnerOnly()
        {
            Assert.True(CustomNotificationRules.CanEdit(CustomNotificationStatuses.Scheduled));
            Assert.False(CustomNotificationRules.CanEdit(CustomNotificationStatuses.Sent));
            Assert.False(CustomNotificationRules.CanEdit(CustomNotificationStatuses.Cancelled));

            Assert.True(CustomNotificationRules.CanUserAccessInboxRow("u1", "u1"));
            Assert.False(CustomNotificationRules.CanUserAccessInboxRow("u1", "u2"));
            Assert.False(CustomNotificationRules.CanUserAccessInboxRow("u1", null));
        }

        [Fact]
        public void Validate_RejectsPastSchedule_MissingChannel_AndOrgsOnlyWithoutEmail()
        {
            var now = new DateTime(2026, 10, 5, 12, 0, 0, DateTimeKind.Utc);
            var request = new UpsertCustomNotificationRequestDto
            {
                Title_en = "T",
                Title_ar = "ع",
                Body_en = "B",
                Body_ar = "ن",
                SendSystem = true,
                ScheduledAtUtc = "2026-10-05T10:00:00Z",
                Targets = new List<CustomNotificationTargetDto>
                {
                    new CustomNotificationTargetDto { TargetType = CustomNotificationTargetTypes.User, UserId = "u1" },
                },
            };

            Assert.NotNull(CustomNotificationRules.Validate(request, now, out _));

            request.ScheduledAtUtc = "2026-10-05T13:00:00Z";
            Assert.Null(CustomNotificationRules.Validate(request, now, out var scheduled));
            Assert.Equal(new DateTime(2026, 10, 5, 13, 0, 0, DateTimeKind.Utc), scheduled);

            request.SendSystem = false;
            Assert.NotNull(CustomNotificationRules.Validate(request, now, out _));

            var orgsOnly = new CustomNotificationRecipientPreviewDto { RegisteredUsers = 0, EmailOnlyOrganizations = 3, Total = 3 };
            Assert.NotNull(CustomNotificationRules.ValidateReachability(orgsOnly, sendEmail: false));
            Assert.Null(CustomNotificationRules.ValidateReachability(orgsOnly, sendEmail: true));
        }
    }
}

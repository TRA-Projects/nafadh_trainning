using System.Collections.Generic;
using System.Linq;
using Bidding_Common;
using Bidding_Core.Helpers;
using Bidding_Core.Services.Implementations;
using ConstructionHub_Core.Helpers;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Bidding_Core.Tests
{
    public sealed class EmailSendRulesTests
    {
        [Fact]
        public void OutboundSending_IsEnabled()
        {
            Assert.False(EmailSendRules.ShouldSkipOutboundSending());
            Assert.False(EmailSendRules.TreatSkippedSendAsSuccess());
        }

        [Fact]
        public void VerificationMailbox_UsesVerificationsFromAddress()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["EmailSettings:Username"] = "smtp-user@nafadh.om".Encrypt(),
                    ["EmailSettings:Password"] = "secret".Encrypt(),
                    ["EmailSettings:Verifications:Username"] = "verify-user@nafadh.om".Encrypt(),
                })
                .Build();

            var settings = new EmailSettingsVerifications(config);

            Assert.Equal("verify-user@nafadh.om", settings.FromEmail);
            Assert.Equal("smtp-user@nafadh.om", settings.Username);
        }

        [Fact]
        public void JobseekerDispatch_UsesVerificationMailbox()
        {
            var ctor = typeof(NotificationDispatchService).GetConstructors().Single();
            Assert.Contains(
                ctor.GetParameters(),
                p => p.ParameterType == typeof(EmailSettingsVerifications));
        }
    }
}

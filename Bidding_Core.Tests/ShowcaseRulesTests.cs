using System;
using Bidding_API.Helpers;
using Bidding_Core.Helpers;
using Bidding_Core.Services.Implementations;
using Bidding_Models.Jobseekers;
using Xunit;

namespace Bidding_Core.Tests
{
    public sealed class ShowcaseRulesTests
    {
        private static UpsertShowcaseItemRequestDto Valid() => new UpsertShowcaseItemRequestDto
        {
            Title_en = "Workshop",
            Title_ar = "ورشة",
            Category = "Training",
            Status = "Draft",
            ContactEmail = "user@example.com",
            PhoneNumber = "+96891234567",
            Amount = 10,
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 1, 5),
        };

        [Fact]
        public void Validate_CompleteRequest_HasNoError()
        {
            Assert.Null(ShowcaseRules.Validate(Valid()));
        }

        [Fact]
        public void Validate_EndBeforeStart_IsRejected()
        {
            var body = Valid();
            body.EndDate = body.StartDate.Value.AddDays(-1);
            Assert.Equal("End date must be on or after the start date.", ShowcaseRules.Validate(body));
        }

        [Fact]
        public void Update_Approved_IsBlocked()
        {
            var service = new ShowcaseMockService();
            var created = service.Create(Valid());
            var approve = Valid();
            approve.Status = ShowcaseRules.StatusApproved;
            var approved = service.Update(created.Id, approve);

            Assert.False(approved.CanEdit);
            Assert.Throws<SafeExecutionHelper.BadRequestException>(() => service.Update(created.Id, Valid()));
            Assert.Throws<SafeExecutionHelper.BadRequestException>(() => service.Delete(created.Id));
        }
    }
}

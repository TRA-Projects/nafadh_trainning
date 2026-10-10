using System;
using System.Globalization;
using Bidding_Core.Services.Implementations;
using Bidding_Models.DTOs;
using Bidding_Models.Jobseekers;
using Xunit;

namespace Bidding_Core.Tests
{
    public sealed class NafathJobSeekerActiveTests
    {
        [Fact]
        public void IsActive_RequiresFlagY_AndFutureExpActiveDate()
        {
            var future = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            var past = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

            Assert.True(NafathJobSeekerService.IsActiveJobSeekerRecord(new NafathJobSeekerDetailsDto
            {
                ActiveFlag = "Y",
                ExpActiveDate = future,
            }));

            Assert.False(NafathJobSeekerService.IsActiveJobSeekerRecord(new NafathJobSeekerDetailsDto
            {
                ActiveFlag = "N",
                ExpActiveDate = future,
            }));

            Assert.False(NafathJobSeekerService.IsActiveJobSeekerRecord(new NafathJobSeekerDetailsDto
            {
                ActiveFlag = "Y",
                ExpActiveDate = past,
            }));

            Assert.False(NafathJobSeekerService.IsActiveJobSeekerRecord(new NafathJobSeekerDetailsDto
            {
                ActiveFlag = "Y",
                ExpActiveDate = "null",
            }));
        }

        [Fact]
        public void ValidityCheck_CanonicalizesKnownCodes()
        {
            Assert.True(NafathValidityCheck.TryCanonicalize("flag", out var flag, out _));
            Assert.Equal(NafathValidityCheck.ActiveFlag, flag);
            Assert.Equal(NafathValidityCheckMode.ExpiryDate, NafathValidityCheck.Parse("DATE"));
            Assert.False(NafathValidityCheck.TryCanonicalize("nope", out _, out var error));
            Assert.False(string.IsNullOrWhiteSpace(error));
        }

        [Fact]
        public void IsActive_HonorsValidityCheckMode()
        {
            var future = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            var past = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            var inactiveFlag = new NafathJobSeekerDetailsDto { ActiveFlag = "N", ExpActiveDate = future };
            var expired = new NafathJobSeekerDetailsDto { ActiveFlag = "Y", ExpActiveDate = past };

            Assert.True(NafathJobSeekerService.IsActiveJobSeekerRecord(inactiveFlag, NafathValidityCheckMode.None));
            Assert.True(NafathJobSeekerService.IsActiveJobSeekerRecord(expired, NafathValidityCheckMode.None));

            Assert.True(NafathJobSeekerService.IsActiveJobSeekerRecord(inactiveFlag, NafathValidityCheckMode.ExpiryDate));
            Assert.False(NafathJobSeekerService.IsActiveJobSeekerRecord(expired, NafathValidityCheckMode.ExpiryDate));

            Assert.False(NafathJobSeekerService.IsActiveJobSeekerRecord(inactiveFlag, NafathValidityCheckMode.ActiveFlag));
            Assert.True(NafathJobSeekerService.IsActiveJobSeekerRecord(expired, NafathValidityCheckMode.ActiveFlag));

            Assert.False(NafathJobSeekerService.IsActiveJobSeekerRecord(inactiveFlag, NafathValidityCheckMode.Both));
            Assert.False(NafathJobSeekerService.IsActiveJobSeekerRecord(expired, NafathValidityCheckMode.Both));
        }

        [Fact]
        public void FormatInactiveDetail_IncludesFoundFlagAndExpiry()
        {
            var past = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            var detail = NafathJobSeekerService.FormatInactiveDetail(
                new NafathJobSeekerDetailsDto { ActiveFlag = "N", ExpActiveDate = past },
                NafathValidityCheckMode.Both);

            Assert.Contains("Active flag found: N", detail);
            Assert.Contains("expected Y", detail);
            Assert.Contains("must be after today", detail);
            Assert.Contains("|flag=N|", detail);
            Assert.Contains("|flagOk=0|", detail);
            Assert.Contains("|expiryOk=0|", detail);
            Assert.Contains("|mode=BOTH", detail);
        }

        [Fact]
        public void FormatInactiveDetail_ReportsNotSetWhenFieldsMissing()
        {
            var detail = NafathJobSeekerService.FormatInactiveDetail(
                new NafathJobSeekerDetailsDto { ActiveFlag = "null", ExpActiveDate = "null" },
                NafathValidityCheckMode.Both);

            Assert.Contains("Active flag found: not set", detail);
            Assert.Contains("Expiry date found: not set", detail);
            Assert.Contains("|flag=|", detail);
            Assert.Contains("|expiry=|", detail);
        }

        [Fact]
        public void FormatInactiveDetail_ActiveFlagMode_OmitsExpirySentence()
        {
            var detail = NafathJobSeekerService.FormatInactiveDetail(
                new NafathJobSeekerDetailsDto { ActiveFlag = "N", ExpActiveDate = "2030-01-01T00:00:00" },
                NafathValidityCheckMode.ActiveFlag);

            Assert.Contains("Active flag found: N", detail);
            Assert.DoesNotContain("Expiry date found", detail);
            Assert.Contains("|mode=ACTIVE_FLAG", detail);
        }

        [Fact]
        public void FormatDisplayDate_UsesDayMonthYear()
        {
            Assert.Equal("18/03/1990", NafathJobSeekerService.FormatDisplayDate("1990-03-18T00:00:00"));
            Assert.Equal("10/01/2024", NafathJobSeekerService.FormatDisplayDate("2024-01-10"));
            Assert.Equal("01/02/2026", NafathJobSeekerService.FormatDisplayDate("01/02/2026"));
            Assert.Null(NafathJobSeekerService.FormatDisplayDate("null"));
        }

        [Fact]
        public void FormatAddress_JoinsVillageWilayatGovernorate()
        {
            var address = NafathJobSeekerService.FormatAddress(new NafathJobSeekerDetailsDto
            {
                PerVillageDescArb = "الخوض",
                PerWilayatDescArb = "السيب",
                PerRegionDesc = "مسقط",
            });

            Assert.Equal("الخوض، السيب، مسقط", address);
        }
    }
}

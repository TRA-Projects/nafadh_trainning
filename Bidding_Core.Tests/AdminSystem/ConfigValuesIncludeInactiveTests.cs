using System;
using System.Linq;
using System.Threading.Tasks;
using Bidding_Core.Services.Implementations;
using Bidding_DLL;
using Bidding_Models.DatabaseModel.SystemCatalog;
using Bidding_Models.Jobseekers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Bidding_Core.Tests.AdminSystem
{
    public class ConfigValuesIncludeInactiveTests
    {
        [Fact]
        public async Task GetConfigValuesPage_Default_ReturnsActiveOnly()
        {
            using var context = CreateContext(nameof(GetConfigValuesPage_Default_ReturnsActiveOnly));
            await SeedAsync(context);

            var page = await CreateService(context).GetConfigValuesPageAsync("LANG", 0, 15, null);

            Assert.Equal(1, page.total);
            Assert.Equal("Active", page.rows.Single().label_en);
        }

        [Fact]
        public async Task GetConfigValuesPage_IncludeInactive_ReturnsActiveAndInactive()
        {
            using var context = CreateContext(nameof(GetConfigValuesPage_IncludeInactive_ReturnsActiveAndInactive));
            await SeedAsync(context);

            var page = await CreateService(context).GetConfigValuesPageAsync("LANG", 0, 15, null, includeInactive: true);

            Assert.Equal(2, page.total);
            Assert.Contains(page.rows, row => row.label_en == "Inactive" && row.IsActive == false);
        }

        [Fact]
        public async Task Update_Deactivate_StaysOnAdminList()
        {
            using var context = CreateContext(nameof(Update_Deactivate_StaysOnAdminList));
            await SeedAsync(context);
            var service = CreateService(context);

            var updated = await service.UpdateConfigValueAsync("LANG", "en", new UpdateSystemConfigValueRequestDto
            {
                label_en = "Active",
                label_ar = "نشط",
                IsActive = false,
            });

            Assert.NotNull(updated);
            Assert.False(updated.IsActive);

            var page = await service.GetConfigValuesPageAsync("LANG", 0, 15, null, includeInactive: true);

            Assert.Equal(2, page.total);
            Assert.Contains(page.rows, row => Equals(row.id, "en") && row.IsActive == false);
        }

        [Fact]
        public async Task Update_Deactivate_HiddenFromActiveLookup()
        {
            using var context = CreateContext(nameof(Update_Deactivate_HiddenFromActiveLookup));
            await SeedAsync(context);
            var service = CreateService(context);

            await service.UpdateConfigValueAsync("LANG", "en", new UpdateSystemConfigValueRequestDto
            {
                IsActive = false,
            });

            var dropdown = await service.GetConfigValuesPageAsync("LANG", null, null, null);

            Assert.DoesNotContain(dropdown.rows, row => Equals(row.id, "en"));
        }

        [Fact]
        public async Task Update_Deactivate_DoesNotDeleteTheRow()
        {
            using var context = CreateContext(nameof(Update_Deactivate_DoesNotDeleteTheRow));
            await SeedAsync(context);

            await CreateService(context).UpdateConfigValueAsync("LANG", "en", new UpdateSystemConfigValueRequestDto
            {
                IsActive = false,
            });

            Assert.Equal(2, await context.SystemConfigValues.CountAsync());
            Assert.Contains(context.SystemConfigValues, row => row.ValueKey == "en" && row.IsActive == false);
        }

        [Fact]
        public async Task GetConfigValuesPage_IncludeInactive_StillFiltersByParent()
        {
            using var context = CreateContext(nameof(GetConfigValuesPage_IncludeInactive_StillFiltersByParent));
            await SeedAsync(context);

            var page = await CreateService(context).GetConfigValuesPageAsync("LANG", 0, 15, "other-parent", includeInactive: true);

            Assert.Equal(0, page.total);
        }

        private static AdminSystemService CreateService(CHDBContext context) =>
            new AdminSystemService(context, notificationTemplateRenderer: null, notificationDispatch: null);

        private static async Task SeedAsync(CHDBContext context)
        {
            var group = new SystemConfigGroup
            {
                Code = "LANG",
                NameEn = "Languages",
                NameAr = "اللغات",
                IsActive = true,
            };
            context.SystemConfigGroups.Add(group);
            await context.SaveChangesAsync();

            var now = DateTime.UtcNow;
            context.SystemConfigValues.AddRange(
                new SystemConfigValue
                {
                    ConfigGroupId = group.Id,
                    ValueKey = "en",
                    Code = "EN",
                    LabelEn = "Active",
                    LabelAr = "نشط",
                    IsActive = true,
                    SortOrder = 1,
                    CreatedAt = now,
                    UpdatedAt = now,
                },
                new SystemConfigValue
                {
                    ConfigGroupId = group.Id,
                    ValueKey = "xx",
                    Code = "XX",
                    LabelEn = "Inactive",
                    LabelAr = "غير نشط",
                    IsActive = false,
                    ParentValueKey = "parent-a",
                    SortOrder = 2,
                    CreatedAt = now,
                    UpdatedAt = now,
                });
            await context.SaveChangesAsync();
        }

        private static CHDBContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<CHDBContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new CHDBContext(options);
        }
    }
}

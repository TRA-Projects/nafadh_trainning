using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bidding_Models;
using Bidding_Models.DatabaseModel;
using Bidding_Models.DatabaseModel.IdentityAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bidding_DLL.Seeding
{
    /// <summary>
    /// Idempotent seed for user types/groups, template roles, claims, pages, and role links.
    /// Also backfills AspNetUsers.UserGroupId / OrganizationId / IsPrimary and AspNetUserRoles from legacy ProfileType.
    /// </summary>
    public static class IdentityAccessDbSeeder
    {
        public static string TemplateRoleName(string userTypeCode, string groupCode)
            => $"{userTypeCode}.{groupCode}";

        public static async Task ApplyAsync(CHDBContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Tables appear only after EF migration is applied.
            if (!await TableExistsAsync(context, "UserTypes"))
            {
                return;
            }

            var utcNow = DateTime.UtcNow;
            await EnsurePermissionColumnsAsync(context);
            await SeedUserTypesAndGroupsAsync(context, utcNow);
            await SeedClaimsAsync(context);
            await SeedPagesAsync(context, utcNow);
            await SeedTemplateRolesAsync(context);
            await SeedRoleClaimsAsync(context);
            await SeedRolePagesAsync(context);
            await BackfillUsersFromLegacyProfileTypeAsync(context, utcNow);
        }

        private static async Task<bool> TableExistsAsync(CHDBContext context, string tableName)
        {
            try
            {
                var conn = context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT CASE WHEN OBJECT_ID(@p, 'U') IS NULL THEN 0 ELSE 1 END";
                var p = cmd.CreateParameter();
                p.ParameterName = "@p";
                p.Value = tableName;
                cmd.Parameters.Add(p);
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result) == 1;
            }
            catch
            {
                return false;
            }
        }

        private static async Task EnsurePermissionColumnsAsync(CHDBContext context)
        {
            await context.Database.ExecuteSqlRawAsync(@"
IF OBJECT_ID(N'dbo.AppPages', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.AppPages', N'RequiredActionFlags') IS NULL
        ALTER TABLE dbo.AppPages ADD RequiredActionFlags int NOT NULL CONSTRAINT DF_AppPages_RequiredActionFlags DEFAULT 1;
    IF COL_LENGTH(N'dbo.AppPages', N'DefaultDataScope') IS NULL
        ALTER TABLE dbo.AppPages ADD DefaultDataScope nvarchar(32) NOT NULL CONSTRAINT DF_AppPages_DefaultDataScope DEFAULT N'mine';
END
IF OBJECT_ID(N'dbo.AspNetRolePages', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.AspNetRolePages', N'ActionFlags') IS NULL
        ALTER TABLE dbo.AspNetRolePages ADD ActionFlags int NOT NULL CONSTRAINT DF_AspNetRolePages_ActionFlags DEFAULT 1;
    IF COL_LENGTH(N'dbo.AspNetRolePages', N'DataScope') IS NULL
        ALTER TABLE dbo.AspNetRolePages ADD DataScope nvarchar(32) NOT NULL CONSTRAINT DF_AspNetRolePages_DataScope DEFAULT N'mine';
END
");
        }

        private static async Task SeedUserTypesAndGroupsAsync(CHDBContext context, DateTime utcNow)
        {
            var types = await context.UserTypes.ToListAsync();
            var typeByCode = types.ToDictionary(t => t.Code, StringComparer.OrdinalIgnoreCase);

            foreach (var seed in IdentityAccessSeedData.UserTypes)
            {
                if (!typeByCode.TryGetValue(seed.Code, out var row))
                {
                    row = new UserType
                    {
                        Code = seed.Code,
                        NameEn = seed.NameEn,
                        NameAr = seed.NameAr,
                        IsActive = true,
                        CreatedAt = utcNow,
                        UpdatedAt = utcNow
                    };
                    context.UserTypes.Add(row);
                    typeByCode[seed.Code] = row;
                }
                else
                {
                    row.NameEn = seed.NameEn;
                    row.NameAr = seed.NameAr;
                    row.IsActive = true;
                    row.UpdatedAt = utcNow;
                }
            }

            await context.SaveChangesAsync();

            var groups = await context.UserGroups.Include(g => g.UserType).ToListAsync();
            foreach (var seed in IdentityAccessSeedData.UserGroups)
            {
                if (!typeByCode.TryGetValue(seed.UserTypeCode, out var type))
                {
                    continue;
                }

                var existing = groups.FirstOrDefault(g =>
                    g.UserTypeId == type.Id
                    && string.Equals(g.Code, seed.Code, StringComparison.OrdinalIgnoreCase));

                if (existing == null)
                {
                    context.UserGroups.Add(new UserGroup
                    {
                        UserTypeId = type.Id,
                        Code = seed.Code,
                        NameEn = seed.NameEn,
                        NameAr = seed.NameAr,
                        SortOrder = seed.SortOrder,
                        IsActive = true,
                        CreatedAt = utcNow,
                        UpdatedAt = utcNow
                    });
                }
                else
                {
                    existing.NameEn = seed.NameEn;
                    existing.NameAr = seed.NameAr;
                    existing.SortOrder = seed.SortOrder;
                    existing.IsActive = true;
                    existing.UpdatedAt = utcNow;
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedClaimsAsync(CHDBContext context)
        {
            var existing = await context.ClaimMaster.ToListAsync();
            var byName = existing
                .Where(c => !string.IsNullOrWhiteSpace(c.ClaimName))
                .ToDictionary(c => c.ClaimName.Trim(), StringComparer.OrdinalIgnoreCase);

            foreach (var seed in IdentityAccessSeedData.Claims)
            {
                if (!byName.TryGetValue(seed.ClaimName, out var row))
                {
                    row = new ClaimMaster
                    {
                        ClaimName = seed.ClaimName,
                        NameEn = seed.NameEn,
                        NameAr = seed.NameAr,
                        IsActive = true
                    };
                    context.ClaimMaster.Add(row);
                    byName[seed.ClaimName] = row;
                }
                else
                {
                    row.NameEn = seed.NameEn;
                    row.NameAr = seed.NameAr;
                    row.IsActive = true;
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedPagesAsync(CHDBContext context, DateTime utcNow)
        {
            var existing = await context.AppPages.ToListAsync();
            var byCode = existing.ToDictionary(p => p.Code, StringComparer.OrdinalIgnoreCase);

            foreach (var seed in IdentityAccessSeedData.Pages)
            {
                if (!byCode.TryGetValue(seed.Code, out var row))
                {
                    context.AppPages.Add(new AppPage
                    {
                        Code = seed.Code,
                        SystemCode = seed.SystemCode,
                        Url = seed.Url,
                        NameEn = seed.NameEn,
                        NameAr = seed.NameAr,
                        SortOrder = seed.SortOrder,
                        IsActive = true,
                        CreatedAt = utcNow,
                        UpdatedAt = utcNow
                    });
                }
                else
                {
                    row.SystemCode = seed.SystemCode;
                    row.Url = seed.Url;
                    row.NameEn = seed.NameEn;
                    row.NameAr = seed.NameAr;
                    row.SortOrder = seed.SortOrder;
                    row.IsActive = true;
                    row.UpdatedAt = utcNow;
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedTemplateRolesAsync(CHDBContext context)
        {
            var groups = await context.UserGroups.Include(g => g.UserType).ToListAsync();
            var roles = await context.Roles.ToListAsync();
            var roleByName = roles.ToDictionary(r => r.NormalizedName ?? r.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var group in groups)
            {
                var roleName = TemplateRoleName(group.UserType.Code, group.Code);
                var normalized = roleName.ToUpperInvariant();
                if (!IdentityAccessSeedData.TemplateRoleIds.TryGetValue(roleName, out var roleId))
                {
                    roleId = Guid.NewGuid().ToString();
                }

                if (!roleByName.TryGetValue(normalized, out var role))
                {
                    role = new Roles
                    {
                        Id = roleId,
                        Name = roleName,
                        NormalizedName = normalized,
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        UserGroupId = group.Id,
                        OrganizationId = null,
                        ParentRoleId = null,
                        IsTemplate = true,
                        IsActive = true
                    };
                    context.Roles.Add(role);
                    roleByName[normalized] = role;
                }
                else
                {
                    role.UserGroupId = group.Id;
                    role.IsTemplate = true;
                    role.IsActive = true;
                    role.OrganizationId = null;
                }
            }

            // AccountAdmin — not tied to a UserGroup
            const string accountAdmin = IdentityRoleNames.AccountAdmin;
            var accountNormalized = accountAdmin.ToUpperInvariant();
            if (!roleByName.ContainsKey(accountNormalized))
            {
                var id = IdentityAccessSeedData.TemplateRoleIds.TryGetValue(accountAdmin, out var rid)
                    ? rid
                    : "a1000001-0001-4000-8000-000000000002";
                context.Roles.Add(new Roles
                {
                    Id = id,
                    Name = accountAdmin,
                    NormalizedName = accountNormalized,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    IsTemplate = true,
                    IsActive = true
                });
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedRoleClaimsAsync(CHDBContext context)
        {
            var claims = await context.ClaimMaster.ToListAsync();
            var claimByName = claims
                .Where(c => !string.IsNullOrWhiteSpace(c.ClaimName))
                .ToDictionary(c => c.ClaimName.Trim(), StringComparer.OrdinalIgnoreCase);

            var roles = await context.Roles.ToListAsync();
            var existingLinks = await context.Set<RolesClaims>().ToListAsync();

            foreach (var pair in IdentityAccessSeedData.RoleClaims)
            {
                var role = roles.FirstOrDefault(r =>
                    string.Equals(r.Name, pair.Key, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(r.NormalizedName, pair.Key.ToUpperInvariant(), StringComparison.OrdinalIgnoreCase));
                if (role == null)
                {
                    continue;
                }

                foreach (var claimName in pair.Value)
                {
                    if (!claimByName.TryGetValue(claimName, out var claim))
                    {
                        continue;
                    }

                    if (existingLinks.Any(l => l.RoleId == role.Id && l.ClaimMasterId == claim.Id))
                    {
                        continue;
                    }

                    context.Set<RolesClaims>().Add(new RolesClaims
                    {
                        RoleId = role.Id,
                        ClaimMasterId = claim.Id
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedRolePagesAsync(CHDBContext context)
        {
            var pages = await context.AppPages.ToListAsync();
            var pageByCode = pages.ToDictionary(p => p.Code, StringComparer.OrdinalIgnoreCase);
            var roles = await context.Roles.ToListAsync();
            var existingLinks = await context.RolePages.ToListAsync();

            foreach (var pair in IdentityAccessSeedData.RolePages)
            {
                var role = roles.FirstOrDefault(r =>
                    string.Equals(r.Name, pair.Key, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(r.NormalizedName, pair.Key.ToUpperInvariant(), StringComparison.OrdinalIgnoreCase));
                if (role == null)
                {
                    continue;
                }

                foreach (var pageCode in pair.Value)
                {
                    if (!pageByCode.TryGetValue(pageCode, out var page))
                    {
                        continue;
                    }

                    if (existingLinks.Any(l => l.RoleId == role.Id && l.AppPageId == page.Id))
                    {
                        continue;
                    }

                    context.RolePages.Add(new RolePage
                    {
                        RoleId = role.Id,
                        AppPageId = page.Id,
                        ActionFlags = IdentityAccessPermissionDefaults.ActionFlagsForRoleName(role.Name),
                        DataScope = IdentityAccessPermissionDefaults.DataScopeForRoleName(role.Name)
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task BackfillUsersFromLegacyProfileTypeAsync(CHDBContext context, DateTime utcNow)
        {
            var groups = await context.UserGroups.Include(g => g.UserType).ToListAsync();
            var roles = await context.Roles.ToListAsync();
            var roleByName = roles.ToDictionary(r => r.Name, StringComparer.OrdinalIgnoreCase);

            // ProfileId → Organization for corporate roots already created in this run
            var orgByProfileId = new Dictionary<int, int>();

            var users = await context.Users
                .Include(u => u.Profile)
                .Where(u => u.ProfileId != null)
                .ToListAsync();

            // Ensure one IsPrimary per profile
            foreach (var profileGroup in users.GroupBy(u => u.ProfileId.Value))
            {
                if (!profileGroup.Any(u => u.IsPrimary))
                {
                    profileGroup.OrderBy(u => u.CreatedDate).First().IsPrimary = true;
                }
            }

            foreach (var user in users.Where(u => u.IsPrimary || u.UserGroupId == null))
            {
                var profile = user.Profile;
                if (profile == null)
                {
                    continue;
                }

                var mapping = MapLegacyProfileType(profile.ProfileType);
                if (mapping == null)
                {
                    continue;
                }

                var (typeCode, groupCode, needsOrg, isAccountAdmin) = mapping.Value;
                var group = groups.FirstOrDefault(g =>
                    string.Equals(g.UserType.Code, typeCode, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(g.Code, groupCode, StringComparison.OrdinalIgnoreCase));
                if (group == null)
                {
                    continue;
                }

                user.UserGroupId = group.Id;

                if (needsOrg)
                {
                    if (profile.ParentProfileId.HasValue)
                    {
                        var parentOrgId = await ResolveOrCreateOrgForProfileAsync(
                            context, orgByProfileId, profile.ParentProfileId.Value, utcNow);
                        if (parentOrgId > 0)
                        {
                            user.OrganizationId = parentOrgId;
                        }
                    }
                    else
                    {
                        var orgId = await ResolveOrCreateOrgForProfileAsync(
                            context, orgByProfileId, profile.Id, utcNow);
                        if (orgId > 0)
                        {
                            user.OrganizationId = orgId;
                        }
                    }
                }

                var roleName = TemplateRoleName(typeCode, groupCode);
                if (roleByName.TryGetValue(roleName, out var role))
                {
                    await EnsureUserInRoleAsync(context, user.Id, role.Id);
                }

                if (isAccountAdmin && roleByName.TryGetValue(IdentityRoleNames.AccountAdmin, out var adminRole))
                {
                    await EnsureUserInRoleAsync(context, user.Id, adminRole.Id);
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task EnsureUserInRoleAsync(CHDBContext context, string userId, string roleId)
        {
            var exists = await context.Set<AppUserRoles>()
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            if (!exists)
            {
                context.Set<AppUserRoles>().Add(new AppUserRoles
                {
                    UserId = userId,
                    RoleId = roleId
                });
            }
        }

        private static async Task<int> ResolveOrCreateOrgForProfileAsync(
            CHDBContext context,
            Dictionary<int, int> orgByProfileId,
            int profileId,
            DateTime utcNow)
        {
            if (orgByProfileId.TryGetValue(profileId, out var existingOrgId))
            {
                return existingOrgId;
            }

            var profile = await context.Profiles.FirstOrDefaultAsync(p => p.Id == profileId);
            if (profile == null)
            {
                return 0;
            }

            // Reuse org by CR when possible
            Organization org = null;
            if (!string.IsNullOrWhiteSpace(profile.CrNumber))
            {
                org = await context.Organizations
                    .FirstOrDefaultAsync(o => o.CrNumber == profile.CrNumber);
            }

            if (org == null)
            {
                org = new Organization
                {
                    Email = FirstNonEmpty(profile.ComanyEmail, profile.EmailAddress),
                    CrNumber = profile.CrNumber,
                    CompanyNameEnglish = profile.CompanyNameEnglish,
                    CompanyNameArabic = profile.CompanyNameArabic,
                    CompanyNameWithLegalEnglish = profile.CompanyNameWithLegalEnglish,
                    CompanyNameWithLegalArabic = profile.CompanyNameWithLegalArabic,
                    RyadaNo = profile.RyadaNo,
                    RegistrationDate = profile.RegistrationDate,
                    EstablishmentDate = profile.EstablishmentDate,
                    ExpiryDate = profile.ExpiryDate,
                    SubjectToForeignInvestment = profile.SubjectToForeignInvestment,
                    IsLicensed = false,
                    IsAuthorized = false,
                    LastVerified = null,
                    IsActive = true,
                    CreatedAt = utcNow,
                    UpdatedAt = utcNow
                };
                context.Organizations.Add(org);
                await context.SaveChangesAsync();
            }

            orgByProfileId[profileId] = org.Id;
            return org.Id;
        }

        private static string FirstNonEmpty(params string[] values)
        {
            foreach (var v in values)
            {
                if (!string.IsNullOrWhiteSpace(v))
                {
                    return v.Trim();
                }
            }

            return null;
        }

        /// <summary>Maps legacy ProfileTypes to (UserTypeCode, UserGroupCode, needsOrganization, grantAccountAdmin).</summary>
        private static (string TypeCode, string GroupCode, bool NeedsOrg, bool AccountAdmin)? MapLegacyProfileType(int profileType)
        {
            if (!IdentityAccessLegacyProfileMap.TryMap(
                    profileType,
                    out var typeCode,
                    out var groupCode,
                    out var needsOrg,
                    out var accountAdmin))
            {
                return null;
            }

            return (typeCode, groupCode, needsOrg, accountAdmin);
        }
    }
}

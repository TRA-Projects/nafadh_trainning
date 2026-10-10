using Bidding_Core.Helpers;
using Bidding_Models;
using Bidding_Models.DatabaseModel.IdentityAccess;
using Xunit;

namespace Bidding_Core.Tests
{
    public sealed class IdentityAccessAdminRulesTests
    {
        [Fact]
        public void NeedsOrganization_OnlyCorporate()
        {
            Assert.True(IdentityAccessAdminRules.NeedsOrganization(UserTypeCodes.Corporate));
            Assert.False(IdentityAccessAdminRules.NeedsOrganization(UserTypeCodes.Individual));
            Assert.False(IdentityAccessAdminRules.NeedsOrganization(UserTypeCodes.System));
        }

        [Theory]
        [InlineData(UserTypeCodes.System, UserGroupCodes.SystemAdmin, (int)Enums.ProfileTypes.Admin)]
        [InlineData(UserTypeCodes.Individual, UserGroupCodes.Jobseeker, (int)Enums.ProfileTypes.JobSeeker)]
        [InlineData(UserTypeCodes.Individual, UserGroupCodes.Freelancer, (int)Enums.ProfileTypes.FreeLancer)]
        [InlineData(UserTypeCodes.Corporate, UserGroupCodes.SME, (int)Enums.ProfileTypes.SME)]
        [InlineData(UserTypeCodes.Corporate, UserGroupCodes.NonSME, (int)Enums.ProfileTypes.Non_SME)]
        [InlineData(UserTypeCodes.Corporate, UserGroupCodes.Operator, (int)Enums.ProfileTypes.Operator)]
        [InlineData(UserTypeCodes.Corporate, UserGroupCodes.Staff, (int)Enums.ProfileTypes.Staff)]
        [InlineData(UserTypeCodes.Corporate, UserGroupCodes.Contractor, (int)Enums.ProfileTypes.Contractor)]
        public void TryMapProfileType_KnownGroups(string type, string group, int expected)
        {
            Assert.True(IdentityAccessAdminRules.TryMapProfileType(type, group, out var profileType));
            Assert.Equal(expected, profileType);
        }

        [Theory]
        [InlineData(UserTypeCodes.Corporate, UserGroupCodes.Provider)]
        [InlineData(UserTypeCodes.Corporate, UserGroupCodes.Trainer)]
        [InlineData(UserTypeCodes.Individual, UserGroupCodes.Trainer)]
        [InlineData(UserTypeCodes.Individual, UserGroupCodes.Trainee)]
        public void TryMapProfileType_UnmappedGroups_ReturnsFalse(string type, string group)
        {
            Assert.False(IdentityAccessAdminRules.TryMapProfileType(type, group, out _));
        }

        [Fact]
        public void NormalizePageSize_AllowsOnlyConfiguredSizes()
        {
            Assert.Equal(5, IdentityAccessAdminRules.NormalizePageSize(null));
            Assert.Equal(5, IdentityAccessAdminRules.NormalizePageSize(7));
            Assert.Equal(10, IdentityAccessAdminRules.NormalizePageSize(10));
            Assert.Equal(15, IdentityAccessAdminRules.NormalizePageSize(15));
        }

        [Fact]
        public void TemplateRoleName_UsesTypeDotGroup()
        {
            Assert.Equal("System.SystemAdmin", IdentityAccessAdminRules.TemplateRoleName(UserTypeCodes.System, UserGroupCodes.SystemAdmin));
        }

        [Fact]
        public void HasRequesterCapability_OnlyIndividual()
        {
            Assert.True(IdentityAccessAdminRules.HasRequesterCapability(UserTypeCodes.Individual));
            Assert.False(IdentityAccessAdminRules.HasRequesterCapability(UserTypeCodes.Corporate));
            Assert.False(IdentityAccessAdminRules.HasRequesterCapability(UserTypeCodes.System));
        }

        [Theory]
        [InlineData((int)Enums.ProfileTypes.JobSeeker)]
        [InlineData((int)Enums.ProfileTypes.FreeLancer)]
        [InlineData((int)Enums.ProfileTypes.SME)]
        [InlineData((int)Enums.ProfileTypes.Operator)]
        [InlineData((int)Enums.ProfileTypes.Non_SME)]
        public void CanCreateServiceRequest_AllowsIndividualAndExistingCorporate(int profileType)
        {
            Assert.True(IdentityAccessAdminRules.CanCreateServiceRequest(profileType));
        }

        [Theory]
        [InlineData((int)Enums.ProfileTypes.Admin)]
        [InlineData((int)Enums.ProfileTypes.Staff)]
        [InlineData((int)Enums.ProfileTypes.Contractor)]
        public void CanCreateServiceRequest_RejectsNonRequesterProfiles(int profileType)
        {
            Assert.False(IdentityAccessAdminRules.CanCreateServiceRequest(profileType));
        }

        [Fact]
        public void CanRemoveAccount_RequiresSiblingAndNotSelf()
        {
            Assert.False(IdentityAccessAdminRules.CanRemoveAccount(true, 3, false));
            Assert.False(IdentityAccessAdminRules.CanRemoveAccount(false, 1, false));
            Assert.False(IdentityAccessAdminRules.CanRemoveAccount(false, 2, true));
            Assert.True(IdentityAccessAdminRules.CanRemoveAccount(false, 2, false));
        }

        [Fact]
        public void BuildAccountEmail_UsesPlusAddress()
        {
            Assert.Equal("a+Freelancer@b.com", IdentityAccessAdminRules.BuildAccountEmail("a@b.com", "Freelancer"));
        }

        [Fact]
        public void DefaultActionFlags_ByUserType()
        {
            Assert.Equal(PageActionFlags.All, IdentityAccessAdminRules.DefaultActionFlags(UserTypeCodes.System));
            Assert.Equal(
                PageActionFlags.View | PageActionFlags.Create | PageActionFlags.Update | PageActionFlags.Approve,
                IdentityAccessAdminRules.DefaultActionFlags(UserTypeCodes.Corporate));
            Assert.Equal(
                PageActionFlags.View | PageActionFlags.Create | PageActionFlags.Update,
                IdentityAccessAdminRules.DefaultActionFlags(UserTypeCodes.Individual));
            Assert.Equal(PageActionFlags.View, IdentityAccessAdminRules.DefaultActionFlags("Unknown"));
        }

        [Fact]
        public void DefaultDataScope_ByUserType()
        {
            Assert.Equal(PageDataScopes.Everything, IdentityAccessAdminRules.DefaultDataScope(UserTypeCodes.System));
            Assert.Equal(PageDataScopes.Organization, IdentityAccessAdminRules.DefaultDataScope(UserTypeCodes.Corporate));
            Assert.Equal(PageDataScopes.Mine, IdentityAccessAdminRules.DefaultDataScope(UserTypeCodes.Individual));
        }

        [Fact]
        public void NormalizeActionFlags_MasksAndDefaultsToView()
        {
            Assert.Equal(PageActionFlags.View, IdentityAccessAdminRules.NormalizeActionFlags(null));
            Assert.Equal(PageActionFlags.View, IdentityAccessAdminRules.NormalizeActionFlags(0));
            Assert.Equal(PageActionFlags.View | PageActionFlags.Create, IdentityAccessAdminRules.NormalizeActionFlags(3));
            Assert.Equal(PageActionFlags.All, IdentityAccessAdminRules.NormalizeActionFlags(PageActionFlags.All | 1024));
        }

        [Theory]
        [InlineData("mine", PageDataScopes.Mine)]
        [InlineData("ORGANIZATION", PageDataScopes.Organization)]
        [InlineData("department", PageDataScopes.Department)]
        [InlineData("assigned", PageDataScopes.Assigned)]
        [InlineData("everything", PageDataScopes.Everything)]
        public void NormalizeDataScope_KnownValues(string input, string expected)
        {
            Assert.Equal(expected, IdentityAccessAdminRules.NormalizeDataScope(input, UserTypeCodes.Corporate));
        }

        [Fact]
        public void NormalizeDataScope_UnknownFallsBackToTypeDefault()
        {
            Assert.Equal(PageDataScopes.Organization, IdentityAccessAdminRules.NormalizeDataScope("team", UserTypeCodes.Corporate));
            Assert.Equal(PageDataScopes.Mine, IdentityAccessAdminRules.NormalizeDataScope(null, UserTypeCodes.Individual));
        }

        [Fact]
        public void HasAction_RequiresExactFlag()
        {
            var granted = PageActionFlags.View | PageActionFlags.Create;
            Assert.True(IdentityAccessAdminRules.HasAction(granted, PageActionFlags.View));
            Assert.True(IdentityAccessAdminRules.HasAction(granted, PageActionFlags.Create));
            Assert.False(IdentityAccessAdminRules.HasAction(granted, PageActionFlags.Approve));
            Assert.False(IdentityAccessAdminRules.HasAction(granted, 0));
        }

        [Fact]
        public void MeetsRequiredActions_AllRequiredBitsMustBeGranted()
        {
            var required = PageActionFlags.View | PageActionFlags.Award;
            Assert.True(IdentityAccessAdminRules.MeetsRequiredActions(PageActionFlags.All, required));
            Assert.False(IdentityAccessAdminRules.MeetsRequiredActions(PageActionFlags.View, required));
        }

        [Fact]
        public void WiderDataScope_PicksBroaderGrant()
        {
            Assert.Equal(PageDataScopes.Everything, IdentityAccessAdminRules.WiderDataScope(PageDataScopes.Mine, PageDataScopes.Everything));
            Assert.Equal(PageDataScopes.Organization, IdentityAccessAdminRules.WiderDataScope(PageDataScopes.Mine, PageDataScopes.Organization));
            Assert.Equal(PageDataScopes.Department, IdentityAccessAdminRules.WiderDataScope(PageDataScopes.Assigned, PageDataScopes.Department));
        }

        [Fact]
        public void MergeActionFlags_UnionsBits()
        {
            Assert.Equal(
                PageActionFlags.View | PageActionFlags.Create | PageActionFlags.Award,
                IdentityAccessAdminRules.MergeActionFlags(PageActionFlags.View | PageActionFlags.Create, PageActionFlags.Award));
        }

        [Fact]
        public void CanMutateUserType_AlwaysFalse()
        {
            Assert.False(IdentityAccessAdminRules.CanMutateUserType());
        }

        [Fact]
        public void IsKnownUserType_OnlyThreeFixedTypes()
        {
            Assert.True(IdentityAccessAdminRules.IsKnownUserType(UserTypeCodes.System));
            Assert.True(IdentityAccessAdminRules.IsKnownUserType(UserTypeCodes.Corporate));
            Assert.True(IdentityAccessAdminRules.IsKnownUserType(UserTypeCodes.Individual));
            Assert.False(IdentityAccessAdminRules.IsKnownUserType("Partner"));
        }

        [Theory]
        [InlineData("SME", true)]
        [InlineData("Jobseeker", true)]
        [InlineData("A1", true)]
        [InlineData("s", false)]
        [InlineData("1SME", false)]
        [InlineData("SM E", false)]
        [InlineData("", false)]
        public void IsValidGroupCode_AlphanumericStartingWithLetter(string code, bool expected)
        {
            Assert.Equal(expected, IdentityAccessAdminRules.IsValidGroupCode(code));
        }

        [Fact]
        public void CanCreateUserGroup_RequiresKnownTypeUniqueValidCode()
        {
            Assert.True(IdentityAccessAdminRules.CanCreateUserGroup(UserTypeCodes.Corporate, "Partner", false));
            Assert.False(IdentityAccessAdminRules.CanCreateUserGroup(UserTypeCodes.Corporate, "Partner", true));
            Assert.False(IdentityAccessAdminRules.CanCreateUserGroup("Partner", "Partner", false));
            Assert.False(IdentityAccessAdminRules.CanCreateUserGroup(UserTypeCodes.Corporate, "x", false));
        }

        [Fact]
        public void CanDeactivateUserGroup_BlocksSystemAdminAndGroupsWithUsers()
        {
            Assert.False(IdentityAccessAdminRules.CanDeactivateUserGroup(UserTypeCodes.System, UserGroupCodes.SystemAdmin, 0));
            Assert.False(IdentityAccessAdminRules.CanDeactivateUserGroup(UserTypeCodes.Corporate, UserGroupCodes.SME, 2));
            Assert.True(IdentityAccessAdminRules.CanDeactivateUserGroup(UserTypeCodes.Corporate, UserGroupCodes.SME, 0));
        }

        [Fact]
        public void IsLockedTemplateRoleName_LocksPlatformAdminAndAccountAdmin()
        {
            Assert.True(IdentityAccessAdminRules.IsLockedTemplateRoleName("System.SystemAdmin"));
            Assert.True(IdentityAccessAdminRules.IsLockedTemplateRoleName(IdentityRoleNames.AccountAdmin));
            Assert.False(IdentityAccessAdminRules.IsLockedTemplateRoleName("Corporate.SME"));
        }

        [Fact]
        public void CanDeleteRole_OnlyUnlockedCustomRoles()
        {
            Assert.False(IdentityAccessAdminRules.CanDeleteRole(true, "Corporate.SME"));
            Assert.False(IdentityAccessAdminRules.CanDeleteRole(false, IdentityRoleNames.AccountAdmin));
            Assert.True(IdentityAccessAdminRules.CanDeleteRole(false, "Hiring lead"));
        }

        [Fact]
        public void CanCreateCustomRole_RequiresOrgTemplateParentAndPlainName()
        {
            Assert.True(IdentityAccessAdminRules.CanCreateCustomRole(12, true, "Hiring lead"));
            Assert.False(IdentityAccessAdminRules.CanCreateCustomRole(null, true, "Hiring lead"));
            Assert.False(IdentityAccessAdminRules.CanCreateCustomRole(12, false, "Hiring lead"));
            Assert.False(IdentityAccessAdminRules.CanCreateCustomRole(12, true, "x"));
            Assert.False(IdentityAccessAdminRules.CanCreateCustomRole(12, true, "Corporate.SME"));
            Assert.False(IdentityAccessAdminRules.CanCreateCustomRole(12, true, IdentityRoleNames.AccountAdmin));
        }

        [Fact]
        public void ResolveCustomRoleOrganizationId_PrefersBodyWhenPositive()
        {
            Assert.Equal(12, IdentityAccessAdminRules.ResolveCustomRoleOrganizationId(12, 99));
            Assert.Equal(99, IdentityAccessAdminRules.ResolveCustomRoleOrganizationId(0, 99));
            Assert.Equal(99, IdentityAccessAdminRules.ResolveCustomRoleOrganizationId(null, 99));
            Assert.Null(IdentityAccessAdminRules.ResolveCustomRoleOrganizationId(null, null));
            Assert.Null(IdentityAccessAdminRules.ResolveCustomRoleOrganizationId(0, null));
        }

        [Fact]
        public void ResolveLoginVendorType_UsesProfileType_IgnoresMissingProfile()
        {
            Assert.Equal(1, IdentityAccessAdminRules.ResolveLoginVendorType(1, 0));
            Assert.Equal(4, IdentityAccessAdminRules.ResolveLoginVendorType(null, 4));
            Assert.Equal(0, IdentityAccessAdminRules.ResolveLoginVendorType(null, 0));
        }

        [Fact]
        public void ResolveVendorType_FromGroup_StillMapsForAdminTools()
        {
            Assert.Equal(
                (int)Enums.ProfileTypes.SME,
                IdentityAccessAdminRules.ResolveVendorType(UserTypeCodes.Corporate, UserGroupCodes.SME, 0));
            Assert.Equal(
                99,
                IdentityAccessAdminRules.ResolveVendorType(null, null, 99));
        }

        [Fact]
        public void MatchesTemplatesOnlyFilter_TrueFalseNull()
        {
            Assert.True(IdentityAccessAdminRules.MatchesTemplatesOnlyFilter(true, true));
            Assert.False(IdentityAccessAdminRules.MatchesTemplatesOnlyFilter(false, true));
            Assert.True(IdentityAccessAdminRules.MatchesTemplatesOnlyFilter(false, false));
            Assert.False(IdentityAccessAdminRules.MatchesTemplatesOnlyFilter(true, false));
            Assert.True(IdentityAccessAdminRules.MatchesTemplatesOnlyFilter(true, null));
            Assert.True(IdentityAccessAdminRules.MatchesTemplatesOnlyFilter(false, null));
        }

        [Fact]
        public void ResolveRoleDisplayNames_Template_UsesGroupBilingualNames()
        {
            var (en, ar) = IdentityAccessAdminRules.ResolveRoleDisplayNames(
                isTemplate: true,
                roleName: "Corporate.SME",
                userGroupNameEn: "Small and medium enterprise",
                userGroupNameAr: "منشأة صغيرة ومتوسطة");

            Assert.Equal("Small and medium enterprise", en);
            Assert.Equal("منشأة صغيرة ومتوسطة", ar);
            Assert.DoesNotContain(".", en);
            Assert.DoesNotContain("Corporate", en);
        }

        [Fact]
        public void ResolveRoleDisplayNames_Custom_UsesRoleName()
        {
            var (en, ar) = IdentityAccessAdminRules.ResolveRoleDisplayNames(
                isTemplate: false,
                roleName: "Ops Custom",
                userGroupNameEn: "Staff member",
                userGroupNameAr: "موظف");

            Assert.Equal("Ops Custom", en);
            Assert.Equal("Ops Custom", ar);
        }

        [Fact]
        public void ResolveRoleDisplayNames_TemplateMissingGroup_FallsBackToRoleName()
        {
            var (en, ar) = IdentityAccessAdminRules.ResolveRoleDisplayNames(
                isTemplate: true,
                roleName: "AccountAdmin",
                userGroupNameEn: null,
                userGroupNameAr: "  ");

            Assert.Equal("AccountAdmin", en);
            Assert.Equal("AccountAdmin", ar);
        }

        [Fact]
        public void CanAssignCustomRole_SameOrganizationOnly()
        {
            Assert.True(IdentityAccessAdminRules.CanAssignCustomRole(9, 9));
            Assert.False(IdentityAccessAdminRules.CanAssignCustomRole(9, 8));
            Assert.False(IdentityAccessAdminRules.CanAssignCustomRole(null, 9));
        }

        [Fact]
        public void ShouldAutoGrantAccountAdmin_FirstCorporateOnOrg()
        {
            Assert.True(IdentityAccessAdminRules.ShouldAutoGrantAccountAdmin(UserTypeCodes.Corporate, 0));
            Assert.False(IdentityAccessAdminRules.ShouldAutoGrantAccountAdmin(UserTypeCodes.Corporate, 1));
            Assert.False(IdentityAccessAdminRules.ShouldAutoGrantAccountAdmin(UserTypeCodes.Individual, 0));
        }

        [Fact]
        public void CanRemoveAccountAdmin_RequiresAnotherAdmin()
        {
            Assert.False(IdentityAccessAdminRules.CanRemoveAccountAdmin(0));
            Assert.True(IdentityAccessAdminRules.CanRemoveAccountAdmin(1));
        }

        [Fact]
        public void CanCreateAdminOrganizationUser_SystemOnly()
        {
            Assert.True(IdentityAccessAdminRules.CanCreateAdminOrganizationUser(UserTypeCodes.System));
            Assert.False(IdentityAccessAdminRules.CanCreateAdminOrganizationUser(UserTypeCodes.Corporate));
            Assert.False(IdentityAccessAdminRules.CanCreateAdminOrganizationUser(UserTypeCodes.Individual));
            Assert.False(IdentityAccessAdminRules.CanCreateAdminOrganizationUser(null));
        }

        [Fact]
        public void HasCreateAdminUserRequiredFields_RequiresAll()
        {
            Assert.True(IdentityAccessAdminRules.HasCreateAdminUserRequiredFields("Ada", "ada@example.com", "Secret1!", 3));
            Assert.False(IdentityAccessAdminRules.HasCreateAdminUserRequiredFields(" ", "ada@example.com", "Secret1!", 3));
            Assert.False(IdentityAccessAdminRules.HasCreateAdminUserRequiredFields("Ada", "ada@example.com", "Secret1!", 0));
        }

        [Fact]
        public void HasCreateAdminUserRequiredFields_RejectsBlankEmailOrPassword()
        {
            Assert.False(IdentityAccessAdminRules.HasCreateAdminUserRequiredFields("Ada", "  ", "Secret1!", 3));
            Assert.False(IdentityAccessAdminRules.HasCreateAdminUserRequiredFields("Ada", "ada@example.com", "", 3));
        }

        [Fact]
        public void CanChangeUserGroup_IsFalse()
        {
            Assert.False(IdentityAccessAdminRules.CanChangeUserGroup());
        }

        [Fact]
        public void HasUpdateAdminUserRequiredFields_RequiresNameAndEmail()
        {
            Assert.True(IdentityAccessAdminRules.HasUpdateAdminUserRequiredFields("Ada", "ada@example.com"));
            Assert.False(IdentityAccessAdminRules.HasUpdateAdminUserRequiredFields(" ", "ada@example.com"));
            Assert.False(IdentityAccessAdminRules.HasUpdateAdminUserRequiredFields("Ada", "  "));
        }

        [Fact]
        public void CanSetUserActive_SelfCannotDeactivate()
        {
            Assert.False(IdentityAccessAdminRules.CanSetUserActive(isSelf: true, nextIsActive: false));
            Assert.True(IdentityAccessAdminRules.CanSetUserActive(isSelf: true, nextIsActive: true));
            Assert.True(IdentityAccessAdminRules.CanSetUserActive(isSelf: false, nextIsActive: false));
        }

        [Fact]
        public void BypassPageGrants_OnlySystemAdmin()
        {
            Assert.True(IdentityAccessAdminRules.BypassPageGrants(UserTypeCodes.System, UserGroupCodes.SystemAdmin));
            Assert.False(IdentityAccessAdminRules.BypassPageGrants(UserTypeCodes.Corporate, UserGroupCodes.SME));
        }

        [Fact]
        public void PageUrlMatches_ExactAndRouteParams()
        {
            Assert.True(IdentityAccessAdminRules.PageUrlMatches("/jobs/apply/:id", "/jobs/apply/55"));
            Assert.True(IdentityAccessAdminRules.PageUrlMatches("/admin/users", "/admin/users/"));
            Assert.False(IdentityAccessAdminRules.PageUrlMatches("/admin/users", "/admin/roles"));
            Assert.False(IdentityAccessAdminRules.PageUrlMatches("/jobs/apply/:id", "/jobs/apply"));
        }

        [Fact]
        public void CanShowNav_BypassAlwaysVisibleOrGranted()
        {
            Assert.True(IdentityAccessAdminRules.CanShowNav("/admin/roles", new string[0], true));
            Assert.True(IdentityAccessAdminRules.CanShowNav("/admin/dashboard", new string[0], false));
            Assert.True(IdentityAccessAdminRules.CanShowNav("/jobs/apply/9", new[] { "/jobs/apply/:id" }, false));
            Assert.False(IdentityAccessAdminRules.CanShowNav("/admin/users", new[] { "/admin/roles" }, false));
        }

        [Fact]
        public void PermissionDefaults_FromRoleName()
        {
            Assert.Equal(PageActionFlags.All, IdentityAccessPermissionDefaults.ActionFlagsForRoleName("System.SystemAdmin"));
            Assert.Equal(PageActionFlags.All, IdentityAccessPermissionDefaults.ActionFlagsForRoleName(IdentityRoleNames.AccountAdmin));
            Assert.Equal(
                PageActionFlags.View | PageActionFlags.Create | PageActionFlags.Update | PageActionFlags.Approve,
                IdentityAccessPermissionDefaults.ActionFlagsForRoleName("Corporate.SME"));
            Assert.Equal(PageDataScopes.Organization, IdentityAccessPermissionDefaults.DataScopeForRoleName("Corporate.Operator"));
            Assert.Equal(PageDataScopes.Mine, IdentityAccessPermissionDefaults.DataScopeForRoleName("Individual.Jobseeker"));
        }

        [Theory]
        [InlineData((int)Enums.ProfileTypes.JobSeeker, UserTypeCodes.Individual, UserGroupCodes.Jobseeker, false)]
        [InlineData((int)Enums.ProfileTypes.FreeLancer, UserTypeCodes.Individual, UserGroupCodes.Freelancer, false)]
        [InlineData((int)Enums.ProfileTypes.Operator, UserTypeCodes.Corporate, UserGroupCodes.Operator, true)]
        public void TryMapLegacyProfileType_RegistrationTargets(
            int profileType,
            string expectedType,
            string expectedGroup,
            bool needsOrg)
        {
            Assert.True(IdentityAccessAdminRules.TryMapLegacyProfileType(
                profileType,
                out var type,
                out var group,
                out var mappedNeedsOrg,
                out _));
            Assert.Equal(expectedType, type);
            Assert.Equal(expectedGroup, group);
            Assert.Equal(needsOrg, mappedNeedsOrg);
        }

        [Fact]
        public void CanOperatorDistributeRole_TemplateStaff_IsTrue()
        {
            Assert.True(IdentityAccessAdminRules.CanOperatorDistributeRole(
                10,
                roleIsTemplate: true,
                roleIsActive: true,
                roleOrganizationId: null,
                roleUserTypeCode: UserTypeCodes.Corporate,
                roleUserGroupCode: UserGroupCodes.Staff));
        }

        [Fact]
        public void CanOperatorDistributeRole_OrgCustom_OnlySameOrg()
        {
            Assert.True(IdentityAccessAdminRules.CanOperatorDistributeRole(
                10,
                roleIsTemplate: false,
                roleIsActive: true,
                roleOrganizationId: 10,
                roleUserTypeCode: UserTypeCodes.Corporate,
                roleUserGroupCode: UserGroupCodes.Staff));
            Assert.False(IdentityAccessAdminRules.CanOperatorDistributeRole(
                10,
                roleIsTemplate: false,
                roleIsActive: true,
                roleOrganizationId: 99,
                roleUserTypeCode: UserTypeCodes.Corporate,
                roleUserGroupCode: UserGroupCodes.Staff));
        }

        [Fact]
        public void CanOperatorDistributeRole_OperatorTemplate_IsFalse()
        {
            Assert.False(IdentityAccessAdminRules.CanOperatorDistributeRole(
                10,
                roleIsTemplate: true,
                roleIsActive: true,
                roleOrganizationId: null,
                roleUserTypeCode: UserTypeCodes.Corporate,
                roleUserGroupCode: UserGroupCodes.Operator));
        }
    }
}

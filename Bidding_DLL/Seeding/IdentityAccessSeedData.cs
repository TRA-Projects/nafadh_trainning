using System;
using System.Collections.Generic;
using System.Linq;
using Bidding_Models.DatabaseModel.IdentityAccess;

namespace Bidding_DLL.Seeding
{
    /// <summary>Static catalog for identity access seed (types, groups, claims, pages, role grants).</summary>
    public static class IdentityAccessSeedData
    {
        public sealed class UserTypeSeed
        {
            public string Code { get; set; }
            public string NameEn { get; set; }
            public string NameAr { get; set; }
            public int SortOrder { get; set; }
        }

        public sealed class UserGroupSeed
        {
            public string UserTypeCode { get; set; }
            public string Code { get; set; }
            public string NameEn { get; set; }
            public string NameAr { get; set; }
            public int SortOrder { get; set; }
        }

        public sealed class ClaimSeed
        {
            public string ClaimName { get; set; }
            public string NameEn { get; set; }
            public string NameAr { get; set; }
        }

        public sealed class PageSeed
        {
            public string Code { get; set; }
            public string SystemCode { get; set; }
            public string Url { get; set; }
            public string NameEn { get; set; }
            public string NameAr { get; set; }
            public int SortOrder { get; set; }
        }

        public static string RoleName(string userTypeCode, string groupCode)
            => $"{userTypeCode}.{groupCode}";

        public static readonly IReadOnlyList<UserTypeSeed> UserTypes = new[]
        {
            new UserTypeSeed { Code = UserTypeCodes.System, NameEn = "System", NameAr = "النظام", SortOrder = 1 },
            new UserTypeSeed { Code = UserTypeCodes.Corporate, NameEn = "Company", NameAr = "شركة", SortOrder = 2 },
            new UserTypeSeed { Code = UserTypeCodes.Individual, NameEn = "Individual", NameAr = "فرد", SortOrder = 3 },
        };

        public static readonly IReadOnlyList<UserGroupSeed> UserGroups = new[]
        {
            new UserGroupSeed { UserTypeCode = UserTypeCodes.System, Code = UserGroupCodes.SystemAdmin, NameEn = "System administrator", NameAr = "مسؤول النظام", SortOrder = 1 },

            new UserGroupSeed { UserTypeCode = UserTypeCodes.Corporate, Code = UserGroupCodes.SME, NameEn = "Small and medium enterprise", NameAr = "منشأة صغيرة ومتوسطة", SortOrder = 1 },
            new UserGroupSeed { UserTypeCode = UserTypeCodes.Corporate, Code = UserGroupCodes.NonSME, NameEn = "Large enterprise", NameAr = "منشأة كبيرة", SortOrder = 2 },
            new UserGroupSeed { UserTypeCode = UserTypeCodes.Corporate, Code = UserGroupCodes.Operator, NameEn = "Operator", NameAr = "مشغّل", SortOrder = 3 },
            new UserGroupSeed { UserTypeCode = UserTypeCodes.Corporate, Code = UserGroupCodes.Provider, NameEn = "Service provider", NameAr = "مزوّد خدمة", SortOrder = 4 },
            new UserGroupSeed { UserTypeCode = UserTypeCodes.Corporate, Code = UserGroupCodes.Trainer, NameEn = "Trainer", NameAr = "مدرب", SortOrder = 5 },
            new UserGroupSeed { UserTypeCode = UserTypeCodes.Corporate, Code = UserGroupCodes.Staff, NameEn = "Staff member", NameAr = "موظف", SortOrder = 6 },
            new UserGroupSeed { UserTypeCode = UserTypeCodes.Corporate, Code = UserGroupCodes.Contractor, NameEn = "Contractor", NameAr = "مقاول", SortOrder = 7 },

            new UserGroupSeed { UserTypeCode = UserTypeCodes.Individual, Code = UserGroupCodes.Jobseeker, NameEn = "Job seeker", NameAr = "باحث عن عمل", SortOrder = 1 },
            new UserGroupSeed { UserTypeCode = UserTypeCodes.Individual, Code = UserGroupCodes.Freelancer, NameEn = "Freelancer", NameAr = "مستقل", SortOrder = 2 },
            new UserGroupSeed { UserTypeCode = UserTypeCodes.Individual, Code = UserGroupCodes.Trainer, NameEn = "Trainer", NameAr = "مدرب", SortOrder = 3 },
            new UserGroupSeed { UserTypeCode = UserTypeCodes.Individual, Code = UserGroupCodes.Trainee, NameEn = "Trainee", NameAr = "متدرب", SortOrder = 4 },
        };

        public static readonly IReadOnlyList<ClaimSeed> Claims = new[]
        {
            new ClaimSeed { ClaimName = "profile.read", NameEn = "Read profile", NameAr = "قراءة الملف" },
            new ClaimSeed { ClaimName = "profile.write", NameEn = "Write profile", NameAr = "تعديل الملف" },
            new ClaimSeed { ClaimName = "org.read", NameEn = "Read organization", NameAr = "قراءة المؤسسة" },
            new ClaimSeed { ClaimName = "org.manage", NameEn = "Manage organization", NameAr = "إدارة المؤسسة" },
            new ClaimSeed { ClaimName = "account.admin", NameEn = "Account admin", NameAr = "مسؤول الحساب" },
            new ClaimSeed { ClaimName = "users.manage", NameEn = "Manage users", NameAr = "إدارة المستخدمين" },
            new ClaimSeed { ClaimName = "roles.manage", NameEn = "Manage roles", NameAr = "إدارة الأدوار" },
            new ClaimSeed { ClaimName = "jobs.read", NameEn = "Read jobs", NameAr = "قراءة الوظائف" },
            new ClaimSeed { ClaimName = "jobs.write", NameEn = "Write jobs", NameAr = "تعديل الوظائف" },
            new ClaimSeed { ClaimName = "applications.read", NameEn = "Read applications", NameAr = "قراءة الطلبات" },
            new ClaimSeed { ClaimName = "applications.write", NameEn = "Write applications", NameAr = "تعديل الطلبات" },
            new ClaimSeed { ClaimName = "interviews.read", NameEn = "Read interviews", NameAr = "قراءة المقابلات" },
            new ClaimSeed { ClaimName = "interviews.write", NameEn = "Write interviews", NameAr = "تعديل المقابلات" },
            new ClaimSeed { ClaimName = "offers.read", NameEn = "Read offers", NameAr = "قراءة العروض" },
            new ClaimSeed { ClaimName = "offers.write", NameEn = "Write offers", NameAr = "تعديل العروض" },
            new ClaimSeed { ClaimName = "training.read", NameEn = "Read training", NameAr = "قراءة التدريب" },
            new ClaimSeed { ClaimName = "training.write", NameEn = "Write training", NameAr = "تعديل التدريب" },
            new ClaimSeed { ClaimName = "requests.read", NameEn = "Read service requests", NameAr = "قراءة طلبات الخدمة" },
            new ClaimSeed { ClaimName = "requests.write", NameEn = "Write service requests", NameAr = "تعديل طلبات الخدمة" },
            new ClaimSeed { ClaimName = "requests.award", NameEn = "Award service requests", NameAr = "ترسية طلبات الخدمة" },
            new ClaimSeed { ClaimName = "system.admin", NameEn = "System admin", NameAr = "إدارة النظام" },
        };

        /// <summary>All navigable URLs from JobseekerModule, FreelancerModule, Bidding_Web.</summary>
        public static readonly IReadOnlyList<PageSeed> Pages = IdentityAccessAppPagesSeedData.All;


        private static readonly string[] CorporateEmployerClaims =
        {
            "profile.read", "profile.write", "org.read", "jobs.read", "jobs.write",
            "applications.read", "interviews.read", "interviews.write", "offers.read", "offers.write",
            "requests.read", "requests.write", "requests.award"
        };

        private static readonly string[] IndividualRequesterClaims =
        {
            "requests.read", "requests.write", "requests.award"
        };

        private static readonly string[] TrainerClaims =
        {
            "profile.read", "profile.write", "training.read", "training.write"
        };

        public static readonly IReadOnlyDictionary<string, string[]> RoleClaims = BuildRoleClaims();
        public static readonly IReadOnlyDictionary<string, string[]> RolePages = BuildRolePages();
        public static readonly IReadOnlyDictionary<string, string> TemplateRoleIds = BuildTemplateRoleIds();

        private static string[] PagesMatching(params string[] prefixes)
        {
            return Pages
                .Where(p => prefixes.Any(prefix =>
                    p.Code.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
                .Select(p => p.Code)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static IReadOnlyDictionary<string, string[]> BuildRoleClaims()
        {
            var d = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                [IdentityRoleNames.AccountAdmin] = new[]
                {
                    "account.admin", "org.read", "org.manage", "users.manage", "roles.manage", "profile.read", "profile.write"
                },
                [RoleName(UserTypeCodes.System, UserGroupCodes.SystemAdmin)] = new[]
                {
                    "system.admin", "profile.read", "profile.write", "org.read", "org.manage", "account.admin",
                    "users.manage", "roles.manage", "jobs.read", "jobs.write", "applications.read", "applications.write",
                    "interviews.read", "interviews.write", "offers.read", "offers.write", "training.read", "training.write",
                    "requests.read", "requests.write", "requests.award"
                },
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.SME)] = CorporateEmployerClaims,
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.NonSME)] = CorporateEmployerClaims,
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.Operator)] = CorporateEmployerClaims,
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.Provider)] = new[]
                {
                    "profile.read", "profile.write", "org.read", "jobs.read", "applications.read", "training.read", "training.write"
                },
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.Contractor)] = new[]
                {
                    "profile.read", "profile.write", "org.read", "jobs.read", "applications.read"
                },
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.Trainer)] = TrainerClaims,
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.Staff)] = new[]
                {
                    "profile.read", "profile.write", "org.read", "jobs.read", "applications.read", "interviews.read"
                },
                [RoleName(UserTypeCodes.Individual, UserGroupCodes.Jobseeker)] = new[]
                {
                    "profile.read", "profile.write", "jobs.read", "applications.read", "applications.write",
                    "interviews.read", "offers.read"
                }.Concat(IndividualRequesterClaims).ToArray(),
                [RoleName(UserTypeCodes.Individual, UserGroupCodes.Freelancer)] = new[]
                {
                    "profile.read", "profile.write", "jobs.read", "applications.read", "applications.write"
                }.Concat(IndividualRequesterClaims).ToArray(),
                [RoleName(UserTypeCodes.Individual, UserGroupCodes.Trainer)] = TrainerClaims.Concat(IndividualRequesterClaims).ToArray(),
                [RoleName(UserTypeCodes.Individual, UserGroupCodes.Trainee)] = new[]
                {
                    "profile.read", "profile.write", "training.read"
                }.Concat(IndividualRequesterClaims).ToArray(),
            };
            return d;
        }

        private static IReadOnlyDictionary<string, string[]> BuildRolePages()
        {
            var all = Pages.Select(p => p.Code).ToArray();
            var jobseekerApp = PagesMatching(
                "js.jobseeker.", "js.jobs.", "js.applications.", "js.profile", "js.post-employment",
                "js.contracts.", "js.settings", "js.support", "js.complete-profile", "js.login", "js.register.");
            var freelancerApp = PagesMatching(
                "fl.dashboard.freelancer", "fl.projects.find", "fl.projects.favorites", "fl.projects.apply",
                "fl.projects.application", "fl.projects.biddings", "fl.projects.bid", "fl.account.user",
                "fl.account.portfolio", "fl.account.freelancer", "fl.account.skills", "fl.account.education",
                "fl.account.certification", "fl.account.professional", "fl.contract.freelancer", "fl.contract.details",
                "fl.contract.certificate", "fl.payment.f", "fl.payment.freelancer", "fl.home.chats", "fl.support.");
            var corporateEmployer = PagesMatching(
                "js.operator.", "fl.dashboard.operator", "fl.projects.list", "fl.projects.post", "fl.projects.applicants",
                "fl.projects.application-details", "fl.projects.overview", "fl.projects.completed",
                "fl.account.operator", "fl.contract.contracts", "fl.contract.create", "fl.contract.op",
                "fl.contract.review", "fl.payment.op", "fl.payment.dispute", "fl.payment.milestone",
                "nw.common.", "nw.user.");
            var vendorNw = PagesMatching("nw.vendor.", "nw.common.dashboard", "nw.common.profile", "nw.common.proposals", "nw.common.bidding");
            var trainerPages = PagesMatching("nw.training.", "nw.common.trainings", "js.profile", "fl.account.user-profile");
            var traineePages = PagesMatching("nw.training.index", "nw.training.login", "nw.common.trainings", "js.profile");
            var individualRequester = PagesMatching(
                "nw.user.", "nw.common.my-draft", "nw.common.request-detail", "nw.common.pending-requests",
                "nw.common.awarded-requests", "nw.common.evaluating-requests", "nw.common.active-requests",
                "fl.projects.post", "fl.projects.list", "fl.projects.applicants", "fl.projects.application-details",
                "fl.projects.overview", "fl.projects.completed");
            var staffPages = PagesMatching("js.operator.jobs.", "js.operator.interview", "js.operator.view-", "js.operator.profile", "fl.projects.list", "fl.projects.applicants");
            var accountAdminPages = PagesMatching("js.operator.company-users", "js.operator.edit-user", "js.operator.profile", "nw.common.accounts", "fl.admin.user-management");
            var adminAll = PagesMatching("js.admin.", "fl.admin.", "nw.admin.");
            var everyRole = PagesMatching("js.notifications");

            var map = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                [IdentityRoleNames.AccountAdmin] = accountAdminPages,
                [RoleName(UserTypeCodes.System, UserGroupCodes.SystemAdmin)] = all,
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.SME)] = corporateEmployer,
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.NonSME)] = corporateEmployer,
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.Operator)] = corporateEmployer,
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.Provider)] = vendorNw.Concat(PagesMatching("js.operator.profile", "fl.dashboard.operator")).Distinct().ToArray(),
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.Contractor)] = vendorNw,
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.Trainer)] = trainerPages,
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.Staff)] = staffPages,
                [RoleName(UserTypeCodes.Individual, UserGroupCodes.Jobseeker)] = jobseekerApp.Concat(individualRequester).Distinct().ToArray(),
                [RoleName(UserTypeCodes.Individual, UserGroupCodes.Freelancer)] = freelancerApp.Concat(individualRequester).Distinct().ToArray(),
                [RoleName(UserTypeCodes.Individual, UserGroupCodes.Trainer)] = trainerPages.Concat(individualRequester).Distinct().ToArray(),
                [RoleName(UserTypeCodes.Individual, UserGroupCodes.Trainee)] = traineePages.Concat(individualRequester).Distinct().ToArray(),
            };

            foreach (var key in map.Keys.ToList())
            {
                map[key] = map[key].Concat(everyRole).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            }

            return map;
        }

        private static IReadOnlyDictionary<string, string> BuildTemplateRoleIds()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [IdentityRoleNames.AccountAdmin] = "a1000001-0001-4000-8000-000000000002",
                [RoleName(UserTypeCodes.System, UserGroupCodes.SystemAdmin)] = "a1000001-0001-4000-8000-000000000001",
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.SME)] = "a1000001-0001-4000-8000-000000000003",
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.NonSME)] = "a1000001-0001-4000-8000-000000000004",
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.Operator)] = "a1000001-0001-4000-8000-000000000005",
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.Provider)] = "a1000001-0001-4000-8000-000000000006",
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.Trainer)] = "a1000001-0001-4000-8000-000000000007",
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.Staff)] = "a1000001-0001-4000-8000-000000000008",
                [RoleName(UserTypeCodes.Corporate, UserGroupCodes.Contractor)] = "a1000001-0001-4000-8000-000000000009",
                [RoleName(UserTypeCodes.Individual, UserGroupCodes.Jobseeker)] = "a1000001-0001-4000-8000-00000000000a",
                [RoleName(UserTypeCodes.Individual, UserGroupCodes.Freelancer)] = "a1000001-0001-4000-8000-00000000000b",
                [RoleName(UserTypeCodes.Individual, UserGroupCodes.Trainer)] = "a1000001-0001-4000-8000-00000000000c",
                [RoleName(UserTypeCodes.Individual, UserGroupCodes.Trainee)] = "a1000001-0001-4000-8000-00000000000d",
            };
        }
    }
}

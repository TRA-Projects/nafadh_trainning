using System.Collections.Generic;
using Bidding_Models.DatabaseModel.IdentityAccess;

namespace Bidding_DLL.Seeding
{
    /// <summary>
    /// Navigable URLs collected from JobseekerModule, FreelancerModule, and Bidding_Web.
    /// Used for AppPages seed; role↔page grants and dynamic nav come later.
    /// </summary>
    public static class IdentityAccessAppPagesSeedData
    {
        private static IdentityAccessSeedData.PageSeed P(
            string system,
            string code,
            string url,
            string nameEn,
            string nameAr,
            int sort)
            => new IdentityAccessSeedData.PageSeed
            {
                SystemCode = system,
                Code = code,
                Url = url,
                NameEn = nameEn,
                NameAr = string.IsNullOrWhiteSpace(nameAr) ? nameEn : nameAr,
                SortOrder = sort
            };

        public static readonly IReadOnlyList<IdentityAccessSeedData.PageSeed> All = BuildAll();

        private static IReadOnlyList<IdentityAccessSeedData.PageSeed> BuildAll()
        {
            var list = new List<IdentityAccessSeedData.PageSeed>();
            list.AddRange(JobseekerPages());
            list.AddRange(FreelancerPages());
            list.AddRange(BiddingWebPages());
            return list;
        }

        private static IEnumerable<IdentityAccessSeedData.PageSeed> JobseekerPages()
        {
            const string s = AppPageSystemCodes.Jobseeker;
            var i = 1000;
            // Public / auth
            yield return P(s, "js.landing", "/landing", "Landing", "الصفحة الرئيسية", i++);
            yield return P(s, "js.landing.en", "/landing/en", "Landing (EN)", "الصفحة الرئيسية (إنجليزي)", i++);
            yield return P(s, "js.auth.pki-callback", "/auth/pki-callback", "PKI callback", "استدعاء PKI", i++);
            yield return P(s, "js.login", "/login", "Login", "تسجيل الدخول", i++);
            yield return P(s, "js.staff.login", "/staff/login", "Staff login", "دخول الموظفين", i++);
            yield return P(s, "js.register.jobseeker", "/register/jobseeker", "Jobseeker registration", "تسجيل باحث عن عمل", i++);
            yield return P(s, "js.register.jobseeker.pki", "/register/jobseeker/pki", "Jobseeker PKI registration", "تسجيل باحث عبر PKI", i++);
            yield return P(s, "js.register.verify-email", "/register/jobseeker/verify-email", "Verify email", "تأكيد البريد", i++);
            yield return P(s, "js.register.complete-profile", "/register/jobseeker/complete-profile", "Complete profile (registration)", "إكمال الملف", i++);
            yield return P(s, "js.complete-profile", "/complete-profile", "Complete profile", "إكمال الملف", i++);

            // Jobseeker authenticated
            yield return P(s, "js.jobseeker.dashboard", "/jobseeker/dashboard", "Dashboard", "لوحة التحكم", i++);
            yield return P(s, "js.jobs.find", "/jobs/find", "Find opportunities", "بحث عن فرص", i++);
            yield return P(s, "js.jobs.saved", "/jobs/saved", "Saved opportunities", "فرص محفوظة", i++);
            yield return P(s, "js.jobs.apply", "/jobs/apply/:id", "Apply for opportunity", "التقديم على فرصة", i++);
            yield return P(s, "js.profile", "/profile", "My Profile", "ملفي", i++);
            yield return P(s, "js.profile.recommendations", "/profile/recommendations", "Recommendations", "التوصيات", i++);
            yield return P(s, "js.applications.applied", "/applications/applied", "Applied opportunities", "فرص مقدّمة", i++);
            yield return P(s, "js.applications.history", "/applications/history", "Application History", "السجل", i++);
            yield return P(s, "js.jobseekers.me-requests", "/jobseekers/me-requests", "My requests", "طلباتي", i++);
            yield return P(s, "js.jobseekers.interviews", "/jobseekers/interviews", "My interviews", "مقابلاتي", i++);
            yield return P(s, "js.jobseekers.interviews.view", "/jobseekers/interviews/:id/view", "Interview details", "تفاصيل المقابلة", i++);
            yield return P(s, "js.jobseekers.offers", "/jobseekers/offers", "Contracts & Offers", "العقود و العروض", i++);
            yield return P(s, "js.jobseekers.offers.view", "/jobseekers/offers/view", "Offer details", "تفاصيل العرض", i++);
            yield return P(s, "js.jobseekers.submit-withdrawal", "/jobseekers/submit-withdrawl-request", "Submit withdrawal request", "تقديم طلب انسحاب", i++);
            yield return P(s, "js.jobseekers.withdrawal-requests", "/jobseekers/withdrawal-requests", "My withdrawal requests", "طلبات الانسحاب والإنهاء المبكر", i++);
            yield return P(s, "js.contracts.view", "/contracts/:id", "Contract details", "تفاصيل العقد", i++);
            yield return P(s, "js.post-employment", "/post-employment", "My post-employment records", "سجلاتي ما بعد التوظيف", i++);
            yield return P(s, "js.settings", "/settings", "Settings", "الإعدادات", i++);
            yield return P(s, "js.support", "/support", "Support", "الدعم", i++);
            yield return P(s, "js.notifications", "/notifications", "My notifications", "إشعاراتي", i++);
            yield return P(s, "js.dashboard", "/dashboard", "Dashboard", "لوحة التحكم", i++);

            // Operator
            yield return P(s, "js.operator.dashboard", "/operator/dashboard", "Operator dashboard", "لوحة المشغّل", i++);
            yield return P(s, "js.operator.jobs.list", "/operator/jobs/list", "Opportunities list", "قائمة الفرص", i++);
            yield return P(s, "js.operator.jobs.jobseekers", "/operator/jobs/jobseekers", "Job seekers list", "قائمة الباحثين عن عمل", i++);
            yield return P(s, "js.operator.jobs.post", "/operator/jobs/post", "Add opportunity", "إضافة فرصة", i++);
            yield return P(s, "js.operator.jobs.post.preview", "/operator/jobs/post/preview", "Opportunity preview", "معاينة الفرصة", i++);
            yield return P(s, "js.operator.view-application", "/operator/view-application", "Application details", "تفاصيل التقديم", i++);
            yield return P(s, "js.operator.profile", "/operator/profile", "Profile", "الملف", i++);
            yield return P(s, "js.operator.company-users", "/operator/company-users", "User administrator", "إدارة المستخدمين", i++);
            yield return P(s, "js.operator.company-users.register", "/operator/company-users/register", "Register user", "تسجيل مستخدم", i++);
            yield return P(s, "js.operator.edit-user", "/operator/edit-user", "Update user", "تحديث المستخدم", i++);
            yield return P(s, "js.operator.jobseeker-dashboard", "/operator/jobseeker-dashboard", "Job seeker dashboard", "لوحة الباحث عن عمل", i++);
            yield return P(s, "js.operator.evaluation.job-seekers", "/operator/evaluation-management/job-seekers", "Job seekers board", "لوحة الباحثين عن عمل", i++);
            yield return P(s, "js.operator.interview-scheduling", "/operator/interview-scheduling", "Schedule interviews", "جدولة مقابلات", i++);
            yield return P(s, "js.operator.interview-management", "/operator/interview-management", "Interview management", "إدارة المقابلات", i++);
            yield return P(s, "js.operator.create-interview", "/operator/create-interview", "New interview", "مقابلة جديدة", i++);
            yield return P(s, "js.operator.jobseeker-interviews", "/operator/jobseeker-interviews", "Job seeker interviews", "مقابلات الباحثين عن عمل", i++);
            yield return P(s, "js.operator.jobseeker-interviews.details", "/operator/jobseeker-interviews/details", "Interview details", "تفاصيل المقابلة", i++);
            yield return P(s, "js.operator.interviews-by-opportunity", "/operator/interviews-by-oportunity", "Opportunity interviews", "مقابلات الفرصة", i++);
            yield return P(s, "js.operator.view-interview", "/operator/view-interview", "View interview", "عرض المقابلة", i++);
            yield return P(s, "js.operator.edit-interview", "/operator/edit-interview", "Edit interview", "تعديل المقابلة", i++);
            yield return P(s, "js.operator.interview-conduct", "/operator/interview-conduct", "Conduct interview", "إجراء المقابلة", i++);
            yield return P(s, "js.operator.offers", "/operator/offers", "Contracts & Offers", "العقود و العروض", i++);
            yield return P(s, "js.operator.offers.view", "/operator/offers/view", "Offer", "العرض", i++);
            yield return P(s, "js.operator.prepare-offers", "/operator/prepare-offers", "Prepare offers", "تجهيز العروض", i++);
            yield return P(s, "js.operator.view-withdrawal-requests", "/operator/view-withdrawal-requests", "Withdrawal & Termination Requests", "طلبات الانسحاب والإنهاء المبكر", i++);
            yield return P(s, "js.operator.withdrawal-requests", "/operator/withdrawal-requests", "Withdrawal & Termination Requests", "طلبات الانسحاب والإنهاء المبكر", i++);
            yield return P(s, "js.operator.post-employment", "/operator/post-employment", "Post-Employment Procedures", "إجراءات ما بعد التوظيف", i++);
            yield return P(s, "js.operator.employment-ratings", "/operator/employment-ratings", "Employment ratings", "تقييمات التوظيف", i++);
            yield return P(s, "js.operator.create-rate-record", "/operator/create-rate-record", "New Employer Evaluation", "اضافة تقييم دوري", i++);
            yield return P(s, "js.operator.edit-rate-record", "/operator/edit-rate-record", "Employer Evaluation", "تقييم صاحب العمل", i++);

            // Admin
            yield return P(s, "js.admin.dashboard", "/admin/dashboard", "Admin dashboard", "لوحة الإدارة", i++);
            yield return P(s, "js.admin.users", "/admin/users", "Users", "المستخدمون", i++);
            yield return P(s, "js.admin.template-roles", "/admin/template-roles", "Template roles", "أدوار القوالب", i++);
            yield return P(s, "js.admin.roles", "/admin/roles", "Custom roles", "الأدوار المخصصة", i++);
            yield return P(s, "js.admin.groups", "/admin/groups", "Groups", "المجموعات", i++);
            yield return P(s, "js.admin.system-configuration", "/admin/system-configuration", "System configuration", "إعدادات النظام", i++);
            yield return P(s, "js.admin.constants-management", "/admin/constants-management", "Constants management", "إدارة الثوابت", i++);
            yield return P(s, "js.admin.notification-templates", "/admin/notification-templates", "Notification templates", "قوالب الإشعارات", i++);
            yield return P(s, "js.admin.audit-trail", "/admin/audit-trail", "Audit trail", "سجل التدقيق", i++);
            yield return P(s, "js.admin.notification-transactions", "/admin/notification-transactions", "Notification transactions", "سجل الإشعارات", i++);
            yield return P(s, "js.admin.custom-notifications", "/admin/custom-notifications", "Custom notifications", "الإشعارات المخصصة", i++);
            yield return P(s, "js.admin.skill-categories", "/admin/skill-categories", "Skill categories", "فئات المهارات", i++);
            yield return P(s, "js.admin.skills-taxonomy", "/admin/skills-taxonomy", "Skills", "المهارات", i++);
            yield return P(s, "js.admin.jobs.list", "/admin/jobs/list", "Opportunities list", "قائمة الفرص", i++);
            yield return P(s, "js.admin.jobs.jobseekers", "/admin/jobs/jobseekers", "Job seekers list", "قائمة الباحثين عن عمل", i++);
            yield return P(s, "js.admin.jobs.post", "/admin/jobs/post", "Add opportunity", "إضافة فرصة", i++);
            yield return P(s, "js.admin.view-application", "/admin/view-application", "Application details", "تفاصيل التقديم", i++);
            yield return P(s, "js.admin.tra-verification", "/admin/tra-verification", "TRA verification", "التحقق", i++);
            yield return P(s, "js.admin.profile-list", "/admin/profile-list", "Profile list", "قائمة الملفات", i++);
            yield return P(s, "js.admin.education-records", "/admin/education-records", "Education certificates", "شهادات التعليم", i++);
            yield return P(s, "js.admin.diploma-marks-verification", "/admin/diploma-marks-verification", "Diploma marks verification", "التحقق من درجات الدبلوم", i++);
            yield return P(s, "js.admin.credential-management", "/admin/credential-management", "Assessment management", "إدارة التقييم", i++);
            yield return P(s, "js.admin.credential-management.assessments", "/admin/credential-management/assessments", "Job seeker assessment records", "سجل تقييمات الباحثين", i++);
            yield return P(s, "js.admin.create-credential", "/admin/create-credential", "Create assessment", "إنشاء تقييم", i++);
            yield return P(s, "js.admin.edit-credential", "/admin/edit-credential", "Edit assessment", "تعديل التقييم", i++);
            yield return P(s, "js.admin.credential-interviews", "/admin/credential-interviews", "Credential interviews", "مقابلات التقييم", i++);
            yield return P(s, "js.admin.conduct-evaluation", "/admin/conduct-evaluation", "Conduct evaluation", "إجراء التقييم", i++);
            yield return P(s, "js.admin.view-evaluation", "/admin/view-evaluation", "View evaluation", "عرض التقييم", i++);
            yield return P(s, "js.admin.evaluation-scheduling", "/admin/evaluation-scheduling", "Evaluation scheduling", "جدولة التقييم", i++);
            yield return P(s, "js.admin.jobseekers-records", "/admin/jobseekers-records", "Job seekers board", "لوحة الباحثين عن عمل", i++);
            yield return P(s, "js.admin.jobseeker-recommendations", "/admin/jobseeker-recommendations", "Job seeker recommendations", "توصيات الباحث عن عمل", i++);
            yield return P(s, "js.admin.jobseeker-dashboard", "/admin/jobseeker-dashboard", "Job seeker dashboard", "لوحة الباحث عن عمل", i++);
            yield return P(s, "js.admin.interview-management", "/admin/interview-management", "Interview management", "إدارة المقابلات", i++);
            yield return P(s, "js.admin.interview-management.jobseeker-interviews", "/admin/interview-management/jobseeker-interviews", "Job seeker interviews", "مقابلات الباحثين عن عمل", i++);
            yield return P(s, "js.admin.view-interview", "/admin/view-interview", "View interview", "عرض المقابلة", i++);
            yield return P(s, "js.admin.employment-procedures", "/admin/employment-procedures", "Contracts & Offers", "العقود و العروض", i++);
            yield return P(s, "js.admin.register-contracts", "/admin/register-contracts", "Register contracts", "تسجيل العقود", i++);
            yield return P(s, "js.admin.post-employment", "/admin/post-employment", "Post-Employment Procedures", "إجراءات ما بعد التوظيف", i++);
            yield return P(s, "js.admin.employment-ratings", "/admin/employment-ratings", "Employment ratings", "تقييمات التوظيف", i++);
            yield return P(s, "js.admin.create-rate-record", "/admin/create-rate-record", "New Employer Evaluation", "اضافة تقييم دوري", i++);
            yield return P(s, "js.admin.edit-rate-record", "/admin/edit-rate-record", "Employer Evaluation", "تقييم صاحب العمل", i++);
            yield return P(s, "js.admin.project-monitoring", "/admin/project-monitoring", "Project monitoring", "مراقبة المشاريع", i++);
            yield return P(s, "js.admin.payment-dispute", "/admin/payment-dispute", "Payment dispute", "منازعات الدفع", i++);
            yield return P(s, "js.admin.reports", "/admin/reports", "Reports", "التقارير", i++);
        }

        private static IEnumerable<IdentityAccessSeedData.PageSeed> FreelancerPages()
        {
            const string s = AppPageSystemCodes.Freelancer;
            var i = 2000;
            // Culture prefix /{en|ar} is applied by FreelancerModule at runtime.
            yield return P(s, "fl.home", "/Home", "Home", "الرئيسية", i++);
            yield return P(s, "fl.home.en", "/Home/En", "Landing (EN)", "الصفحة الرئيسية (إنجليزي)", i++);
            yield return P(s, "fl.account.signin", "/Account/SignIn", "Sign in", "تسجيل الدخول", i++);
            yield return P(s, "fl.account.register-sme", "/Account/RegisterSME", "Register SME", "تسجيل منشأة", i++);
            yield return P(s, "fl.account.register-freelancer", "/Account/RegisterFreelancer", "Register freelancer", "تسجيل مستقل", i++);
            yield return P(s, "fl.account.register-operator", "/Account/RegisterOperator", "Register operator", "تسجيل مشغّل", i++);
            yield return P(s, "fl.account.otp", "/Account/OTPVerification", "OTP verification", "تحقق OTP", i++);
            yield return P(s, "fl.account.fotp", "/Account/FOTPVerification", "Freelancer OTP", "تحقق OTP للمستقل", i++);
            yield return P(s, "fl.account.ootp", "/Account/OOTPVerification", "Operator OTP", "تحقق OTP للمشغّل", i++);
            yield return P(s, "fl.account.email-verification", "/Account/EmailVerification", "Email verification", "تأكيد البريد", i++);
            yield return P(s, "fl.account.email-done", "/Account/EmailVerificationDone", "Email verified", "تم تأكيد البريد", i++);
            yield return P(s, "fl.account.email-failed", "/Account/EmailVerificationFailed", "Email verification failed", "فشل تأكيد البريد", i++);
            yield return P(s, "fl.account.email-pending", "/Account/EmailVerificationPending", "Email verification pending", "تأكيد البريد معلّق", i++);
            yield return P(s, "fl.account.tra-pending", "/Account/TRAVerificationPending", "TRA verification pending", "تحقق TRA معلّق", i++);
            yield return P(s, "fl.account.complete-profile", "/Account/CompleteProfile", "Complete profile", "إكمال الملف", i++);
            yield return P(s, "fl.account.freelancer-complete-profile", "/Account/FreelancerCompleteProfile", "Freelancer complete profile", "إكمال ملف المستقل", i++);
            yield return P(s, "fl.account.operator-complete-profile", "/Account/OperatorCompleteProfile", "Operator complete profile", "إكمال ملف المشغّل", i++);
            yield return P(s, "fl.account.profile-setting-done", "/Account/ProfileSettingDone", "Profile setting done", "اكتمل إعداد الملف", i++);
            yield return P(s, "fl.account.login-otp", "/Account/LoginOtp", "Login OTP", "OTP تسجيل الدخول", i++);

            yield return P(s, "fl.dashboard.freelancer", "/Dashboard/FreelancerDashboard", "Freelancer dashboard", "لوحة المستقل", i++);
            yield return P(s, "fl.dashboard.operator", "/Dashboard/OperatorDashboard", "Operator dashboard", "لوحة المشغّل", i++);
            yield return P(s, "fl.dashboard.admin", "/Dashboard/AdminDashboard", "Admin dashboard", "لوحة الإدارة", i++);
            yield return P(s, "fl.home.chats", "/Home/Chats", "Messages", "الرسائل", i++);
            yield return P(s, "fl.support.quick", "/Support/QuickSupport", "Support", "الدعم", i++);
            yield return P(s, "fl.support.live-chat", "/Support/LiveChat", "Live chat", "محادثة مباشرة", i++);
            yield return P(s, "fl.support.blogs", "/Support/CreateBlogs", "Create blogs", "إنشاء مقالات", i++);
            yield return P(s, "fl.support.blog-view", "/Support/BlogView", "Blog view", "عرض المقال", i++);
            yield return P(s, "fl.support.author-view", "/Support/AuthorView", "Author view", "عرض الكاتب", i++);

            yield return P(s, "fl.projects.find", "/Projects/FindProject", "Find project", "البحث عن مشروع", i++);
            yield return P(s, "fl.projects.favorites", "/Projects/FavoriteProjects", "Favorite projects", "المشاريع المفضلة", i++);
            yield return P(s, "fl.projects.apply", "/Projects/Apply", "Apply", "تقديم", i++);
            yield return P(s, "fl.projects.application-done", "/Projects/ApplicationDone", "Application done", "تم التقديم", i++);
            yield return P(s, "fl.projects.biddings", "/Projects/BiddingsList", "Applied projects / Placed bids", "المشاريع المقدّم عليها", i++);
            yield return P(s, "fl.projects.applications", "/Projects/ApplicationsList", "Proposal history", "سجل العروض", i++);
            yield return P(s, "fl.projects.bid-details", "/Projects/BidDetails", "Bid details", "تفاصيل العرض", i++);
            yield return P(s, "fl.account.user-profile", "/Account/UserProfile", "User profile", "ملف المستخدم", i++);
            yield return P(s, "fl.account.user-profile-view", "/Account/UserProfileView", "Profile details", "تفاصيل الملف", i++);
            yield return P(s, "fl.account.portfolio", "/Account/Portfolio", "Portfolio", "المعروض", i++);
            yield return P(s, "fl.account.freelancer-portfolio", "/Account/FreelancerPortfolio", "Freelancer portfolio", "معروض المستقل", i++);
            yield return P(s, "fl.account.skills", "/Account/FreelancerSkillsDetails", "Skills", "المهارات", i++);
            yield return P(s, "fl.account.education", "/Account/FreelancerEducationDetails", "Education", "التعليم", i++);
            yield return P(s, "fl.account.certification", "/Account/FreelancerCertificationDetails", "Certifications", "الشهادات", i++);
            yield return P(s, "fl.account.professional", "/Account/FreelancerProfessionalDetails", "Professional details", "التفاصيل المهنية", i++);
            yield return P(s, "fl.contract.freelancer", "/Contract/FreelancerContracts", "Contracts", "العقود", i++);
            yield return P(s, "fl.contract.details", "/Contract/ContractDetails", "Contract details", "تفاصيل العقد", i++);
            yield return P(s, "fl.contract.certificate", "/Contract/Certificate", "Certificate", "الشهادة", i++);
            yield return P(s, "fl.payment.freelancer", "/Payment/FPayments", "Payment & Milestone", "الدفع والمعالم", i++);
            yield return P(s, "fl.payment.f-disputes", "/Payment/FDisputes", "Disputes", "النزاععات", i++);
            yield return P(s, "fl.payment.f-milestones", "/Payment/FProjectMilestones", "Project milestones", "معالم المشروع", i++);
            yield return P(s, "fl.payment.f-milestone-details", "/Payment/FMilestoneDetails", "Milestone details", "تفاصيل المعلم", i++);
            yield return P(s, "fl.payment.f-dispute-details", "/Payment/FDisputeDetails", "Dispute details", "تفاصيل النزاع", i++);

            yield return P(s, "fl.projects.list", "/Projects/ProjectsList", "Projects list", "قائمة المشاريع", i++);
            yield return P(s, "fl.projects.post", "/Projects/PostProjectMo", "Post project", "نشر مشروع", i++);
            yield return P(s, "fl.projects.post-legacy", "/Projects/PostProject", "Post project (legacy)", "نشر مشروع", i++);
            yield return P(s, "fl.projects.applicants", "/Projects/ProjectApplicants", "Project applicants", "المتقدمون", i++);
            yield return P(s, "fl.projects.application-details", "/Projects/ApplicationDetails", "Application details", "تفاصيل الطلب", i++);
            yield return P(s, "fl.projects.overview", "/Projects/OverviewProject", "Project overview", "نظرة عامة على المشروع", i++);
            yield return P(s, "fl.projects.completed", "/Projects/ViewCompletedProject", "Completed project", "مشروع مكتمل", i++);
            yield return P(s, "fl.account.operator-profile", "/Account/OperatorProfile", "Operator profile", "ملف المشغّل", i++);
            yield return P(s, "fl.account.operator-portfolio", "/Account/OperatorPortfolio", "Operator portfolio", "معروض المشغّل", i++);
            yield return P(s, "fl.contract.contracts", "/Contract/Contracts", "Contracts", "العقود", i++);
            yield return P(s, "fl.contract.create", "/Contract/CreateContract", "Create contract", "إنشاء عقد", i++);
            yield return P(s, "fl.contract.op-details", "/Contract/OpContractDetails", "Contract details", "تفاصيل العقد", i++);
            yield return P(s, "fl.contract.review-rating", "/Contract/ReviewRating", "Review & rating", "المراجعة والتقييم", i++);
            yield return P(s, "fl.payment.op", "/Payment/OpPayments", "Payment & Milestone", "الدفع والمعالم", i++);
            yield return P(s, "fl.payment.disputes", "/Payment/Disputes", "Disputes", "النزاعات", i++);
            yield return P(s, "fl.payment.dispute-details", "/Payment/DisputeDetails", "Dispute details", "تفاصيل النزاع", i++);
            yield return P(s, "fl.payment.milestones", "/Payment/ProjectMilestones", "Project milestones", "معالم المشروع", i++);
            yield return P(s, "fl.payment.milestone-details", "/Payment/MilestoneDetails", "Milestone details", "تفاصيل المعلم", i++);
            yield return P(s, "fl.payment.pgcallback", "/Payment/pgcallback", "Payment gateway callback", "استدعاء بوابة الدفع", i++);

            yield return P(s, "fl.admin.dashboard", "/Admin/Dashboard", "Dashboard", "لوحة التحكم", i++);
            yield return P(s, "fl.admin.tra-pending", "/Admin/TraPendingVerificationList", "TRA verification", "تحقق TRA", i++);
            yield return P(s, "fl.admin.profile-list", "/Admin/ProfileList", "Profile list", "قائمة الملفات", i++);
            yield return P(s, "fl.admin.verify-operator", "/Admin/VerifyOperator", "Verify operator", "تحقق المشغّل", i++);
            yield return P(s, "fl.admin.user-management", "/Admin/UserManagement", "User management", "إدارة المستخدمين", i++);
            yield return P(s, "fl.admin.user-management-view", "/Admin/UserManagementView", "User management view", "عرض إدارة المستخدمين", i++);
            yield return P(s, "fl.admin.project-monitoring", "/Admin/ProjectMonitoring", "Project monitoring", "مراقبة المشاريع", i++);
            yield return P(s, "fl.admin.project-monitoring-view", "/Admin/ProjectMonitoringView", "Project monitoring view", "عرض مراقبة المشاريع", i++);
            yield return P(s, "fl.admin.payment-dispute", "/Admin/PaymentDispute", "Payment dispute", "منازعات الدفع", i++);
            yield return P(s, "fl.admin.notification-log", "/Admin/NotificationLog", "Notification log", "سجل الإشعارات", i++);
            yield return P(s, "fl.admin.messages", "/Admin/Messages", "Messages", "الرسائل", i++);
            yield return P(s, "fl.admin.project-report", "/Admin/ProjectReport", "Project report", "تقرير المشاريع", i++);
            yield return P(s, "fl.admin.reports", "/Admin/ReportsAnalytics", "Reports", "التقارير", i++);
            yield return P(s, "fl.admin.settings", "/Admin/Settings", "Settings", "الإعدادات", i++);
            yield return P(s, "fl.admin.logout", "/Admin/Logout", "Logout", "تسجيل الخروج", i++);
        }

        private static IEnumerable<IdentityAccessSeedData.PageSeed> BiddingWebPages()
        {
            const string s = AppPageSystemCodes.BiddingWeb;
            var i = 3000;
            yield return P(s, "nw.home", "/Home", "Home", "الرئيسية", i++);
            yield return P(s, "nw.home.about", "/Home/About", "About", "حول", i++);
            yield return P(s, "nw.home.services", "/Home/Services", "Services", "الخدمات", i++);
            yield return P(s, "nw.home.terms", "/Home/Terms", "Terms", "الشروط", i++);
            yield return P(s, "nw.home.policy", "/Home/Policy", "Policy", "السياسة", i++);
            yield return P(s, "nw.account.login", "/Account/Login", "Login", "تسجيل الدخول", i++);
            yield return P(s, "nw.account.launch", "/Account/Launch", "Launch", "بدء", i++);
            yield return P(s, "nw.account.tenders-first", "/Account/TendersFirstPage", "Tenders first page", "صفحة المناقصات", i++);
            yield return P(s, "nw.account.registration", "/Account/Registration", "Registration", "التسجيل", i++);
            yield return P(s, "nw.account.cho-registration", "/Account/CHORegistration", "CHO registration", "تسجيل CHO", i++);
            yield return P(s, "nw.account.forgot-password", "/Account/ForgotPassword", "Forgot password", "نسيت كلمة المرور", i++);
            yield return P(s, "nw.account.reset-password", "/Account/ResetPassword", "Reset password", "إعادة تعيين كلمة المرور", i++);
            yield return P(s, "nw.account.verification", "/Account/AccountVerification", "Account verification", "تحقق الحساب", i++);
            yield return P(s, "nw.account.email-verification", "/Account/EmailVerification", "Email verification", "تأكيد البريد", i++);
            yield return P(s, "nw.account.access-denied", "/Account/AccessDenied", "Access denied", "الوصول مرفوض", i++);
            yield return P(s, "nw.account.logout", "/Account/Logout", "Logout", "تسجيل الخروج", i++);

            yield return P(s, "nw.common.dashboard", "/Common/Dashboard", "Dashboard", "لوحة التحكم", i++);
            yield return P(s, "nw.common.profile", "/Common/Profile", "Profile", "الملف", i++);
            yield return P(s, "nw.common.cho-profile", "/Common/CHOProfile", "CHO profile", "ملف CHO", i++);
            yield return P(s, "nw.common.notifications", "/Common/Notifications", "Notifications", "الإشعارات", i++);
            yield return P(s, "nw.common.chat", "/Common/Chat", "Chat", "المحادثة", i++);
            yield return P(s, "nw.common.all-requests", "/Common/AllRequests", "All requests", "كل الطلبات", i++);
            yield return P(s, "nw.common.active-requests", "/Common/ActiveRequests", "Active requests", "الطلبات النشطة", i++);
            yield return P(s, "nw.common.pending-requests", "/Common/PendingRequests", "Pending requests", "الطلبات المعلقة", i++);
            yield return P(s, "nw.common.awarded-requests", "/Common/AwardedRequests", "Awarded requests", "الطلبات الممنوحة", i++);
            yield return P(s, "nw.common.expired-requests", "/Common/ExpiredRequests", "Expired requests", "الطلبات المنتهية", i++);
            yield return P(s, "nw.common.evaluating-requests", "/Common/EvaluatingRequests", "Evaluating requests", "طلبات التقييم", i++);
            yield return P(s, "nw.common.my-draft", "/Common/MyDraft", "My draft", "مسوداتي", i++);
            yield return P(s, "nw.common.proposals", "/Common/Proposals", "My proposals", "عروضي", i++);
            yield return P(s, "nw.common.request-detail", "/Common/RequestDetail", "Request detail", "تفاصيل الطلب", i++);
            yield return P(s, "nw.common.bid-detail", "/Common/BidDetail", "Proposal detail", "تفاصيل العرض", i++);
            yield return P(s, "nw.common.bidding", "/Common/Bidding", "Bidding", "التقديم", i++);
            yield return P(s, "nw.common.projects", "/Common/Projects", "All projects", "كل المشاريع", i++);
            yield return P(s, "nw.common.pending-projects", "/Common/PendingProjects", "Pending projects", "مشاريع معلقة", i++);
            yield return P(s, "nw.common.initiated-projects", "/Common/InitiatedProjects", "Initiated projects", "مشاريع بدأَت", i++);
            yield return P(s, "nw.common.closed-projects", "/Common/ClosedProjects", "Closed projects", "مشاريع مغلقة", i++);
            yield return P(s, "nw.common.accounts", "/Common/Accounts", "Accounts", "الحسابات", i++);
            yield return P(s, "nw.common.general-report", "/Common/GeneralReport", "General report", "تقرير عام", i++);
            yield return P(s, "nw.common.trainings", "/Common/Trainings", "Attended training", "التدريب المحضور", i++);
            yield return P(s, "nw.common.proposal-clarification", "/Common/ProposalClarification", "Proposal clarification", "توضيح العرض", i++);

            yield return P(s, "nw.user.post-requirement", "/User/PostRequirement", "Post requirement", "نشر متطلب", i++);
            yield return P(s, "nw.user.pending-requests", "/User/PendingRequests", "Pending requests", "طلبات معلقة", i++);
            yield return P(s, "nw.user.awarded-requests", "/User/AwardedRequests", "Awarded requests", "طلبات ممنوحة", i++);
            yield return P(s, "nw.user.awarding", "/User/Awarding", "Awarding", "الترسية", i++);
            yield return P(s, "nw.user.system-award", "/User/SystemAward", "System award", "ترسية النظام", i++);
            yield return P(s, "nw.user.awarding-detail", "/User/AwardingDetail", "Awarding detail", "تفاصيل الترسية", i++);
            yield return P(s, "nw.user.nda", "/User/NDA", "NDA", "اتفاقية السرية", i++);

            yield return P(s, "nw.vendor.rfqs", "/Vendor/RFQS", "All RFQ", "كل طلبات العروض", i++);
            yield return P(s, "nw.vendor.quoted-rfqs", "/Vendor/QuoutedRFQS", "Quoted RFQ", "طلبات مقدّم عليها", i++);
            yield return P(s, "nw.vendor.pending-rfqs", "/Vendor/PendingRFQS", "Pending RFQ", "طلبات معلقة", i++);
            yield return P(s, "nw.vendor.saved-rfqs", "/Vendor/SavedRFQS", "Saved RFQ", "طلبات محفوظة", i++);
            yield return P(s, "nw.vendor.expired-rfqs", "/Vendor/ExpiredRFQS", "Expired RFQ", "طلبات منتهية", i++);
            yield return P(s, "nw.vendor.post-bid", "/Vendor/PostBid", "Post bid", "تقديم عرض", i++);
            yield return P(s, "nw.vendor.nda", "/Vendor/NDA", "NDA", "اتفاقية السرية", i++);

            yield return P(s, "nw.admin.tender-category", "/Admin/TenderCategory", "Tender categories", "فئات المناقصات", i++);
            yield return P(s, "nw.admin.questions", "/Admin/Questions", "Questions", "الأسئلة", i++);
            yield return P(s, "nw.admin.templates", "/Admin/Templates", "Templates", "القوالب", i++);
            yield return P(s, "nw.admin.notification-settings", "/Admin/NotificationSettings", "Notification settings", "إعدادات الإشعارات", i++);
            yield return P(s, "nw.admin.vendors", "/Admin/Vendors", "All vendors", "كل الموردين", i++);
            yield return P(s, "nw.admin.pending-vendors", "/Admin/PendingVendors", "Pending vendors", "موردون معلقون", i++);
            yield return P(s, "nw.admin.clients", "/Admin/Clients", "All clients", "كل العملاء", i++);
            yield return P(s, "nw.admin.companies", "/Admin/Companies", "Companies", "الشركات", i++);
            yield return P(s, "nw.admin.freelancers", "/Admin/FreeLancers", "Freelancers", "المستقلون", i++);
            yield return P(s, "nw.admin.contractors", "/Admin/Contractors", "Contractors", "المقاولون", i++);
            yield return P(s, "nw.admin.roles", "/Admin/Roles", "Roles", "الأدوار", i++);
            yield return P(s, "nw.admin.training-list", "/Admin/Traininglist", "Training list", "قائمة التدريب", i++);
            yield return P(s, "nw.admin.tra-training", "/Admin/TRATraining", "TRA training", "تدريب TRA", i++);
            yield return P(s, "nw.admin.accounts-verification", "/Admin/AccountsVerification", "Verification of account", "تحقق الحسابات", i++);
            yield return P(s, "nw.admin.accounts-verification-report", "/Admin/AccountsVerificationReport", "Account verification report", "تقرير تحقق الحسابات", i++);
            yield return P(s, "nw.admin.training-report", "/Admin/TrainingReport", "Training report", "تقرير التدريب", i++);
            yield return P(s, "nw.admin.banners", "/Admin/Banners", "Banners", "اللافتات", i++);
            yield return P(s, "nw.admin.advertisement", "/Admin/Advertisement", "Advertisement", "الإعلانات", i++);
            yield return P(s, "nw.admin.add-operators", "/Admin/AddOperatorsUsers", "Add operator users", "إضافة مستخدمي المشغّل", i++);
            yield return P(s, "nw.admin.vendor-profile", "/Admin/VendorProfile", "Vendor profile", "ملف المورد", i++);

            yield return P(s, "nw.training.index", "/Training", "Training registration", "تسجيل التدريب", i++);
            yield return P(s, "nw.training.login", "/Training/TrainingLogin", "Training login", "دخول التدريب", i++);
            yield return P(s, "nw.training.tra", "/Training/TRA", "TRA training", "تدريب TRA", i++);
            yield return P(s, "nw.training.tra-registration", "/Training/TRAregistration", "TRA registration", "تسجيل TRA", i++);
            yield return P(s, "nw.training.tra-sme", "/Training/TRAregistrationSME", "SME TRA registration", "تسجيل TRA للمنشآت", i++);
            yield return P(s, "nw.training.vendors", "/Training/VendorsTraining", "Vendors training", "تدريب الموردين", i++);
            yield return P(s, "nw.training.add", "/Training/AddTraining", "Add training", "إضافة تدريب", i++);
            yield return P(s, "nw.training.job-application", "/Training/JobApplication", "Job application", "طلب وظيفة", i++);
            yield return P(s, "nw.training.verify-otp", "/Training/VerifyOTP", "Verify OTP", "تحقق OTP", i++);
        }
    }
}

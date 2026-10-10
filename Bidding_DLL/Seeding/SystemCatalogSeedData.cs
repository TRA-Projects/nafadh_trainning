using System;
using System.Collections.Generic;
using Bidding_Models.Jobseekers;

namespace Bidding_DLL.Seeding
{
    /// <summary>
    /// Canonical jobseeker system-catalog seed (system variables, config groups/values, taxonomy).
    /// Applied idempotently at startup — not sourced from in-memory mocks.
    /// </summary>
    internal static class SystemCatalogSeedData
    {
        internal sealed class SystemVariableSeed
        {
            public string VariableKey { get; set; }
            public string VariableName { get; set; }
            public string VariableNameEn { get; set; }
            public string VariableNameAr { get; set; }
            public string ValueType { get; set; }
            public string Value { get; set; }
            public string Description { get; set; }
            public string DescriptionEn { get; set; }
            public string DescriptionAr { get; set; }
        }

        internal sealed class ConfigGroupSeed
        {
            public string Code { get; set; }
            public string NameEn { get; set; }
            public string NameAr { get; set; }
            public int SortOrder { get; set; }
        }

        internal sealed class ConfigValueSeed
        {
            public string ValueKey { get; set; }
            public string Code { get; set; }
            public string LabelEn { get; set; }
            public string LabelAr { get; set; }
            public bool? SubReasonDropdown { get; set; }
            public string ParentValueKey { get; set; }
            public int SortOrder { get; set; }
        }

        internal sealed class SkillCategorySeed
        {
            public int Id { get; set; }
            public string NameEn { get; set; }
            public string NameAr { get; set; }
            public int SortOrder { get; set; }
        }

        internal sealed class SkillSeed
        {
            public int Id { get; set; }
            public int CategoryId { get; set; }
            public string NameEn { get; set; }
            public string NameAr { get; set; }
        }

        /// <summary>Seeded by (Kind, NameEn) — no fixed id, since merged-table ids are identity-assigned.</summary>
        internal sealed class BasicSkillSeed
        {
            public int CategoryId { get; set; }
            public string NameEn { get; set; }
            public string NameAr { get; set; }
        }

        /// <summary>Seeded by (Kind, NameEn) — no fixed id, since merged-table ids are identity-assigned.</summary>
        internal sealed class TechStackSeed
        {
            public string NameEn { get; set; }
            public string NameAr { get; set; }
        }

        internal static IReadOnlyList<SystemVariableSeed> SystemVariables { get; } = new List<SystemVariableSeed>
        {
            Var(
                SystemVariableCodes.AllowUserRating,
                "Allow user skill self-rating",
                "السماح للمستخدم بتقييم مهاراته",
                "String",
                "N",
                "",
                ""),
            Var(
                SystemVariableCodes.SelfRatingWeight,
                "Self-rating assessment weight",
                "وزن التقييم الذاتي",
                "Integer",
                "0",
                "",
                ""),
            Var(
                SystemVariableCodes.PeriodicRatingWeight,
                "Post-employment periodic rating weight",
                "وزن التقييم الدوري لما بعد التوظيف",
                "Integer",
                "1",
                "Weight used when rolling up employer periodic ratings into jobseeker skill summaries.",
                "الوزن المستخدم عند احتساب تقييمات صاحب العمل الدورية في ملخص مهارات الباحث."),
            Var(
                SystemVariableCodes.PeriodicEvalMinPeriodDays,
                "Periodic evaluation minimum period (days)",
                "الحد الأدنى لفترة التقييم الدوري (أيام)",
                "Integer",
                "90",
                "Minimum number of calendar days between employer periodic evaluation From and To dates.",
                "الحد الأدنى لعدد الأيام التقويمية بين تاريخي بداية ونهاية التقييم الدوري لصاحب العمل."),
            Var(
                SystemVariableCodes.PeriodicEvalAutoFillDates,
                "Auto-fill periodic evaluation dates",
                "تعبئة تواريخ التقييم الدوري تلقائياً",
                "String",
                "Y",
                "",
                ""),
            Var(
                SystemVariableCodes.PeriodicEvalAllowFutureDates,
                "Allow future dates for periodic evaluations",
                "السماح بالتواريخ المستقبلية للتقييمات الدورية",
                "String",
                "Y",
                "",
                ""),
            Var(
                SystemVariableCodes.MatchNotificationThreshold,
                "Minimum match % to notify seekers",
                "الحد الأدنى لنسبة التطابق لإشعار الباحثين",
                "Integer",
                "70",
                "Published opportunities notify seekers at or above this match score.",
                "يتم إشعار الباحثين بالفرص المنشورة عند هذا الحد أو أعلى."),
            Var(
                SystemVariableCodes.DefaultLocale,
                "Default UI locale",
                "اللغة الافتراضية للواجهة",
                "String",
                "en",
                "Fallback when user has no preference.",
                "اللغة الاحتياطية عندما لا يحدد المستخدم تفضيلاً."),
            Var(
                SystemVariableCodes.PkiEnabled,
                "PKI authentication enabled",
                "تفعيل المصادقة عبر الهوية الرقمية",
                "String",
                "N",
                "",
                ""),
            Var(
                SystemVariableCodes.LoginOtpEnabled,
                "Staff login OTP enabled",
                "تفعيل رمز التحقق لتسجيل دخول الموظفين",
                "String",
                "N",
                "When Y, /staff/login requires SMS OTP after password.",
                "عند التفعيل Y يتطلب تسجيل دخول الموظفين رمز تحقق عبر الرسائل بعد كلمة المرور."),
            Var(
                SystemVariableCodes.RegistrationSmsEnabled,
                "Registration SMS OTP enabled",
                "تفعيل رمز التحقق عبر الرسائل عند التسجيل",
                "String",
                "N",
                "",
                ""),
            Var(
                SystemVariableCodes.RegistrationEmailEnabled,
                "Registration email verification enabled",
                "تفعيل التحقق من البريد عند التسجيل",
                "String",
                "N",
                "",
                ""),
            Var(
                SystemVariableCodes.NafathRegistrationCheckEnabled,
                "Nafath registration check enabled",
                "تفعيل التحقق عبر نفاذ عند التسجيل",
                "String",
                "Y",
                "",
                ""),
            Var(
                SystemVariableCodes.NafathUseMock,
                "Nafath JobSeeker use mock",
                "استخدام بيانات تجريبية لنفاذ الباحث عن عمل",
                "String",
                "Y",
                "",
                ""),
            Var(
                SystemVariableCodes.NafathValidityCheck,
                "Nafath job-seeker validity check",
                "التحقق من صلاحية الباحث عن عمل عبر نفاذ",
                "String",
                NafathValidityCheck.DefaultCode,
                "When Nafath registration check is on: no check, expiry date only, active flag only, or both.",
                "عند تفعيل التحقق عبر نفاذ: بدون تحقق، تاريخ الانتهاء فقط، علامة النشاط فقط، أو كلاهما."),
            Var(
                SystemVariableCodes.TraLicensesUseMock,
                "TRA licenses use mock",
                "استخدام بيانات تجريبية لتراخيص هيئة تنظيم الاتصالات",
                "String",
                "Y",
                "",
                ""),
            Var(
                SystemVariableCodes.AttachmentAllowedExtensions,
                "Attachment allowed extensions",
                "امتدادات المرفقات المسموحة",
                "String",
                "pdf,doc,docx,xls,xlsx,jpg,jpeg,png",
                "Comma-separated file extensions (no dots).",
                "امتدادات الملفات مفصولة بفواصل وبدون نقاط."),
            Var(
                SystemVariableCodes.AttachmentMaxSizeMb,
                "Attachment max size (MB)",
                "الحد الأقصى لحجم المرفق (ميجابايت)",
                "Integer",
                "5",
                "Maximum size in megabytes per attachment.",
                "الحد الأقصى لحجم كل مرفق بالميجابايت."),
            Var(
                SystemVariableCodes.JobApplicationUndertakingEn,
                "Job application undertaking (English)",
                "تعهد طلب التوظيف (الإنجليزية)",
                "String",
                "I confirm that the information and documents submitted with this job application are accurate and complete. I authorize the employer and platform administrators to review them for recruitment purposes, and I understand that false or misleading information may result in the rejection or withdrawal of my application.",
                "English undertaking shown before a job seeker submits an application.",
                "نص التعهد باللغة الإنجليزية الذي يظهر قبل إرسال طلب التوظيف."),
            Var(
                SystemVariableCodes.JobApplicationUndertakingAr,
                "Job application undertaking (Arabic)",
                "تعهد طلب التوظيف (العربية)",
                "String",
                "أقر بأن المعلومات والمستندات المقدمة في طلب التوظيف هذا صحيحة وكاملة، وأوافق على تمكين جهة العمل ومسؤولي المنصة من مراجعتها لأغراض التوظيف، وأدرك أن تقديم معلومات غير صحيحة أو مضللة قد يؤدي إلى رفض طلبي أو سحبه.",
                "Arabic undertaking shown before a job seeker submits an application.",
                "نص التعهد باللغة العربية الذي يظهر قبل إرسال طلب التوظيف."),
            Var(
                SystemVariableCodes.SocialInsuranceDeductionPercent,
                "Social insurance deduction (%)",
                "نسبة استقطاع التأمينات الاجتماعية (%)",
                "Integer",
                "8",
                "Percent deducted from offer wage total for social insurance.",
                "النسبة المقتطعة من إجمالي الأجر للتأمينات الاجتماعية."),
            Var(
                SystemVariableCodes.SchedulingDayEndTime,
                "Scheduling day end time",
                "وقت نهاية يوم الجدولة",
                "Time",
                "15:00",
                "Interview and assessment slots at or after this time (HH:mm) move to the next working day (Friday and Saturday skipped).",
                "مواعيد المقابلات والتقييمات في هذا الوقت أو بعده (س:د) تنتقل إلى يوم العمل التالي (الجمعة والسبت مستثنيان)."),
            Var(
                SystemVariableCodes.EducationDegreeRanks,
                "Education degree ranks",
                "مراتب الدرجات العلمية",
                "Json",
                EducationDegreeRanks.DefaultJson,
                "Rank 1 is the highest. Labels match Nafath education descriptions.",
                "المرتبة 1 هي الأعلى. التسميات تطابق وصف التعليم من نفاذ."),
        };

        internal static IReadOnlyList<ConfigGroupSeed> ConfigGroups { get; } = new List<ConfigGroupSeed>
        {
            Group("OPP_TYPE", "Opportunity type", "نوع الفرصة", 1),
            Group("APPL_METH", "Application method", "طريقة التقديم", 2),
            Group("WORK_MODE", "Work mode", "نمط العمل", 3),
            Group("TGT_GEND", "Target gender", "الجنس المستهدف", 4),
            Group("INTV_MODE", "Interview mode", "نمط المقابلة", 5),
            Group("CRDT_STAT", "Credential status", "حالة الاعتماد", 6),
            Group("EXAM_TYPE", "Exam type", "نوع الاختبار", 7),
            Group("CRDT_PROV", "Course provider", "مزود الدورة", 8),
            Group("EDUC_LVL", "Education level", "المستوى التعليمي", 9),
            Group("QUAL_DEGR", "Qualification degree", "درجة المؤهل", 10),
            Group("TRNG_TYPE", "Training program type", "نوع برنامج التدريب", 11),
            Group("STDY_FILDS", "Field of study", "مجال الدراسة", 12),
            Group("LANG", "Language", "اللغة", 13),
            Group("TGT_APPL", "Target applicant", "المتقدم المستهدف", 14),
            Group("DOC_TYPE", "Document file type", "نوع ملف المستند", 15),
            Group("SAL_DISP", "Salary display", "عرض الراتب", 16),
            Group("CURR", "Currency", "العملة", 17),
            Group("PAY_FREQ", "Pay frequency", "تكرار الدفع", 18),
            Group("BENEF", "Benefit", "المزايا", 19),
            Group("ALLOW", "Allowance", "البدلات", 20),
            Group("EDUC_INST", "Educational institution", "المؤسسة التعليمية", 21),
            Group("NAT", "Nationality", "الجنسية", 22),
            Group("PRIM_SPEC", "Primary specification", "التخصص الرئيسي", 23),
            Group("CONT_TYPE", "Contract type", "نوع العقد", 24),
            Group("OFFR_RJCT", "Offer rejection reason", "سبب رفض العرض", 25),
            Group("RJCT_RSN", "Interview rejection reason", "سبب رفض المقابلة", 26),
            Group("RJCT_SUB", "Interview rejection sub-reason", "سبب فرعي لرفض المقابلة", 27),
            Group("NSHW_RSN", "Interview no-show reason", "سبب عدم الحضور للمقابلة", 28),
            Group("RSCH_RSN", "Interview reschedule reason", "سبب إعادة جدولة المقابلة", 29),
            Group("WRK_SCOPE", "Opportunity field / Work scope", "مجال الفرصة / نطاقات العمل", 30),
            Group("JOB_CAT", "Job category (legacy)", "فئة الوظيفة (قديم)", 31),
            Group("OFFR_ENTY", "Offer entity", "جهة العرض", 32),
            Group("AOFF_RJCT", "Assessment offer rejection reason", "سبب رفض عرض التقييم", 33),
            Group("AOFF_SUB", "Assessment offer rejection sub-reason", "سبب فرعي لرفض عرض التقييم", 34),
            Group("WD_RSN", "Withdrawal reason category", "فئة سبب الانسحاب", 35),
            Group("BEHV", "Behavior", "السلوك", 36),
            Group("ATA_RJCT", "Ask-to-apply rejection reason", "سبب رفض طلب التقديم", 37)
        };

        internal static IReadOnlyDictionary<string, IReadOnlyList<ConfigValueSeed>> ConfigValuesByCode { get; } =
            new Dictionary<string, IReadOnlyList<ConfigValueSeed>>
            {
                ["OPP_TYPE"] = Rows(
                    V("full-time", "FLT", "Full-Time", "دوام كامل", 1),
                    V("part-time", "PRT", "Part-Time", "دوام جزئي", 2),
                    V("contract", "CON", "Contract", "عقد", 3),
                    V("internship", "INT", "Internship", "تدريب عملي", 4),
                    V("training", "TRN", "Training", "تدريب", 5)),
                ["APPL_METH"] = Rows(
                    V("tra-nomination", "TRA", "TRA Nomination", "ترشيح الجهة", 1),
                    V("employer-search", "EMP", "Employer Search", "بحث صاحب العمل", 2),
                    V("candidate-apply", "CND", "Candidate Apply", "تقديم المرشح", 3),
                    V("all", "ALL", "All", "الكل", 4)),
                ["WORK_MODE"] = Rows(
                    V("onsite", "ONS", "On-site", "حضوري", 1),
                    V("remote", "RMT", "Remote", "عن بعد", 2),
                    V("hybrid", "HYB", "Hybrid", "هجين", 3)),
                ["TGT_GEND"] = Rows(
                    V("male", "MAL", "Male", "ذكر", 1),
                    V("female", "FEM", "Female", "أنثى", 2),
                    V("both", "BTH", "Both", "كلاهما", 3)),
                ["INTV_MODE"] = Rows(
                    V("in-person", "INP", "In-Person", "حضوري", 1),
                    V("online", "ONL", "Online", "عن بعد", 2),
                    V("phone", "PHN", "Phone", "هاتف", 3),
                    V("to-be-scheduled", "TBS", "To Be Scheduled", "يحدد لاحقاً", 4)),
                ["CRDT_STAT"] = Rows(
                    V("Draft", "DRF", "Draft", "مسودة", 1),
                    V("PendingScheduling", "PND", "Pending scheduling", "بانتظار الجدولة", 2),
                    V("Active", "ACT", "Active", "نشط", 3),
                    V("Completed", "CMP", "Completed", "مكتمل", 4)),
                ["EXAM_TYPE"] = Rows(
                    V("Theory", "THY", "Theory", "نظري", 1),
                    V("Practical", "PRC", "Practical", "عملي", 2)),
                ["CRDT_PROV"] = Rows(
                    V("nafadh-academy", "NAF", "NAFADH Academy", "أكاديمية نافذ", 1)),
                ["EDUC_LVL"] = Rows(
                    V("high-school", "HSC", "High School", "الثانوية العامة", 1),
                    V("diploma", "DIP", "Diploma", "دبلوم", 2),
                    V("higher-diploma", "HDIP", "Higher Diploma", "الدبلوم العالي", 3),
                    V("bachelor", "BCH", "Bachelor's", "بكالوريوس", 4),
                    V("master", "MST", "Master's", "ماجستير", 5),
                    V("phd", "PHD", "PhD", "دكتوراه", 6)),
                ["QUAL_DEGR"] = Rows(
                    V("high-school", "HSC", "High School", "الثانوية العامة", 1),
                    V("diploma", "DIP", "Diploma", "دبلوم", 2),
                    V("higher-diploma", "HDIP", "Higher Diploma", "الدبلوم العالي", 3),
                    V("certificate", "CRT", "Certificate", "شهادة مهنية", 4),
                    V("bachelor", "BCH", "Bachelor's degree", "بكالوريوس", 5),
                    V("master", "MST", "Master's degree", "ماجستير", 6),
                    V("phd", "PHD", "Doctorate (PhD)", "دكتوراه", 7)),
                ["TRNG_TYPE"] = Rows(
                    V("ojt", "OJT", "On-the-job training", "التدريب على رأس العمل", 1),
                    V("in-person-program", "IPP", "In-person training program", "برنامج تدريبي حضوري", 2),
                    V("remote-program", "RMP", "Remote training program", "برنامج تدريبي عن بعد", 3)),
                ["STDY_FILDS"] = Rows(
                    V("computer-science", "CSC", "Computer Science", "علوم الحاسب", 1),
                    V("information-technology", "ITC", "Information Technology", "تقنية المعلومات", 2),
                    V("engineering", "ENG", "Engineering", "الهندسة", 3),
                    V("business", "BSN", "Business", "الأعمال", 4),
                    V("design", "DSG", "Design", "التصميم", 5)),
                ["LANG"] = Rows(
                    V("arabic", "ARA", "Arabic", "العربية", 1),
                    V("english", "ENG", "English", "الإنجليزية", 2),
                    V("hindi", "HIN", "Hindi", "الهندية", 3),
                    V("urdu", "URD", "Urdu", "الأردية", 4)),
                ["TGT_APPL"] = Rows(
                    V("all", "ALL", "All", "الكل", 1),
                    V("job-seeker", "JSK", "Job Seeker", "باحث عن عمل", 2),
                    V("fresh-graduate", "FGR", "Recent Graduate", "حديثي التخرج", 3)),
                ["DOC_TYPE"] = Rows(
                    V("any", "ANY", "Any format", "أي صيغة", 1),
                    V("pdf", "PDF", "PDF", "PDF", 2),
                    V("excel", "XLS", "Excel", "إكسل", 3),
                    V("images", "IMG", "Images", "صور", 4)),
                ["SAL_DISP"] = Rows(
                    V("visible", "VIS", "Displayed to the jobseeker", "يُعرض للباحث عن عمل", 1),
                    V("hidden", "HID", "Not displayed to the jobseeker", "لا يُعرض للباحث عن عمل", 2),
                    V("negotiable", "NEG", "Negotiable", "قابل للتفاوض", 3)),
                ["CURR"] = Rows(
                    V("OMR", "OMR", "Omani Rial", "ريال عماني", 1),
                    V("USD", "USD", "US Dollar", "دولار أمريكي", 2),
                    V("EUR", "EUR", "Euro", "يورو", 3),
                    V("SAR", "SAR", "Saudi Riyal", "ريال سعودي", 4)),
                ["PAY_FREQ"] = Rows(
                    V("monthly", "MTH", "Monthly", "شهري", 1),
                    V("hourly", "HRL", "Hourly", "بالساعة", 2),
                    V("project-based", "PRJ", "Project-Based", "حسب المشروع", 3),
                    V("annual", "ANN", "Annual", "سنوي", 4),
                    V("PF-MONTHLY", "PFM", "Monthly", "شهري", 5),
                    V("PF-BIWEEKLY", "PFB", "Bi-weekly", "كل أسبوعين", 6),
                    V("PF-WEEKLY", "PFW", "Weekly", "أسبوعي", 7)),
                ["BENEF"] = Rows(
                    V("health-insurance", "HLT", "Health Insurance", "تأمين صحي", 1),
                    V("remote-allowance", "RMA", "Remote Allowance", "بدل عمل عن بعد", 2),
                    V("transportation", "TRP", "Transportation", "مواصلات", 3),
                    V("training-budget", "TRB", "Training Budget", "ميزانية تدريب", 4)),
                ["ALLOW"] = Rows(
                    V("basic-salary", "BASIC", "Basic salary", "الراتب الأساسي", 1),
                    V("housing", "HSG", "Housing allowance", "بدل سكن", 2),
                    V("transportation", "TRP", "Transportation allowance", "بدل مواصلات", 3),
                    V("food", "FOOD", "Food allowance", "بدل طعام", 4),
                    V("phone", "PHN", "Phone allowance", "بدل هاتف", 5),
                    V("other", "OTH", "Other allowance", "بدل أخرى", 6)),
                ["EDUC_INST"] = Rows(
                    V("squ", "SQU", "Sultan Qaboos University", "جامعة السلطان قابوس", 1),
                    V("ut", "UT", "University of Technology and Applied Sciences", "جامعة التقنية والعلوم التطبيقية", 2),
                    V("muscat-college", "MC", "Muscat College", "كلية مسقط", 3)),
                ["NAT"] = Rows(
                    V("OM", "OM", "Oman", "عُمان", 1),
                    V("AE", "AE", "United Arab Emirates", "الإمارات العربية المتحدة", 2)),
                ["PRIM_SPEC"] = Rows(
                    V("IT", "IT", "Information Technology", "تقنية المعلومات", 1),
                    V("HL", "HL", "Healthcare", "الرعاية الصحية", 2),
                    V("ENG", "ENG", "Engineering", "الهندسة", 3)),
                ["CONT_TYPE"] = Rows(
                    V("CT-PERM", "PRM", "Permanent", "دائم", 1),
                    V("CT-FIXED", "FXD", "Fixed-term", "محدد المدة", 2),
                    V("CT-INTERN", "INT", "Internship", "تدريب عملي", 3),
                    V("CT-TRAIN", "TRN", "Training", "تدريب", 4)),
                ["OFFR_RJCT"] = Rows(
                    V("RR-SALARY", "SAL", "Total wage / compensation", "الأجر الإجمالي أو التعويض", 1),
                    V("RR-LOCATION", "LOC", "Work location", "موقع العمل", 2),
                    V("RR-HOURS", "HRS", "Working hours", "ساعات العمل", 3),
                    V("RR-TYPE", "TYP", "Opportunity type", "نوع الفرصة", 4),
                    V("RR-TIMING", "TIM", "Timing / start availability", "توقيت أو موعد البدء", 5),
                    V("RR-OTHER-OPP", "OOP", "Accepted another opportunity", "تم قبول فرصة أخرى", 6),
                    V("RR-PERSONAL", "PER", "Personal reasons", "أسباب شخصية", 7),
                    V("RR-OTHER", "OTH", "Other", "أخرى", 8)),
                ["OFFR_ENTY"] = Rows(
                    V("company-a", "CPA", "Company A", "الشركة أ", 1),
                    V("company-b", "CPB", "Company B", "الشركة ب", 2),
                    V("gov-entity", "GOV", "Government entity", "جهة حكومية", 3)),
                ["AOFF_RJCT"] = new List<ConfigValueSeed>
                {
                    V("salary", "SAL", "Salary / compensation", "الراتب / التعويض", 1),
                    V("location", "LOC", "Work location", "موقع العمل", 2),
                    VSub("role-mismatch", "ROL", "Role mismatch", "عدم ملاءمة الدور", 3, true),
                    V("other", "OTH", "Other", "أخرى", 4)
                },
                ["AOFF_SUB"] = new List<ConfigValueSeed>
                {
                    VParent("role-seniority", "RSN", "Seniority level", "مستوى الأقدمية", 1, "role-mismatch"),
                    VParent("role-scope", "RSP", "Scope of work", "نطاق العمل", 2, "role-mismatch"),
                    VParent("role-title", "RTL", "Job title", "المسمى الوظيفي", 3, "role-mismatch")
                },
                ["WD_RSN"] = Rows(
                    V("PersonalReasons", "PER", "Personal reasons", "أسباب شخصية", 1),
                    V("BetterOpportunity", "BOP", "Better opportunity", "فرصة أفضل", 2),
                    V("Relocation", "REL", "Relocation", "انتقال", 3),
                    V("HealthReasons", "HLT", "Health reasons", "أسباب صحية", 4),
                    V("Other", "OTH", "Other", "أخرى", 5)),
                ["ATA_RJCT"] = Rows(
                    V("ATA-INTEREST", "INT", "Not interested", "غير مهتم", 1),
                    V("ATA-LOC", "LOC", "Work location", "موقع العمل", 2),
                    V("ATA-TIME", "TIM", "Timing / start availability", "توقيت أو موعد البدء", 3),
                    V("ATA-OTHER-OPP", "OOP", "Accepted another opportunity", "تم قبول فرصة أخرى", 4),
                    V("ATA-PERSONAL", "PER", "Personal reasons", "أسباب شخصية", 5),
                    V("ATA-OTHER", "OTH", "Other", "أخرى", 6)),
                ["BEHV"] = Rows(
                    V("angry", "ANG", "Angry", "غاضب", 1),
                    V("careless", "CRL", "Careless", "مهمل", 2),
                    V("distracted", "DST", "Distracted", "مشتت", 3),
                    V("rude", "RUD", "Rude", "فظ", 4),
                    V("late", "LAT", "Late", "متأخر", 5),
                    V("unprepared", "UNP", "Unprepared", "غير مستعد", 6),
                    V("cooperative", "COP", "Cooperative", "متعاون", 7),
                    V("attentive", "ATT", "Attentive", "منتبه", 8)),
                ["RJCT_RSN"] = new List<ConfigValueSeed>
                {
                    V("insufficient-experience", "IEX", "Insufficient experience", "خبرة غير كافية", 1),
                    VSub("missing-certificate", "MCE", "Missing certificate", "شهادة مفقودة", 2, true),
                    V("failed-assessment", "FAS", "Failed assessment", "فشل في التقييم", 3),
                    V("cultural-fit", "CFI", "Cultural fit concerns", "مخاوف ملاءمة ثقافية", 4)
                },
                ["RJCT_SUB"] = new List<ConfigValueSeed>
                {
                    VParent("cert-pmp", "PMP", "PMP", "PMP", 1, "missing-certificate"),
                    VParent("cert-aws", "AWS", "AWS Certified Solutions Architect", "AWS Certified Solutions Architect", 2, "missing-certificate"),
                    VParent("cert-cissp", "CIS", "CISSP", "CISSP", 3, "missing-certificate"),
                    VParent("cert-ielts", "IEL", "IELTS", "IELTS", 4, "missing-certificate")
                },
                ["NSHW_RSN"] = Rows(
                    V("personal-emergency", "PEM", "Personal emergency", "ظرف شخصي طارئ", 1),
                    V("transport-delay", "TRD", "Transport / traffic delay", "تأخير مواصلات أو ازدحام", 2),
                    V("forgot-appointment", "FGT", "Forgot appointment", "نسيان الموعد", 3),
                    V("illness", "ILL", "Illness", "مرض", 4),
                    V("other", "OTH", "Other", "أخرى", 5)),
                ["RSCH_RSN"] = Rows(
                    V("scheduling-conflict", "SCF", "Scheduling conflict", "تعارض في المواعيد", 1),
                    V("illness", "ILL", "Illness", "مرض", 2),
                    V("travel", "TRV", "Travel", "سفر", 3),
                    V("personal-reasons", "PER", "Personal reasons", "أسباب شخصية", 4),
                    V("other", "OTH", "Other", "أخرى", 5)),
                ["WRK_SCOPE"] = Rows(
                    V("information-technology", "1", "Information Technology", "تقنية المعلومات", 1),
                    V("engineering", "2", "Engineering", "الهندسة", 2),
                    V("healthcare", "3", "Healthcare", "الرعاية الصحية", 3),
                    V("business-administration", "4", "Business & Administration", "الأعمال والإدارة", 4),
                    V("education-training", "5", "Education & Training", "التعليم والتدريب", 5),
                    V("finance-accounting", "6", "Finance & Accounting", "المالية والمحاسبة", 6),
                    V("design-creative", "7", "Design & Creative", "التصميم والإبداع", 7),
                    V("operations-logistics", "8", "Operations & Logistics", "العمليات واللوجستيات", 8) ),
                    
                ["JOB_CAT"] = Rows(
                    V("JOB_CAT_IT",    "JIT", "IT & Software",          "تقنية المعلومات والبرمجيات",  1),
                    V("JOB_CAT_DESIGN","JDS", "Design & Creative",       "التصميم والإبداع",            2),
                    V("JOB_CAT_BIZ",   "JBZ", "Business",               "الأعمال",                     3),
                    V("JOB_CAT_ENG",   "JEN", "Engineering",            "الهندسة",                     4),
                    V("JOB_CAT_DATA",  "JDA", "Analytics & Data",       "التحليل والبيانات",           5),
                    V("JOB_CAT_SWDEV", "JSD", "Software Development",   "تطوير البرمجيات",             6),
                    V("JOB_CAT_SEC",   "JSC", "Cybersecurity",          "الأمن السيبراني",             7),
                    V("JOB_CAT_CLOUD", "JCL", "Cloud & Infrastructure", "السحابة والبنية التحتية",     8),
                    V("JOB_CAT_NET",   "JNT", "Networking & Telecom",   "الشبكات والاتصالات",          9))
            };

        internal static IReadOnlyList<SkillCategorySeed> SkillCategories { get; } = new List<SkillCategorySeed>
        {
            new SkillCategorySeed { Id = 1, NameEn = "Software", NameAr = "برمجيات", SortOrder = 1 },
            new SkillCategorySeed { Id = 2, NameEn = "Data & AI", NameAr = "البيانات والذكاء الاصطناعي", SortOrder = 2 }
        };

        internal static IReadOnlyList<SkillSeed> Skills { get; } = new List<SkillSeed>
        {
            Skill(101, 1, "TypeScript", "تايب سكريبت"),
            Skill(102, 1, "Angular", "أنجولار"),
            Skill(103, 1, "API Design", "تصميم واجهات برمجية"),
            Skill(104, 1, "System Design", "تصميم الأنظمة"),
            Skill(105, 1, "Flutter", "فلاتر"),
            Skill(106, 1, "UX Research", "بحث تجربة المستخدم"),
            Skill(107, 1, "Communication", "التواصل"),
            Skill(201, 2, "Python", "بايثون")
        };

        internal static IReadOnlyList<BasicSkillSeed> BasicSkills { get; } = new List<BasicSkillSeed>
        {
            Basic(1, "Communication", "التواصل"),
            Basic(1, "Teamwork", "العمل الجماعي"),
            Basic(2, "Problem Solving", "حل المشكلات")
        };

        internal static IReadOnlyList<TechStackSeed> TechStacks { get; } = new List<TechStackSeed>
        {
            Tech("Node.js", "نود جي إس"),
            Tech("PostgreSQL", "بوستجرس"),
            Tech("Backend", "الواجهة الخلفية"),
            Tech("Data", "البيانات"),
            Tech("Frontend", "الواجهة الأمامية"),
            Tech("Mobile", "الجوال")
        };

        private static SystemVariableSeed Var(
            string key,
            string nameEn,
            string nameAr,
            string type,
            string value,
            string descriptionEn,
            string descriptionAr) =>
            new SystemVariableSeed
            {
                VariableKey = key,
                VariableName = nameEn,
                VariableNameEn = nameEn,
                VariableNameAr = nameAr,
                ValueType = type,
                Value = value,
                Description = descriptionEn,
                DescriptionEn = descriptionEn,
                DescriptionAr = descriptionAr,
            };

        private static ConfigGroupSeed Group(string code, string nameEn, string nameAr, int sortOrder) =>
            new ConfigGroupSeed { Code = code, NameEn = nameEn, NameAr = nameAr, SortOrder = sortOrder };

        private static ConfigValueSeed V(string valueKey, string code, string en, string ar, int sortOrder) =>
            new ConfigValueSeed { ValueKey = valueKey, Code = code, LabelEn = en, LabelAr = ar, SortOrder = sortOrder };

        private static ConfigValueSeed VSub(string valueKey, string code, string en, string ar, int sortOrder, bool subReasonDropdown) =>
            new ConfigValueSeed { ValueKey = valueKey, Code = code, LabelEn = en, LabelAr = ar, SortOrder = sortOrder, SubReasonDropdown = subReasonDropdown };

        private static ConfigValueSeed VParent(string valueKey, string code, string en, string ar, int sortOrder, string parentValueKey) =>
            new ConfigValueSeed { ValueKey = valueKey, Code = code, LabelEn = en, LabelAr = ar, SortOrder = sortOrder, ParentValueKey = parentValueKey };

        private static IReadOnlyList<ConfigValueSeed> Rows(params ConfigValueSeed[] rows) => rows;

        private static SkillSeed Skill(int id, int categoryId, string en, string ar) =>
            new SkillSeed { Id = id, CategoryId = categoryId, NameEn = en, NameAr = ar };

        private static BasicSkillSeed Basic(int categoryId, string en, string ar) =>
            new BasicSkillSeed { CategoryId = categoryId, NameEn = en, NameAr = ar };

        private static TechStackSeed Tech(string en, string ar) =>
            new TechStackSeed { NameEn = en, NameAr = ar };
    }
}

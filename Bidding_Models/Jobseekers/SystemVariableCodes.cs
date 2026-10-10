namespace Bidding_Models.Jobseekers
{
    /// <summary>Stable keys for <see cref="DatabaseModel.SystemCatalog.SystemVariable.VariableKey"/> lookups.</summary>
    public static class SystemVariableCodes
    {
        public const string AllowUserRating = "ALLOW_USER_RATING";
        public const string SelfRatingWeight = "SELF_RATING_WEIGHT";
        /// <summary>Weight applied to post-employment employer periodic ratings in skill rollup.</summary>
        public const string PeriodicRatingWeight = "PERIODIC_RATING_WEIGHT";
        /// <summary>Minimum inclusive calendar-day span between periodic evaluation From/To dates.</summary>
        public const string PeriodicEvalMinPeriodDays = "PERIODIC_EVAL_MIN_PERIOD_DAYS";
        /// <summary>Y/N — when Y, new periodic evaluations auto-fill From/To from contract start or prior period.</summary>
        public const string PeriodicEvalAutoFillDates = "PERIODIC_EVAL_AUTO_FILL_DATES";
        /// <summary>Y/N — when Y, From/To may be after today; when N, future dates are rejected.</summary>
        public const string PeriodicEvalAllowFutureDates = "PERIODIC_EVAL_ALLOW_FUTURE_DATES";
        public const string MatchNotificationThreshold = "MATCH_NOTIFICATION_THRESHOLD";
        public const string DefaultLocale = "DEFAULT_LOCALE";
        public const string PkiEnabled = "PKI_ENABLED";
        /// <summary>Y/N — when Y, staff password sign-in requires SMS OTP (verify-login).</summary>
        public const string LoginOtpEnabled = "LOGIN_OTP_ENABLED";
        public const string RegistrationSmsEnabled = "REGISTRATION_SMS_ENABLED";
        public const string RegistrationEmailEnabled = "REGISTRATION_EMAIL_ENABLED";
        /// <summary>Y/N — when Y, jobseeker registration gates on Nafath JobSeekerDetails.</summary>
        public const string NafathRegistrationCheckEnabled = "NAFATH_REGISTRATION_CHECK_ENABLED";
        /// <summary>Y/N — when Y, Nafath JobSeekerDetails uses in-process fixtures instead of BaseUrl.</summary>
        public const string NafathUseMock = "NAFATH_USE_MOCK";
        /// <summary>
        /// How registration treats Nafath active status: NONE, EXPIRY_DATE, ACTIVE_FLAG, or BOTH.
        /// </summary>
        public const string NafathValidityCheck = "NAFATH_VALIDITY_CHECK";
        /// <summary>Y/N — when Y, TRA licensed companies / active licenses use in-process fixtures instead of live URLs.</summary>
        public const string TraLicensesUseMock = "TRA_LICENSES_USE_MOCK";
        /// <summary>Comma-separated extensions without dots (e.g. pdf,doc,docx).</summary>
        public const string AttachmentAllowedExtensions = "ATTACHMENT_ALLOWED_EXTENSIONS";
        /// <summary>Maximum upload size in megabytes for attachments.</summary>
        public const string AttachmentMaxSizeMb = "ATTACHMENT_MAX_SIZE_MB";
        public const string JobApplicationUndertakingEn = "JOB_APPLICATION_UNDERTAKING_EN";
        public const string JobApplicationUndertakingAr = "JOB_APPLICATION_UNDERTAKING_AR";
        /// <summary>Social insurance deduction percent applied on offer wage total (e.g. 8).</summary>
        public const string SocialInsuranceDeductionPercent = "SOCIAL_INSURANCE_DEDUCTION_PERCENT";
        /// <summary>HH:mm wall-clock — interview/assessment slots at or after this time roll to the next working day.</summary>
        public const string SchedulingDayEndTime = "SCHEDULING_DAY_END_TIME";
        /// <summary>JSON array of degree ranks (rank 1 = highest) used to pick Nafath higher-education.</summary>
        public const string EducationDegreeRanks = "EDUCATION_DEGREE_RANKS";
    }
}

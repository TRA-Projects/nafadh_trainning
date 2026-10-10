namespace Bidding_Models.Jobseekers.Jobs
{
    public class CompanyGridRowDto
    {
        public int ProfileId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNameEn { get; set; }
        public string CompanyNameAr { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CrNumber { get; set; }
        public string Website { get; set; }
        public string Service { get; set; }
        public string ClassDc { get; set; }
        /// <summary>True when CR is in TRA LisensedCompanys list.</summary>
        public bool IsLicensed { get; set; }
        public bool IsAuthorized { get; set; }
        public string LastVerifiedAtIso { get; set; }
        public string VerificationSource { get; set; }
    }
}

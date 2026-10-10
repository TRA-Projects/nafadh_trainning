namespace Bidding_Models.Jobseekers.Jobs
{
    public class CompanyVerificationResultDto
    {
        public int ProfileId { get; set; }
        public string Authorized { get; set; }
        public string LastVerifiedAtIso { get; set; }
        public string VerificationSource { get; set; }
    }
}

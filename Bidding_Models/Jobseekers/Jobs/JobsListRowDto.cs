using System.Collections.Generic;

namespace Bidding_Models.Jobseekers.Jobs
{
    public class JobsListRowDto
    {
        public int Id { get; set; }
        public string JobReference { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string CompanyEn { get; set; }
        public string CompanyAr { get; set; }
        public string PublishDate { get; set; }
        public string DueDate { get; set; }
        public int ApplicationsCount { get; set; }
        public string Status { get; set; }
        public string NominationType { get; set; }
        public List<string> ApplicationMethodIds { get; set; }
        public int OwnerProfileId { get; set; }
    }
}

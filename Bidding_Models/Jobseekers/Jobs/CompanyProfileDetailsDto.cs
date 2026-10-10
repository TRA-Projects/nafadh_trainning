using System.Collections.Generic;

namespace Bidding_Models.Jobseekers.Jobs
{
    public class CompanyProfileDetailsDto : CompanyGridRowDto
    {
        public string About { get; set; }
        public string MissionVision { get; set; }
        public int Address { get; set; }
        public List<int> ServicesOffered { get; set; }
        public string ProfileImage { get; set; }
        public List<CompanySocialProfileDto> SocialProfiles { get; set; }
        public List<CompanyPortfolioItemDto> PortfolioItems { get; set; }
    }

    public class CompanySocialProfileDto
    {
        public int Id { get; set; }
        public string SocialProfileName { get; set; }
        public string SocialProfileUrl { get; set; }
    }

    public class CompanyPortfolioItemDto
    {
        public int Id { get; set; }
        public string PortfolioTitle { get; set; }
        public string PortfolioType { get; set; }
        public string PortfolioDescription { get; set; }
    }
}

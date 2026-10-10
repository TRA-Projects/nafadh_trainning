using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("JobSeekerPortfolioItemTechStacks")]
    public class JobSeekerPortfolioItemTechStack
    {
        public int Id { get; set; }

        public int JobSeekerPortfolioItemId { get; set; }

        public JobSeekerPortfolioItem JobSeekerPortfolioItem { get; set; }

        public int CatalogTechStackId { get; set; }

        public CatalogSkill CatalogTechStack { get; set; }
    }
}

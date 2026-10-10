using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("JobSeekerPortfolioItemSkills")]
    public class JobSeekerPortfolioItemSkill
    {
        public int Id { get; set; }

        public int JobSeekerPortfolioItemId { get; set; }

        public JobSeekerPortfolioItem JobSeekerPortfolioItem { get; set; }

        public int CatalogSkillId { get; set; }

        public CatalogSkill CatalogSkill { get; set; }
    }
}

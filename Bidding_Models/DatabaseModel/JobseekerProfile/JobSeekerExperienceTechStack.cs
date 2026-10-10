using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("JobSeekerExperienceTechStacks")]
    public class JobSeekerExperienceTechStack
    {
        public int Id { get; set; }

        public int JobSeekerExperienceId { get; set; }

        public JobSeekerExperience JobSeekerExperience { get; set; }

        public int CatalogTechStackId { get; set; }

        public CatalogSkill CatalogTechStack { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("JobSeekerExperienceSkills")]
    public class JobSeekerExperienceSkill
    {
        public int Id { get; set; }

        public int JobSeekerExperienceId { get; set; }

        public JobSeekerExperience JobSeekerExperience { get; set; }

        public int CatalogSkillId { get; set; }

        public CatalogSkill CatalogSkill { get; set; }
    }
}

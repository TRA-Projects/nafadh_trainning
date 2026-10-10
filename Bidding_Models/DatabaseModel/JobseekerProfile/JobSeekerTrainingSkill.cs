using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("JobSeekerTrainingSkills")]
    public class JobSeekerTrainingSkill
    {
        public int Id { get; set; }

        public int JobSeekerTrainingId { get; set; }

        public JobSeekerTraining JobSeekerTraining { get; set; }

        public int CatalogSkillId { get; set; }

        public CatalogSkill CatalogSkill { get; set; }
    }
}

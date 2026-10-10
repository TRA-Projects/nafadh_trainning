using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("JobSeekerTrainingTechStacks")]
    public class JobSeekerTrainingTechStack
    {
        public int Id { get; set; }

        public int JobSeekerTrainingId { get; set; }

        public JobSeekerTraining JobSeekerTraining { get; set; }

        public int CatalogTechStackId { get; set; }

        public CatalogSkill CatalogTechStack { get; set; }
    }
}

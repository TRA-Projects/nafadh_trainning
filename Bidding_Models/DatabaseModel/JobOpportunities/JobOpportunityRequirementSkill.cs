using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobOpportunityRequirementSkills")]
    public class JobOpportunityRequirementSkill
    {
        public int Id { get; set; }

        public int JobOpportunityRequirementId { get; set; }

        public JobOpportunityRequirement Requirement { get; set; }

        public int CatalogSkillId { get; set; }

        public CatalogSkill CatalogSkill { get; set; }

        public bool IsSecondary { get; set; }

        public int? YearsOfExperience { get; set; }

        public int? YearsMargin { get; set; }

        public int SortOrder { get; set; }
    }
}

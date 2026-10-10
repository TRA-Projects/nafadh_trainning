using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobOpportunityRequirementTechStacks")]
    public class JobOpportunityRequirementTechStack
    {
        public int Id { get; set; }

        public int JobOpportunityRequirementId { get; set; }

        public JobOpportunityRequirement Requirement { get; set; }

        public int CatalogTechStackId { get; set; }

        public CatalogSkill CatalogTechStack { get; set; }

        public int? YearsOfExperience { get; set; }

        public int? YearsMargin { get; set; }

        public int SortOrder { get; set; }
    }
}

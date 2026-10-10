using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.Credentials
{
    [Table("CredentialDefinitionTechStacks")]
    public class CredentialDefinitionTechStack
    {
        public int Id { get; set; }

        public int CredentialDefinitionId { get; set; }

        public CredentialDefinition CredentialDefinition { get; set; }

        public int CatalogTechStackId { get; set; }

        public CatalogSkill CatalogTechStack { get; set; }

        /// <summary>Optional free-text questions for this technology (single language).</summary>
        [MaxLength(2000)]
        public string Questions { get; set; }
    }
}

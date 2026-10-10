using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.Credentials
{
    [Table("CredentialDefinitionSkills")]
    public class CredentialDefinitionSkill
    {
        public int Id { get; set; }

        public int CredentialDefinitionId { get; set; }

        public CredentialDefinition CredentialDefinition { get; set; }

        public int CatalogSkillId { get; set; }

        public CatalogSkill CatalogSkill { get; set; }

        /// <summary>Optional free-text interview/assessment questions for this skill (single language).</summary>
        [MaxLength(2000)]
        public string Questions { get; set; }
    }
}

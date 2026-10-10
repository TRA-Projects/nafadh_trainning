using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.Credentials
{
    [Table("CredentialDefinitionBasicSkills")]
    public class CredentialDefinitionBasicSkill
    {
        public int Id { get; set; }

        public int CredentialDefinitionId { get; set; }

        public CredentialDefinition CredentialDefinition { get; set; }

        /// <summary>FK into the merged <see cref="CatalogSkill"/> table (rows with <see cref="CatalogSkillKind.Personal"/>).</summary>
        public int CatalogBasicSkillId { get; set; }

        public CatalogSkill CatalogBasicSkill { get; set; }

        /// <summary>Optional free-text questions for this basic skill (single language).</summary>
        [MaxLength(2000)]
        public string Questions { get; set; }
    }
}

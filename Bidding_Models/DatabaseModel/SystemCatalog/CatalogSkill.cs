using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.SystemCatalog
{
    /// <summary>
    /// Admin-managed skill taxonomy row (distinct from legacy <c>Skills</c> table).
    /// Merged table: <see cref="Kind"/> discriminates professional / basic / personal skills and tech stacks.
    /// </summary>
    [Table("CatalogSkills")]
    public class CatalogSkill
    {
        public int Id { get; set; }

        /// <summary>Null for tech stacks (flat list without categories).</summary>
        public int? CategoryId { get; set; }

        public CatalogSkillCategory Category { get; set; }

        public CatalogSkillKind Kind { get; set; } = CatalogSkillKind.Professional;

        [Required]
        [MaxLength(256)]
        public string NameEn { get; set; }

        [Required]
        [MaxLength(256)]
        public string NameAr { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

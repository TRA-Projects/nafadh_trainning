using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.SystemCatalog
{
    /// <summary>Admin-managed skill taxonomy category (all skill kinds).</summary>
    [Table("CatalogSkillCategories")]
    public class CatalogSkillCategory
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string NameEn { get; set; }

        [Required]
        [MaxLength(256)]
        public string NameAr { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<CatalogSkill> Skills { get; set; } = new List<CatalogSkill>();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.SystemCatalog
{
    /// <summary>
    /// Lookup group header (e.g. NAT, CURR, RJCT_RSN). Same code is used in API paths and UI configCode.
    /// </summary>
    [Table("SystemConfigGroups")]
    public class SystemConfigGroup
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(16)]
        public string Code { get; set; }

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

        public List<SystemConfigValue> Values { get; set; } = new List<SystemConfigValue>();
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.SystemCatalog
{
    /// <summary>
    /// Bilingual option row inside a <see cref="SystemConfigGroup"/>.
    /// <see cref="ValueKey"/> is the stable id stored on domain records (API field <c>id</c>).
    /// </summary>
    [Table("SystemConfigValues")]
    public class SystemConfigValue
    {
        public int Id { get; set; }

        public int ConfigGroupId { get; set; }

        public SystemConfigGroup ConfigGroup { get; set; }

        /// <summary>Business key exposed as API <c>id</c> (e.g. OM, full-time, missing-certificate).</summary>
        [Required]
        [MaxLength(64)]
        public string ValueKey { get; set; }

        [MaxLength(16)]
        public string Code { get; set; }

        [Required]
        [MaxLength(512)]
        public string LabelEn { get; set; }

        [Required]
        [MaxLength(512)]
        public string LabelAr { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>Main reason row: UI may show a sub-reason picker (interview rejection).</summary>
        public bool? SubReasonDropdown { get; set; }

        /// <summary>Parent row's <see cref="ValueKey"/> (not DB Id) for hierarchical catalogs.</summary>
        [MaxLength(64)]
        public string ParentValueKey { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.SystemCatalog
{
    /// <summary>
    /// Platform-wide toggles and settings (PKI, registration OTP, match threshold, …).
    /// Consumed via admin system-variables API and server-side feature flags.
    /// </summary>
    [Table("SystemVariables")]
    public class SystemVariable
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string VariableKey { get; set; }

        [Required]
        [MaxLength(256)]
        public string VariableName { get; set; }

        [MaxLength(256)]
        public string VariableNameEn { get; set; }

        [MaxLength(256)]
        public string VariableNameAr { get; set; }

        /// <summary>Boolean | Integer | String — parsed when read.</summary>
        [Required]
        [MaxLength(32)]
        public string ValueType { get; set; }

        [Required]
        public string Value { get; set; }

        [MaxLength(512)]
        public string Description { get; set; }

        [MaxLength(512)]
        public string DescriptionEn { get; set; }

        [MaxLength(512)]
        public string DescriptionAr { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

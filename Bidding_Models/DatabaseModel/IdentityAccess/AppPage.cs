using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.IdentityAccess
{
    [Table("AppPages")]
    public class AppPage
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string Code { get; set; }

        /// <summary>Jobseeker | Freelancer | BiddingWeb</summary>
        [Required]
        [MaxLength(32)]
        public string SystemCode { get; set; }

        [Required]
        [MaxLength(512)]
        public string Url { get; set; }

        [Required]
        [MaxLength(200)]
        public string NameEn { get; set; }

        [Required]
        [MaxLength(200)]
        public string NameAr { get; set; }

        public bool IsActive { get; set; } = true;

        public int RequiredActionFlags { get; set; } = PageActionFlags.View;

        [MaxLength(32)]
        public string DefaultDataScope { get; set; } = PageDataScopes.Mine;

        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public static class AppPageSystemCodes
    {
        public const string Jobseeker = "Jobseeker";
        public const string Freelancer = "Freelancer";
        public const string BiddingWeb = "BiddingWeb";
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.IdentityAccess
{
    [Table("Organizations")]
    public class Organization
    {
        public int Id { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string CrNumber { get; set; }

        [MaxLength(256)]
        public string CompanyNameEnglish { get; set; }

        [MaxLength(256)]
        public string CompanyNameArabic { get; set; }

        [MaxLength(256)]
        public string CompanyNameWithLegalEnglish { get; set; }

        [MaxLength(256)]
        public string CompanyNameWithLegalArabic { get; set; }

        [MaxLength(100)]
        public string RyadaNo { get; set; }

        public string RegistrationDate { get; set; }

        public string EstablishmentDate { get; set; }

        public string ExpiryDate { get; set; }

        public string SubjectToForeignInvestment { get; set; }

        public bool IsLicensed { get; set; }

        public bool IsAuthorized { get; set; }

        public DateTime? LastVerified { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

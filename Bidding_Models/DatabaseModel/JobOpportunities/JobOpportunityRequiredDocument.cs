using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobOpportunityRequiredDocuments")]
    public class JobOpportunityRequiredDocument
    {
        public int Id { get; set; }

        public int JobOpportunityId { get; set; }

        public JobOpportunity JobOpportunity { get; set; }

        [Required]
        [MaxLength(256)]
        public string NameEn { get; set; }

        [Required]
        [MaxLength(256)]
        public string NameAr { get; set; }

        [MaxLength(64)]
        public string FileTypeValueKey { get; set; }

        [MaxLength(32)]
        public string Importance { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.JobseekerProfile;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobOpportunityParticipationAttachments")]
    public class JobOpportunityParticipationAttachment
    {
        public int Id { get; set; }

        public int ParticipationId { get; set; }

        public JobOpportunityParticipation Participation { get; set; }

        public int AttachmentId { get; set; }

        public Attachment Attachment { get; set; }

        public int? RequiredDocumentId { get; set; }

        public JobOpportunityRequiredDocument RequiredDocument { get; set; }

        [Required]
        [MaxLength(32)]
        public string Purpose { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

using System.Collections.Generic;

namespace Bidding_Models.Jobseekers.Jobs
{
    public class PostJobResponseDto
    {
        public int Id { get; set; }
        public string JobReference { get; set; }
        public List<AttachmentDraft> Attachments { get; set; }
    }
}

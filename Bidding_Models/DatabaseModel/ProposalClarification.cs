using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class ProposalClarification : EntityTrail
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; }

        public bool IsDocumentRequired { get; set; }

        public Bid Bid { get; set; }
        public int BidId { get; set; }

        public List<Notification> NotificationList{ get; set; }

        public List<ProposalClarificationDocuments> ProposalClarificationDocumentsList { get; set; }

        [NotMapped]
        public string EncryptedBidId { get; set; }

        [NotMapped]
        public int ProposalClarificationId { get; set; }
        [NotMapped]
        public string EncryptedId { get; set; }
        [NotMapped]
        public List<IFormFile> Files { get; set; }

        [NotMapped]
        public List<DocumentWrapper> DocumentWrapperList { get; set; }


    }
}

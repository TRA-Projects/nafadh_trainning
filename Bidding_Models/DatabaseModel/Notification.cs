using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public AppUser ForUser { get; set; }
        public string ForUserId { get; set; }
        public Request Request { get; set; }

        public int? RequestId { get; set; }
        public Bid Bid { get; set; }
        public int? BidId { get; set; }


        public NDA NDA { get; set; }
        public int? NDAId { get; set; }

        public DateTime NotificationDate { get; set; }
        public bool IsReaded { get; set; }

        public NotificationType NotificationType { get; set; }
        public int? NotificationTypeId { get; set; }

        public ProposalClarification ProposalClarification { get; set; }
        public int? ProposalClarificationId { get; set; }


        public RequestClarificationQuestion RequestClarificationQuestion { get; set; }
        public int? RequestClarificationQuestionId { get; set; }

        [NotMapped]
        public string EncryptedRequestId { get; set; }

        [NotMapped]
        public string EncryptedBidId { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        [NotMapped]
        public PagedResult<Notification> PagedResult { get; set; }
        [NotMapped]
        public string EncryptedProposalClarificationId { get; set; }
        

    }
}

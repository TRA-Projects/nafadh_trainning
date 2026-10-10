using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class NDA : EntityTrail
    {
        public NDA()
        {
            NotificationList = new List<Notification>();
            NDADocumentList = new List<NDADocument>();
        }
        public int Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [StringLength(40)]
        public string EncryptedId { get; set; }
        public Request Request { get; set; }

        public int RequestId { get; set; }


        public AppUser Vendor { get; set; }
        public string VendorId { get; set; }

        public List<Notification> NotificationList { get; set; }

        public List<NDADocument> NDADocumentList { get; set; }

        public NDAStatus Status { get; set; }

        [NotMapped]
        public string NDAStatusString { get {
                string status = string.Empty;
                switch (Status)
                {
                    case NDAStatus.Send:
                        status = "Submitted";
                        break;
                    case NDAStatus.Approve:
                        status = "Approved";
                        break;
                    case NDAStatus.Reject:
                        status = "Rejected";
                        break;
                    default:
                        break;
                }
                return status;
            } }


        [NotMapped]
        public string EncryptedRequestId { get; set; }        

        [NotMapped]
        public List<DocumentWrapper> DocumentWrapper { get; set; }


        [NotMapped]
        public List<RequestNDADocument> RequestNDADocumentList { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class VendorExtensionRequest
    {
        public int Id { get; set; }
        public AppUser Vendor { get; set; }
        public string VendorId { get; set; }

        public Request Request { get; set; }

        public int RequestId { get; set; }
        public DateTime? Lastsubmissiondate { get; set; }
        public DateTime? LastClarificationDate { get; set; }
        public string ExpiryExtendedReason { get; set; }

        [NotMapped]
        public string EncryptedRequestId { get; set; }
    }
}

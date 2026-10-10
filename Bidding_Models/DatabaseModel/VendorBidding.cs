using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class VendorBidding : EntityTrail
    {
        public long Id { get; set; }
        public Bidding Bidding { get; set; }
        public long BiddingId { get; set; }
        public AppUser Vendor { get; set; }
        public string VendorId { get; set; }
        public int Price { get; set; }

        public bool IsActive { get; set; }

        public string SubmittedBy { get; set; }
        public DateTime SubmittedDate { get; set; }
    }
}

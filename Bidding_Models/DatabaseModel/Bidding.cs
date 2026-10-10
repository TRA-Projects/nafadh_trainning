using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class Bidding
    {
        public long Id { get; set; }
        public Request Request { get; set; }
        public int RequestId { get; set; }

        public bool IsApproved { get; set; }

        public DateTime BiddingStart { get; set; }

        public DateTime BiddingEnd { get; set; }
        public string SubmittedById { get; set; }

        public DateTime SubmittedDate { get; set; }

        public List<VendorBidding> VendorBiddingList { get; set; }
    }
}

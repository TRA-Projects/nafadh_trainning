using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class Subscriptions :EntityTrail
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }

        public int? SubscriptionPackageId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TrackNo { get; set; }

        public Profile Profile { get; set; }

        public SubscriptionPackages SubscriptionPackage { get; set; }
        public  SubscriptionPayments SubscriptionPayment { get; set; }
    }
}

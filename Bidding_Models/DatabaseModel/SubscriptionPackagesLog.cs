using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class SubscriptionPackagesLog : EntityTrail
    {
        public int Id { get; set; }
        public int SubscriptionPackagesId { get; set; }
        public decimal Price { get; set; }
        public int PeriodPerMonth { get; set; }

        public SubscriptionPackages SubscriptionPackage { get; set; }
    }
}

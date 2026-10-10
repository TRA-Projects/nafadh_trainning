using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class SubscriptionPackages
    {
        public int Id { get; set; }
        public int ProfileType { get; set; }
        public decimal Price { get; set; }
        public int PeriodPerMonth { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Subscriptions> Subscriptions { get; set; }
        public ICollection<SubscriptionPackagesLog> Logs { get; set; }
    }
}

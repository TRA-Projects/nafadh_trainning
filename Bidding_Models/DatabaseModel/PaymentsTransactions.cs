using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class PaymentsTransactions :EntityTrail
    {
        public int Id { get; set; }
        public int ProfileId{ get; set; }
        public string TrackNo { get; set; }
        public string SessionId { get; set; }
        public string CurrencyType { get; set; }
        public decimal Price { get; set; }
        public string PaymentId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string PaymentResult { get; set; }
        public SubscriptionPayments SubscriptionPayments { get; set; }
        public Profile Profile{ get; set; }
    }
}

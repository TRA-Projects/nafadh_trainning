using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class SubscriptionPayments : EntityTrail
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public decimal Price { get; set; }
        public string TrackNo { get; set; }
        public string RRN { get; set; }
        public string AmwalTransactionId { get; set; }
         
        public string AmwalTrackId { get; set; }
        public string AmwalPaymentId { get; set; }
        public int? PaymentsTransactionsId { get; set; }
        public DateTime TransactionDate { get; set; }
        
        public PaymentsTransactions PaymentsTransaction { get; set; }
        public Subscriptions Subscription { get; set; }

    }
}

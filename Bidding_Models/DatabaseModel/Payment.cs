using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class Payment
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string OrderId { get; set; }
        public string BankTrackId { get; set; }
        public string BankRefNumber { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal Amount{ get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal MerchantAmount { get; set; }
        [MaxLength(10)]
        public string Currency { get; set; }
        public string CardName { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string FailureMessage { get; set; }
        public string CollectionReq { get; set; }
        [MaxLength(250)]
        public string Code { get; set; }
        [MaxLength(250)]
        public string OrderStatus { get; set; }
        [MaxLength(250)]
        public string Payment_mode { get; set; }
      

        [MaxLength(100)]
        public string TransactionDate { get; set; }
        public int ProfileId { get; set; }
        public DateTime CratedOn { get; set; }= DateTime.Now;
        public Profile Profile { get; set; }
        public ProfileProjectMilestone ProfileProjectMilestone { get; set; }

    }
}

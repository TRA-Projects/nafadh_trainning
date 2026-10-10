using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class RequestParticipents
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public AppUser User { get; set; }

        public string Currency { get; set; }

        public decimal FinicialAmount { get; set; }


        [NotMapped]
        public string FinicialAmountStr
        {
            get { return FinicialAmount == 0 ? "" : FinicialAmount.ToString("N"); }
            set { FinicialAmount = value == "" ? 0 : Convert.ToDecimal(value); }
        }

        public string Comments { get; set; }
        public bool IsAwarded { get; set; }

        public int RequestId { get; set; }
        public Request Request { get; set; }

        public DateTime? AwardedDate { get; set; }
        public string AwardedById { get; set; }

        [NotMapped]


        public string EncryptedRequestId { get; set; }
        [NotMapped]

        public string BidEncryptedId { get; set; }
        [NotMapped]
        public int BidId { get; set; }
    }
}


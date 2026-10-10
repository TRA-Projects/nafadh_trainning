using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class BidQuotation
    {
        public int Id { get; set; }
        [RegularExpression(@"^.*\D+.*$", ErrorMessage = "Service Type should not contain numbers only")]
        public string ServiceType { get; set; }
        [RegularExpression(@"^.*\D+.*$", ErrorMessage = "Description should not contain numbers only")]
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

    

        public string PriceStr
        {

            get { return Price == 0 ? "" : Price.ToString("N"); }
            set { Price = value == "" ? 0 : Convert.ToDecimal(value); }

        }

     
        public Bid Bid { get; set; }
        public int BidId { get; set; }

        [NotMapped]
        public int lengthNo { get; set; }
    }
}

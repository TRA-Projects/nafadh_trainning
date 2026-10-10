using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class BidDocument
    {
        public int Id { get; set; }
        public string DocName { get; set; }
        public Bid Bid { get; set; }
        public int BidId { get; set; }

        public RequestRequiredDocument RequestRequiredDocument { get; set; }
        public int RequestRequiredDocumentId { get; set; }

        [NotMapped]
        public bool IsDeleted { get; set; }
    }
}

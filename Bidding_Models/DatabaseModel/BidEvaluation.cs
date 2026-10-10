using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class BidEvaluation
    {
        public int Id { get; set; }
        public int ObtainedScore { get; set; }
        public Bid Bid { get; set; }
        public int BidId { get; set; }
        public RequestEvaluationCriteria RequestEvaluationCriteria { get; set; }
        public int RequestEvaluationCriteriaId { get; set; }
        [NotMapped]
        public string BidEncryptedId { get; set; }
    }
}

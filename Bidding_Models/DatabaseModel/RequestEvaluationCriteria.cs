using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class RequestEvaluationCriteria
    {
        public int Id { get; set; }
   
        public string CriteriaName { get; set; }

    
        public string CategoryName { get; set; }

        public int CriteriaScore { get; set; }
        public Request Request { get; set; }
        public int RequestId { get; set; }

        public int Type { get; set; }
        public List<BidEvaluation> BidEvaluationList { get; set; }


        [NotMapped]
        public int ObtainedScore { get; set; }


        [NotMapped]
        public bool IsSelected { get; set; }
    }
}

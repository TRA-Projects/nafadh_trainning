using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class RequestQuestion
    {

        public string Answer { get; set; }
        public Request Request { get; set; }
        public int RequestId { get; set; }
        public Question Question { get; set; }
        public int QuestionId { get; set; }

        
        [NotMapped]
        public string[] AnswerArray { get; set; }

        [NotMapped]
        public int QuestionType { get; set; }
    }
}

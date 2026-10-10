using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
  public  class UserTrainingAnswer
    {
        public int Id { get; set; }
        public int QuestionNum { get; set; }

        public int AnswerNum { get; set; }

        public UserTraining UserTraining { get; set; }
        public int UserTrainingId { get; set; }
    }
}

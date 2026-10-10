using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
  public  class UserTraining
    {

        public int Id { get; set; }
        public string TrainingName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [NotMapped]
        public String CreatedDateString { get; set; } 

        public AppUser User { get; set; }
        public string UserId { get; set; }

        public int Status { get; set; }

        
        public List<UserTrainingAnswer> UserTrainingAnswers { get; set; }
    }
}

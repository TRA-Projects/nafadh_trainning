using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
  public class Question : EntityTrail
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public string ArabicText { get; set; }
        public int QuestionType { get; set; }
        public string Options { get; set; }
        public string ArabicOptions { get; set; }
        public bool IsActive { get; set; }
        public TendorCategory TendorCategory { get; set; }

        public List<RequestQuestion> RequestQuestionList { get; set; }
        public int TendorCategoryId { get; set; }
        [NotMapped]
        public string TendorCategoryName { get; set; }
        [NotMapped]
        public string TendorCategoryArabicName { get; set; }
    }
}

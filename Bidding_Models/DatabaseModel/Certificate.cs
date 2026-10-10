using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class Certificate
    {
        public int Id { get; set; }
        [RegularExpression(@"[a-zA-Z\s]*$", ErrorMessage = "Use English letters only please")]
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        [RegularExpression(@"[a-zA-Z\s]*$", ErrorMessage = "Use English letters only please")]
        public string IssuerEn { get; set; }
        public string IssuerAr { get; set; }
        public DateTime IssueDate { get; set; }

        [RegularExpression(@"[a-zA-Z\s]*$", ErrorMessage = "Use English letters only please")]
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }

        [RegularExpression(@"[a-zA-Z\s]*$", ErrorMessage = "Use English letters only please")]
        public string BusinessSectorEn { get; set; }
        public string BusinessSectorAr { get; set; }

        public string Image { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        [NotMapped]
        public List<DocumentWrapper> DocumentList { get; set; }
        public DateTime CrreatedDate { get; set; } = DateTime.Now;
        public Profile Profile { get; set; }
        public int ProfileId { get; set; }
        public bool IsActive { get; set; } = false;


    }
}

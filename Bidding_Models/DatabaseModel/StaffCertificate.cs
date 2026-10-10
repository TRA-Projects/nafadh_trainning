using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class StaffCertificate
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string Picture { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public DateTime IssueDate { get; set; }
        public Staff Staff { get; set; }
        public int StaffId { get; set; }

        public DateTime CrreatedDate { get; set; } = DateTime.Now;


    }
}

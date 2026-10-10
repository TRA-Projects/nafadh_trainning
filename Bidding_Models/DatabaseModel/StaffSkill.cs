using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class StaffSkill
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public Staff Staff { get; set; }
        public int StaffId { get; set; }

        public DateTime CrreatedDate { get; set; } = DateTime.Now;


    }
}

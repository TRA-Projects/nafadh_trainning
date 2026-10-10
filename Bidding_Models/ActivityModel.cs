using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
   public class ActivityModel
    {
            public int Id { get; set; }
            public string Code { get; set; }
            public string NameAr { get; set; }
            public string NameEn { get; set; }
            public bool IsActive { get; set; }
            public string ClassNameAr { get; set; }
            public string ClassNameEn { get; set; }
            public string GroupAr { get; set; }
            public string GroupEn { get; set; }
            public string DivisionAr { get; set; }
            public string DivisionEn { get; set; }

            public int ?DivisionId { get; set; }
            public int? GroupId { get; set; }
            public int ClassId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class ActivityLookup
    {
        [Key]

        public int Id { get; set; }
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("ClassId")]
        public int ClassId { get; set; }

        public ActivityClass Class { get; set; }


    }

    public class ActivityClass
    {
        [Key]

        public int Id { get; set; }
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("GroupId")]
        public int GroupId { get; set; }
        public ActivityGroup Group { get; set; }

        public List <ActivityLookup> Activities { get; set; }
    }

    public class ActivityGroup
    {
        [Key]

        public int Id { get; set; }
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("DivisionId")]
        public int DivisionId { get; set; }
        public ActivityDivision Division { get; set; }

        public List<ActivityClass> ActivityClasses { get; set; }
    }

    public class ActivityDivision
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public bool IsActive { get; set; }

        public List<ActivityGroup> ActivityGroups { get; set; }
    }
}

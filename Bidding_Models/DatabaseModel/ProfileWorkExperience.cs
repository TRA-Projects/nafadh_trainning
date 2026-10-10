using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel
{
    public class ProfileWorkExperience
    {
        public int Id { get; set; }

        [NotMapped]        
        public int Slno { get; set; }
        public Profile Profile { get; set; }
        public int? ProfileId { get; set; }
        
        public string DesignationTitle { get; set; }
        
        public string OrganizationName { get; set; }
        
        public string EmploymentType { get; set; }
        
        public string Location { get; set; }
        
        public string ModeOfWork { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public bool IsCurrent { get; set; }
    }
}
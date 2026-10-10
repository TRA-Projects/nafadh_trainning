using System;
using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class ProfileWorkExperienceRequestDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Profile Id is required")]
        public int ProfileId { get; set; }
        
        [Required(ErrorMessage = "Designation Title is required")]
        public string DesignationTitle { get; set; }
        
        [Required(ErrorMessage = "Organization Name is required")]
        public string OrganizationName { get; set; }
        
        [Required(ErrorMessage = "Employment Type is required")]
        public string EmploymentType { get; set; }
        
        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }
        
        [Required(ErrorMessage = "Mode of Work is required")]
        public string ModeOfWork { get; set; }
        
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; }
                
        public DateTime? EndDate { get; set; }
        
        [Required(ErrorMessage = "Is Current is required")]
        public bool IsCurrent { get; set; }
    }
}
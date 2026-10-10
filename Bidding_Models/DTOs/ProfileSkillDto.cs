using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class ProfileSkillRequestDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "ProfileId is required")]
        public int ProfileId { get; set; }
        
        [Required(ErrorMessage = "SkillName is required")]
         public string SkillName { get; set; }
      
         [Required(ErrorMessage = "SkillLevel is required")]
        public string SkillLevel { get; set; }
    }
}
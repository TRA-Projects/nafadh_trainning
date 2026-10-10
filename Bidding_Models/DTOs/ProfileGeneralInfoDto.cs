
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class ProfileGeneralInfoDto
    {
        [Required(ErrorMessage = "ProfileId is required")]
        public int ProfileId { get; set; }
        //[Required(ErrorMessage = "CompanyName is required")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        
        public string Website { get; set; }
        [Required(ErrorMessage = "About is required")]
        public string About { get; set; }
        [Required(ErrorMessage = "Mission & Vision is required")]
        public string MissionVision { get; set; }
        public string ProfileImage { get; set; }
        [Required(ErrorMessage = "ServicesOffered is required")]
        public List<int> ServicesOffered { get; set; }
        
    }
}
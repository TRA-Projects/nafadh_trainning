using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class ProfilePortfolioRequestDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "ProfileId is required")]
        public int ProfileId { get; set; }
        
        [Required(ErrorMessage = "PortfolioType is required")]
        public string PortfolioType { get; set; }
        
        [Required(ErrorMessage = "PortfolioTitle is required")]
        public string PortfolioTitle { get; set; }
        
        [Required(ErrorMessage = "PortfolioDescription is required")]
        public string PortfolioDescription { get; set; }
        
        public string PortfolioLink { get; set; }
        public string Sector { get; set; }
        public string Cost { get; set; }
        public string Duration { get; set; }
        public string Participation { get; set; }
    }
}
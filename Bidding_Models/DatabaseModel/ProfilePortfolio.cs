using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel
{
    public class ProfilePortfolio
    {
        public int Id { get; set; }

        [NotMapped]
        public int Slno { get; set; }
        public Profile Profile { get; set; }
        public int? ProfileId { get; set; }
        
        public string PortfolioType { get; set; }
        
        public string PortfolioTitle { get; set; }
        
        public string PortfolioDescription { get; set; }
        
        public string PortfolioLink { get; set; }
        public string Sector { get; set; } 
        public string Cost { get; set; }
        public string Duration { get; set; }
        public string Participation { get; set; }
    }
}
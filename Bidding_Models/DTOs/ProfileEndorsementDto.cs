using System.Collections.Generic;

namespace Bidding_Models.DTOs
{
    public class ProfileEndorsementDto
    {
        public string Skill { get; set; }
        public List<ProfileEndorsementItem> Endorsements { get; set; } = new List<ProfileEndorsementItem>();
    }
    public class ProfileEndorsementItem
    {
        public string ProjectTitle { get; set; }
        public string OperatorName { get; set; }
        public string OperatorImage { get; set; }         
      
    }
}
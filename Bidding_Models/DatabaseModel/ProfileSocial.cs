namespace Bidding_Models.DatabaseModel
{
    public class ProfileSocial
    {
        public int Id { get; set; }
        
        public Profile Profile { get; set; }
        public int? ProfileId { get; set; }
        
        public string SocialProfileUrl { get; set; }
        
        public string SocialProfileType { get; set; }
        
        public string SocialProfileName { get; set; }
        
        
        
    }
}
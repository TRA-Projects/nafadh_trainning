using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bidding_Models.DatabaseModel;

namespace Bidding_Models.DTOs
{
    public class CreateSocialProfileRequestDto
    {
        public  int ProfileId { get; set; }
        public List<ProfileSocial> SocialProfiles { get; set; }
    }

    public class ProfileSocialResponseDto
    {
        public int Id { get; set; }
        
        public int? ProfileId { get; set; }
        
        public string SocialProfileUrl { get; set; }
        
        public string SocialProfileType { get; set; }
        
        public string SocialProfileName { get; set; }
    
    }
}
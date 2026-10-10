namespace Bidding_Models
{
    public class SsoLoginRequestDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string FreelancerModuleWebBaseUrl { get; set; }
    }
}

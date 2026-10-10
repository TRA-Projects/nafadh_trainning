namespace Bidding_Models
{
    public class SsoAuthResultDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string TempToken { get; set; }
        public string Mobile { get; set; }
        public string CivilId { get; set; }
    }
}

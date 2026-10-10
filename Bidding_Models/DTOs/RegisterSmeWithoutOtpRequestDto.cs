using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class RegisterSmeWithoutOtpRequestDto
    {
        [Required(ErrorMessage = "CR Number is required.")]
        public string CrNumber { get; set; }

        [Required(ErrorMessage = "Company Name is required.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Token is required.")]
        public string TempToken { get; set; }

        public string MissionVision { get; set; }
    }
}
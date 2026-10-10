using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class RegisterOperatorsRequestDto
    {
        [Required(ErrorMessage = "Token is required.")]
        public string TempToken { get; set; }

        [Required(ErrorMessage = "CR Number is required.")]
        public string CrNumber { get; set; }
        
        [Required(ErrorMessage = "Company Name is required.")]
        public string CompanyName { get; set; }
        
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "PhoneNumber is required.")]
        public string PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "PhoneNumber Confirmed is required.")]
        public bool PhoneNumberConfirmed { get; set; }
        
        [Required(ErrorMessage = "OTP is required.")]
        public string OTP { get; set; }
        
        [Required(ErrorMessage = "OTP Confirmed is required.")]
        public string OtpConfirmed { get; set; }
        public string AttachmentFileName { get; set; }
        
    }

}
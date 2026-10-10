using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class RegisterFreelancerWithoutOtpRequestDto
    {
        [Required(ErrorMessage = "Token is required.")]
        public string TempToken { get; set; }

        [Required(ErrorMessage = "CR Number is required.")]
        public string CrNumber { get; set; }

        [Required(ErrorMessage = "Civil Id is required.")]
        public string CivilId { get; set; }

        [Required(ErrorMessage = "Full Name is required."), MaxLength(150)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required."), MaxLength(8)]
        public string PhoneNumber { get; set; }
    }
}   
using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class RegisterFreelancerRequestDto
    {
        [Required(ErrorMessage = "Token is required.")]
        public string TempToken { get; set; }

        [Required(ErrorMessage = "CR Number is required.")]
        public string CrNumber { get; set; }


        [Required(ErrorMessage = "Civil Id is required.")]
        public string CivilId { get; set; }       

        
        [Required(ErrorMessage = "Full Name is required."),MaxLength(50)]
        //[RegularExpression(@"^[A-Za-z]+(?:[ ][A-Za-z]+)*$", ErrorMessage = "Name must contain only letters and space.")]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        
        
        [Required(ErrorMessage = "PhoneNumber is required."),MaxLength(8)]
        public string PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "PhoneNumber Confirmed is required.")]
        public bool PhoneNumberConfirmed { get; set; }
        
        [Required(ErrorMessage = "OTP is required."), MaxLength(6)]
        public string OTP { get; set; } 

        [Required(ErrorMessage = "OTP Confirmed is required.")]
        public string OtpConfirmed { get; set; }
    }
}   
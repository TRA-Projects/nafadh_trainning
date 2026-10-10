using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class RegisterJobseekerRequestDto
    {
        [Required(ErrorMessage = "Civil Id is required.")]
        public string CivilId { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        /// <summary>
        /// Optional. Ignored by the API — a server-generated password is stored instead.
        /// </summary>
        public string Password { get; set; }

        [Required(ErrorMessage = "English name is required.")]
        public string Name_en { get; set; }

        [Required(ErrorMessage = "Arabic name is required.")]
        public string Name_ar { get; set; }

        public string MobileNumber { get; set; }

        public string NationalityCode { get; set; }

        public string PrimarySpecificationId { get; set; }

        public string SubSpecificationId { get; set; }

        public string OTP { get; set; }

        public string OtpConfirmed { get; set; }

        /// <summary>
        /// Pre-registration JWT from PKI (<c>POST auth/TempToken</c>). Required when registering via PKI.
        /// </summary>
        public string TempToken { get; set; }
    }

    public class ValidateRegistrationOtpRequestDto
    {
        [Required(ErrorMessage = "OTP is required.")]
        public string OTP { get; set; }

        [Required(ErrorMessage = "OTP Confirmed is required.")]
        public string OtpConfirmed { get; set; }
    }

    public class JobseekerRegistrationOtpRequestDto
    {
        public string CivilId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class VerifyJobseekerEmailRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
    }
}

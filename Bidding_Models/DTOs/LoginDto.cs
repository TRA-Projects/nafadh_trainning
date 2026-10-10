



using Bidding_Models.DatabaseModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// When true, only internal staff roles may sign in (Admin / Operator / SME / Staff / company user).
        /// </summary>
        public bool StaffOnly { get; set; }
        /// <summary>
        /// When true (PKI mode on public login), JobSeeker / Freelancer password login is rejected.
        /// </summary>
        public bool DisallowPublicPasswordForJobSeekers { get; set; }
    }
    public class LoginVerifyRequestDto
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        public string EncryptedOtp { get; set; }
        public bool StaffOnly { get; set; }
    }
    public class LoginByCrOrMobileRequestDto
    {

        public string EncryptedCivilId { get; set; }
        public string EncryptedMobile { get; set; }
    }
    public class VerifyTempTokenRequestDto
    {
        public string Token { get; set; }
        public string CivilId { get; set; }
        public string Mobile { get; set; }
    }

    public class LoginResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public AppUser User { get; set; } = null;
        
    }
    public class LoginResponseDtoWrapper
    {
        public LoginResponseDto Data { get; set; }


    }

    public class SendVerificationRequest
    {
        [Required(ErrorMessage = "Phone number is required.")]
        public string PhoneNumber { get; set; }
        
       
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
    }
    // create RefreshTokenRequestDto
    public class RefreshTokenRequestDto
        {
            [Required(ErrorMessage = "Refresh token is required.")]
            public string RefreshToken { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            public string Email { get; set; }
        }
    
}
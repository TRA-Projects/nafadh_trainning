using System;
using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class ProfileEducationRequestDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage ="Profile ID is required.")] 
        public int ProfileId { get; set; }
        [Required(ErrorMessage = "Degree is required. ")]
        public string Degree { get; set; }
        
        [Required(ErrorMessage = "InstituteName required. ")]
        public string InstituteName { get; set; }
        
        [Required(ErrorMessage = "MajorSpecialization is required. ")]
        public string MajorSpecialization { get; set; }
        
        [Required(ErrorMessage = "Country is required. ")]
        public string Country { get; set; }
        
        [Required(ErrorMessage = "City is required. ")]
        public string City { get; set; }
       
        [Required(ErrorMessage = "YearOfPassing is required. "), MaxLength(4)]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Max/Min year length is 4 digits.")]
        public string YearOfPassing { get; set; }
        
        [Required(ErrorMessage = "Grade is required. ")]
        public string Grade { get; set; }
        public string FileName { get; set; }
    }
}
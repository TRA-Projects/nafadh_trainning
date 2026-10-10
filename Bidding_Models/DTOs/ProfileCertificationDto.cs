using System;
using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class ProfileCertificationRequestDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "profile Id is required")]
        public int ProfileId { get; set; }
        
        [Required(ErrorMessage = "certification Id is required")]
        public string CertificateId { get; set; }
        [Required(ErrorMessage = "certificate Name is required")]
        public string CertificateName { get; set; }
        
        [Required(ErrorMessage = "Provider is required")]
        public string Provider { get; set; }
        
        [Required(ErrorMessage = "Certificate Url is required")]
        public string CertificateUrl { get; set; }
        
        [Required(ErrorMessage = "Date Issued is required")]
        public DateTime DateIssued { get; set; }
        
        public DateTime? DateExpired { get; set; }
        public string FileName { get; set; }
    }
   
}
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel
{
    public class ProfileCertificate
    {
        public int Id { get; set; }

        [NotMapped]
        public int Slno { get; set; }
        public Profile Profile { get; set; }
        public int? ProfileId { get; set; }
        
        public string CertificateId { get; set; }
        
        public string CertificateName { get; set; }
        
        public string Provider { get; set; }
        
        public string CertificateUrl { get; set; }
        public string FileName { get; set; }
        public DateTime DateIssued { get; set; }
        public DateTime? DateExpired { get; set; }
    }
}
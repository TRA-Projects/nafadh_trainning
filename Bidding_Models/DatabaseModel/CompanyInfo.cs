using Bidding_Models.Integrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel
{
    public class CompanyInfo
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string CrNumber { get; set; }
        public string CompanyNameArabic { get; set; }
        public string CompanyNameEnglish { get; set; }
        public string CompanyNameWithLegalArabic { get; set; }
        public string CompanyNameWithLegalEnglish { get; set; }
        public RegistrationStatus CompanyStatus { get; set; } = new RegistrationStatus();
        public LegalStatus LegalStatus { get; set; } = new LegalStatus();
        public Location Location { get; set; } = new Location();
        public DateTime? RegistrationDate { get; set; }
        public string SubjectToForeignInvestment { get; set; }
        public string CompanyGrade { get; set; } = "Unknown";
        public DateTime? ExpiryDate { get; set; }
        public Capital Capital { get; set; } = new Capital();
        public List<DeclaredActivity> DeclaredActivities { get; set; }
        public List<CompanyInfoSignatory> Signatories { get; set; }


        [NotMapped]
        public bool IsExpired { 
            get 
            { 
                return ExpiryDate.HasValue && ExpiryDate.Value.Date < DateTime.Now.Date;
            } 
        }

    }
    public class CompanyInfoSignatory
    {
        public int Id { get; set; }
        public int CompanyInfoId { get; set; }
        public string PersonNameEn { get; set; }
        public string PersonNameAr { get; set; }
        public string CivilId { get; set; }
        public Designation Designation { get; set; }
        public Signaturetype SignatureType { get; set; }
        public DateTime? SignatoryStartDate { get; set; }
    }
    [Owned]
    public class Designation
    {
        public string code { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
    }
    [Owned]
    public class Signaturetype
    {
        public string code { get; set; }
        public string nameAr { get; set; }
        public string nameEn { get; set; }
    }
    
    [Owned]
    public class Location
    {
        public string Town { get; set; }
        public string Governorate { get; set; }
        public string State { get; set; }
    }
}

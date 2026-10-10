using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel
{
    public class ProfileEducation
    {
        public int Id { get; set; }

        [NotMapped]
        public int Slno { get; set; }
        public Profile Profile { get; set; }
        public int? ProfileId { get; set; }
        
        public string Degree { get; set; }
        
        public string InstituteName { get; set; }
        
        public string MajorSpecialization { get; set; }
        
        public string Country { get; set; }
        
        public string City { get; set; }
        
        public string YearOfPassing { get; set; }
        
        public string Grade { get; set; }
        public string FileName { get; set; }
      
    }
}
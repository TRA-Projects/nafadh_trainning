using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class Project
    {
        public int Id { get; set; }
        [RegularExpression(@"[a-zA-Z\s]*$", ErrorMessage = "Use English letters only please")]
        public string ProjectNameEn { get; set; }
        public string ProjectNameAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }

        [RegularExpression(@"[a-zA-Z\s]*$", ErrorMessage = "Use English letters only please")]
        public string ClientName { get; set; }
        public Profile Client { get; set; }
        [ForeignKey("Client")]
        public int? ClientId { get; set; }
        public TendorCategory BusinessSector { get; set; }
        [ForeignKey("BusinessSector")]
        public int? BusinessSectorId { get; set; }
        public string BusinessSectorName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfResources { get; set; }
        public string Location { get; set; }
        public DateTime CrreatedDate { get; set; } = DateTime.Now;
        public Profile Profile { get; set; }
        [ForeignKey("Profile")]
        public int ProfileId { get; set; }
        public bool IsActive { get; set; } = false;


    }
}

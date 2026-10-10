using Bidding_Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models
{
    public class DeclaredActivity
    {
        public int Id { get; set; }
        public string IsicCode { get; set; }
        public string IsicVersion { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string RegistrationDate { get; set; }
        public string SubjectToLicensing { get; set; }
        public string LicenseObtained { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsApproved { get; set; }

        public Profile Profile { get; set; }
        public int? ProfileId { get; set; }
        public int? CompanyInfoId { get; set; }
        [NotMapped]
        public string ActivityDivisionAr { get; set; }
        [NotMapped]

        public string ActivityDivisionEn { get; set; }

        [NotMapped]

        public string exstring { get; set; }
    }

    public class CompanyDeclaredActivities
    {
        public List<DeclaredActivity> Activity { get; set; }
    }
}

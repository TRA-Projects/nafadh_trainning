using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class Staff
    {
        public int Id { get; set; }
        [RegularExpression(@"[a-zA-Z\s]*$", ErrorMessage = "Use English letters only please")]
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        [RegularExpression(@"[a-zA-Z\s]*$", ErrorMessage = "Use English letters only please")]
        public string DesignationEn { get; set; }
        public string DesignationAr { get; set; }
        public int TotalExperience { get; set; }
        public bool IsOmani { get; set; }
        public string Picture { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }  
        [NotMapped]

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
         ErrorMessage = "Passerror")]
        public string Password { get; set; }

        [NotMapped]
        public List<DocumentWrapper> DocumentList { get; set; }

        

        public DateTime JoinDate { get; set; }
        [Remote(action: "IsEmailExistForStaff", controller: "Account", ErrorMessage = "IsEmailExist")]
        public string Email { get; set; }
        [Remote(action: "IsPhoneNumberExistForStaff", controller: "Account", ErrorMessage = "IsPhoneNumberExist")]
        public string Phone { get; set; }
        public DateTime CrreatedDate { get; set; } = DateTime.Now;
        public Profile Profile { get; set; }
        public int ProfileId { get; set; }

        public List<StaffSkill> StaffSkills { get; set; }
        public List<StaffCertificate> StaffCertificates { get; set; }
        public bool IsActive { get; set; } = false;

        public Department Department { get; set; }
        public int? DepartmentId { get; set; }

        public string StaffNumber { get; set; }
        [NotMapped]
        public bool IsOUser { get; set; }

        [NotMapped]
        public bool IsHead { get; set; }

        [NotMapped]
        public bool IsPostOpertunity { get; set; }

        [NotMapped]
        public List<SelectListItem> DepartmentsList { get; set; }

        [NotMapped]
        public List<SelectListItem> OperatorsList { get; set; }


        [NotMapped]
        public string ClientId { get; set; }



    }
}

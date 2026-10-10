using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Bidding_Models
{
    public class SMETrainingRegistration
    {
        [Required(ErrorMessage = "isRequired.")]
        [Remote(action: "IsCivilNoExist", controller: "Training", AdditionalFields = "IsAlreadyRegisterd", ErrorMessage = "IsRegistrationNumberExist")]
        public string CRNumber { get; set; }

     //   [RequiredIf(ErrorMessage = "isRequired.")]
        public string NameEn { get; set; }
        [Required(ErrorMessage = "isRequired.")]
        public string NameAr { get; set; }

        [Required(ErrorMessage = "isRequired.")]
        public string Confirmation { get; set; }

        [Remote(action: "IsEmailExist", controller: "Training", AdditionalFields = "IsAlreadyRegisterd", ErrorMessage = "IsPhoneNumberExist")]
        [Required(ErrorMessage = "isRequired.")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "isRequired.")]
        [Remote(action: "IsPhoneNumberExist", controller: "Training", AdditionalFields = "IsAlreadyRegisterd", ErrorMessage = "IsPhoneNumberExist")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "isRequired.")]
        public string Password { get; set; }

        public bool IsAlreadyRegisterd { get; set; }
    }
}

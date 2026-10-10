using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class AccountDetails
    {

        public int Id { get; set; }

        public Bank Bank { get; set; }
        public int? BankId { get; set; }

        //[RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Account Name should  contain only alphabets.")]
        public string AccountName { get; set; }

        //[RegularExpression(@"^\d+$", ErrorMessage = "Account Number should contain only numbers.")]
        public string AccountNumber { get; set; }

        public Profile Profile { get; set; }
        public int ProfileId { get; set; }

        [NotMapped]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Account Name should  contain only alphabets.")]
        public string AccountNameRegex { get; set; }
        [NotMapped]
        [RegularExpression(@"^\d+$", ErrorMessage = "Account Number should contain only numbers.")]
        public string AccountNumberRegex { get; set; }




    }
}

using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DatabaseModel
{
    public class FreelancerBankAccount
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public int ProfileId { get; set; }
        [Required]
        public string AccountHolderName { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public string SwiftCode { get; set; }
        public string IbanCode { get; set; }
        public Profile Profile { get; set; }
        public Bank Bank { get; set; }        
    }
}

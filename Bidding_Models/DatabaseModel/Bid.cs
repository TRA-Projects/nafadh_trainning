using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class Bid : EntityTrail
    {
        public Bid()
        {
            BidDocumentList = new List<BidDocument>();
            NotificationList = new List<Notification>();            
        }
        public int Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [StringLength(40)]
        public string EncryptedId { get; set; }
        public Request Request { get; set; }

        public int RequestId { get; set; }


        public AppUser Vendor { get; set; }
        public string VendorId { get; set; }
        public decimal BidBudget { get; set; }
        
        public string BidBudgetStr
        {
            get { return BidBudget == 0 ? "" : BidBudget.ToString("N"); }
            set { BidBudget = value == "" ? 0 : Convert.ToDecimal(value); }
        }

       
        public string Description { get; set; }
        public int Days { get; set; }

        public DateTime? AwardedDate { get; set; }

        public string? AwardedById { get; set; }
        public BidStatus Status  { get; set; }

        public List<Feedback> FeedbackList { get; set; }
        public List<Notification> NotificationList { get; set; }
        public List<BidDocument> BidDocumentList { get; set; }

        public List<BidQuotation> BidQuotationList { get; set; }

        public List<BidEvaluation> BidEvaluationList { get; set; }

        public List<ProposalClarification> ProposalClarificationList { get; set; }


        public int AwardedDays { get; set; }

        public string Currency { get; set; }

        public decimal AwardedBudget { get; set; }

        [NotMapped]
        public string AwardedBudgetStr
        {
            get { return AwardedBudget == 0 ? "" : AwardedBudget.ToString("N"); }
            set { AwardedBudget = value == "" ? 0 : Convert.ToDecimal(value); }
        }

        public bool ByAdmin { get; set; }
        public string TechnicalDoc { get; set; }
        public string FinancialDoc { get; set; }
       
        [RegularExpression(@"^.*\D+.*$", ErrorMessage = "Reason should not contain numbers only")]
        public string Reason { get; set; }

        [NotMapped]
        public IFormFile TechnicalDocFile { get; set; }

        [NotMapped]
        public IFormFile FinancialDocFile { get; set; }

        [NotMapped]
        public int BidDocumentCount { get; set; }

        [NotMapped]
        public List<DocumentWrapper> DocumentWrapperList { get; set; }

        [NotMapped]
        public PagedResult<Bid> PagedResult { get; set; }

        [NotMapped]
        public string VendorName { get; set; }

        [NotMapped]
        public string Action { get; set; }
        [NotMapped]
        public string Controller { get; set; }
        [NotMapped]
        public string Host { get; set; }
        [NotMapped]
        public string Protocol { get; set; }
        [NotMapped]
        public string SearchStatus { get; set; }
        [NotMapped]
        public string ByAdminSearch { get; set; }
        [NotMapped]
        public string RequestEncryptedId { get; set; }


        [NotMapped]
        public List<RequestRequiredDocument> RequestRequiredDocumentList { get; set; }

        [NotMapped]
        public string LoggedInUser { get; set; }
        //public string BidQuotationString { get; set; }

    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml.Linq;

namespace Bidding_Models.DatabaseModel
{
  public  class Request : EntityTrail
    {
        public Request()
        {
            RequestDocumentList = new List<RequestDocument>();
            NotificationList = new List<Notification>();
        }

        public int Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [StringLength(40)]
        public string EncryptedId { get; set; }

        public virtual string Name { get; set; }
        public TendorCategory TendorCategory { get; set; }
        public int? TendorCategoryId { get; set; }

        [NotMapped]
        public string TendorCategoryName { get; set; }
        public AppUser User { get; set; }
        public string UserId { get; set; }
        public int? RequestType { get; set; }
        public int? OpportunityType { get; set; }
        public AppUser Vendor { get; set; }
        public string VendorId { get; set; }
        //public int GradeofVendor { get; set; }
        public virtual string Description { get; set; }
        public int? Budget { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? Lastsubmissiondate { get; set; }
        public DateTime? LastClarificationDate { get; set; }

        public DateTime? AwardedDate { get; set; }
        public Governorate Governorate { get; set; }
        public int? GovernorateId { get; set; }
        public Wilayat Wilayat { get; set; }
        public int? WilayatId { get; set; }
        public City City { get; set; }
        public int? CityId { get; set; }
        public string ServiceCategory { get; set; }
        public string Lang { get; set; }
        public string Code { get; set; }

        public bool IsCodeManual { get; set; }

        public string CodePrefix { get; set; }
        public string Lat { get; set; }
        public string OmantelLink { get; set; }

        //0 for Created , 1 for Approved by Admin , 2 for awarded to vendor
        public RequestStatus? Status { get; set; }
        public int? MainCategoryId { get; set; }
        public bool IsExpiryExtendedByClient { get; set; }
        public string ExpiryExtendedReason { get; set; }
        public string JustificationReason { get; set; }
        public string CanceledReason { get; set; }

        public string AdminRejectReason { get; set; }

        public string DeleteReason { get; set; }
        public bool ByAdmin { get; set; }

        public bool PermissiontoOneUser { get; set; }
     
        public bool IsNDARequired { get; set; }

        public bool IsProposalRequired { get; set; }
        public List<RequestQuestion> RequestQuestionList { get; set; }
    
        public List<RequestDocument> RequestDocumentList { get; set; }
        public List<Notification> NotificationList { get; set; }
        public List<Bid> BidList { get; set; }
        public Bidding Bidding { get; set; }

        public List<NDA> NDAList { get; set; }
        public List<VendorSavedRequest> VendorSavedRequestList { get; set; }
        public List<RequestEvaluationCriteria> RequestEvaluationCriteriaList { get; set; }
        public List<RequestRequiredDocument> RequestRequiredDocumentList { get; set; }

        public List<RequestNDADocument> RequestNDADocumentList { get; set; }
        
        public List<RequestPrerequiste> RequestPrerequisteList { get; set; }
        public List<RequestClarificationQuestion> RequestClarificationQuestionList { get; set; }

        public List<RequestParticipents> RequestParticipentsList { get; set; }
        
        [NotMapped]
        public List<DocumentWrapper> DocumentWrapperList { get; set; }
        [NotMapped]
        public PagedResult<Request> PagedResult { get; set; }
        [NotMapped]
        public int MainCat { get; set; }

        [NotMapped]
        public string EncryptedMainCategoryId { get; set; }

        [NotMapped]
        public string EncryptedTendorCategoryId { get; set; }

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
        public string CodeValue { get {
                if (IsCodeManual)
                {
                    return Code;
                }
                else
                {
                    return CodePrefix + Code;
                }
               
            } }


        [NotMapped]
        public string LocationStr
        {
            get
            {
                string Location = String.Empty;
                if (GovernorateId == null && CityId == null)
                {
                    Location = "Not Specified";
                }
                else
                {
                

                    if (!String.IsNullOrEmpty(Governorate?.Name) && !String.IsNullOrEmpty(Wilayat?.Name))
                    {
                        Location = Governorate?.Name + " , " + Wilayat?.Name;
                    }
                    else if (!String.IsNullOrEmpty(Governorate?.Name))
                    {
                        Location = Governorate?.Name;
                    }
                }
                return Location;
            }
        }


        [NotMapped]
        public string RequestStatusStr
        {
            get
            {
                string title = String.Empty;
                if (Status!=null)
                {
                    if (Status == RequestStatus.Draft)
                    {

                        title = "Draft";
                    }
                    if (Status == RequestStatus.Created)
                    {

                        title = "Pending Approval";
                    }

                    else if (Status == RequestStatus.Expired)
                    {

                        title = "Expired";
                    }
                    else if (Status == RequestStatus.Closed) // || model.ExpiryDate < DateTime.Today
                    {
                        title = "Closed";
                    }
                    else if (Status == RequestStatus.Canceled)
                    {
                        if (!String.IsNullOrEmpty(CanceledReason))
                        {
                            title = "Canceled" + " - " + CanceledReason;
                        }
                        else
                        {
                            title = "Canceled";
                        }

                    }
                    else if (Status == RequestStatus.Awarded)
                    {

                        title = "Awarded";
                    }
                    else if (Status == RequestStatus.AdminRejected)
                    {
                        if (!String.IsNullOrEmpty(AdminRejectReason))
                        {
                            title = "Rejected" + " - " + AdminRejectReason;
                        }
                        else
                        {
                            title = "Rejected";
                        }

                    }
                    else if (Status == RequestStatus.AdminApproved)
                    {
                        if (ExpiryDate < DateTime.Now)
                            title = "Expired";
                        else
                        {
                            if (Lastsubmissiondate <= DateTime.Now)
                                title = "Evaluting";
                            else
                                title = "Active";
                        }
                    }
                    //else if (Status == RequestStatus.AdminApproved && (Lastsubmissiondate <= DateTime.Now && ExpiryDate >= DateTime.Now))
                    //{

                    //    title = "Evaluting";
                    //}

                    //else if (Status == RequestStatus.AdminApproved && Lastsubmissiondate > DateTime.Now) // && (model.Lastsubmissiondate > currentTime)
                    //{

                    //    title = "Active";
                    //}
                    else if (Status == RequestStatus.Deleted) // && (model.Lastsubmissiondate > currentTime)
                    {
                        if (!String.IsNullOrEmpty(DeleteReason))
                        {
                            title = "Deleted" + " - " + DeleteReason;
                        }
                        else
                        {
                            title = "Deleted";
                        }

                    }
                }
            
                return title;

            }
        }
        public string ProjectManagers { get; set; }     
        public string ClarificationAnswering { get; set; }

        public string cancellation { get; set; }
        
        public string Extension { get; set; }
        
        public string TechnicalEvaluation { get; set; }
      
        public string FinancialEvaluation { get; set; }
  
    
      
        public string Reviewers { get; set; }
      
        public string Awarding { get; set; }

        public string AllPermissions { get; set; }
        public string Approvers { get; set; }
        public string CloseVendors { get; set; }

        public string ForProfileTypes { get; set; }

        public bool IsPublished { get; set; }

        public bool IsPublicAcceptanceCriteria{ get; set; }
        public List<RequestViewHistory> RequestViewHistoryList { get; set; }

        public List<VendorExtensionRequest> VendorExtensionRequestList { get; set; }
        public int? DirectVendor { get; set; }
        [NotMapped]
        public List<SelectListItem> VendorsLst { get; set; }
       
        [NotMapped]
        public List<SelectListItem> Users { get; set; }
        [NotMapped]
        public List<string> SelectedCloseVendors { get; set; }
        [NotMapped]
        public List<string> SelectedReviewers { get; set; }
        [NotMapped]
        public List<string> SelectedAllPermissions { get; set; }
        
        [NotMapped]
        public List<string> SelectedProjectManagers { get; set; }
        [NotMapped]
        public List<string> SelectedClarificationAnswersUsers { get; set; }
        [NotMapped]
        public List<string> SelectedTechnicalEvaluationUsers { get; set; }
        [NotMapped]
        public List<string> SelectedFinancialEvaluationUsers { get; set; }
        [NotMapped]
        public List<string> SelectedAwardingUsers { get; set; }
        [NotMapped]
        public List<string> SelectedCanceledUsers { get; set; }
        [NotMapped]
        public List<string> SelectedExtensionUsers { get; set; }
        [NotMapped]
        public List<string> SelectedApprovers { get; set; }

        [NotMapped]
        public string LoggedInUserId { get; set; }
        public bool IsDeleted { get; set; }

      [NotMapped]
        public int ViewCount { get; set; }

        [NotMapped]
        public int DownloadCount { get; set; }

        [NotMapped]
        public int RequestParticipentsCount { get; set; }

        [NotMapped]
        public List<AppUser> CloseVendorsList { get; set; }

        [NotMapped]

        public List<AppUser> RequestTotalCount { get; set; }

        [NotMapped]

        public List<Request> RequestsByVendorCategories { get; set; }

        [NotMapped]
        public string Reason { get; set; }
    }


    public class OmantelRequest 
    {
        [NotMapped]
        public int OpertunityId { get; set; }

        public string OmantelLink { get; set; }
        public virtual string Name { get; set; }
        public string Code { get; set; }


        [NotMapped]
        public string UserId { get; set; }
        public virtual string Description { get; set; }
        public int? Budget { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? Lastsubmissiondate { get; set; }
        public DateTime? LastClarificationDate { get; set; }
        public string Link { get; set; }
        public RequestStatus Status { get; set; }

        public string VendorCrNumber { get; set; }

    }
}

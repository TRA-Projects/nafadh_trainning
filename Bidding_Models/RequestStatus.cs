using System.ComponentModel.DataAnnotations;

namespace Bidding_Models
{
    public enum RequestStatus
    {
        Draft = -1,
        Created =0,
        AdminRejected,
        AdminApproved,
        Expired,
        BiddingApproval,
        biddingRejectedByAdmin,
        BiddingApprovedByAdmin,
        BiddingExpired,
        Awarded,
        Completed,
        Closed,
        Pending,
        Extended,
        Canceled,
        Deleted

    }

    public enum RequestStatusForSearch
    {
        Drafted = -1,
        Created = 1,
        Approved,
        Rejected,
        Active,
        Evaluating,
        Awarded,
        Expired,
        Closed,
        Canceled

    }

    public enum RequestStatusForReportSearch
    {
        [Display(Name = "Pending Approval")]
        Pending_Approval = 1,
        Approved,
        Rejected,
        Active,
        Evaluating,
        [Display(Name = "All Awarded")]
        Awarded,
        [Display(Name = "Direct Awarded")]
        Direct_Awarded,
        [Display(Name = "System Awarded")]
        System_Awarded,
        Expired,
        Closed,
        Canceled,
        Drafted,
        Deleted
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
    public enum BidStatus
    {
        Send,
        TechnicalRejected,
        TechnicalAwarded,
        FinancialAwarded,
        Awarded,
        Completed,
        Closed,
        UserAwarded,
        VendorRejected,
        VendorAwarded,
        VendorCompleted,
        UserCompleted,
        Submitted,
        Evaluating,
        Rejected,
        Not_Awarded
    }
    public enum BidStatusForShow
    {
        Send =0,
        TechnicalRejected = 1,
        Evaluating = 10,
        UserAwarded = 7,
        Rejected = 14,
     
    }
}

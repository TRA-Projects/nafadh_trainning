using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
    public enum NotificationTypesEnum
    {
        VendorRegistration=1,
        RequirmentPosted,
        RequirmentUpdate,
        BidPlaced,
        BidUpdate,
        BidRejected,
       
        BidAccepted,
        ProjectCompleted,
        ProjectFinish,
        RequirmentAccepted,
        RequirmentRejected,
        ClientRegistration,
        SendOTP,
        ForgotPassword,
        ContactUs,
        VendorReminderForRequestExpiry,
        ClientReminderForRequestExpiry,
        ExtendedExpiryDate,
        RequirmentExpired,
        ProjectAccepted,
        FeedbackforVendor,
        FeedbackforUser,
        CancedledRequest,
        AskQuestion,
        AnswerSubmitted,
        TechnicalReject,
        TechnicalApprove,
        FinancialReject,
        FinancialApprove,
        EmailVerification,
        MissingDocuments,
        AddedSubCategory,
        AssignPermission,
        RemovePermission,
        OpratorUserRegistration,
        ExtentionRequestByVendor,
        PostProposalClarification,
        PostProposalClarificationAnswer,
        RequestAwarded,
        TrainingRegistration,
        BeforeTrainingRegistration,
        TwoWeeksBeforetheClosedDate,
        OneWeeksBeforetheClosedDate,
        ThreeDaysBeforetheClosedDate,
        DeleteUser,
        ReminderEmailtoVendorsBeforeLastSubmissionDate,
        UpdateProfile,
        ApprovedProfile,
        RejectedProfile,
        ResetPasswordNotification
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
    public class Enums
    {
        
        public enum ProfileTypes
        {
            Admin = 0,
            FreeLancer = 1,
            SME = 2,
            Contractor = 3,
            Operator = 4,
            OUser = 5, //Users under Operator Profile
            Staff = 6, //Users under SME,Freelancer,Contractor,
            Non_SME = 7, //Same as SME but not register in Riyada
            JobSeeker = 8,
        }
        public enum OpportunityTypes
        {
            Open = 1,
            Close = 2,
            Direct = 3
        }
        public enum RequestTypes
        {
            
            RFQ = 1,
            RFI = 2,
            RFP = 3,
            RFT = 4
        }

        public enum ProfileStatus
        {

            UnVerified = 0,
            InApprovalProcess,
            Approved,
            Rejected
        }
        public enum ProjectStatus
        {
            Pending =0,
            Active = 1,
            Evaluation = 2,
            Awarded = 3,
            Draft = 4,
            Completed = 5
        }

        public enum  PaymentStatus
        {
            None = -1,
            Pending = 0,
            AwaitingApproval = 1,
            Paid = 2,
        }

        public enum ContractStatus
        {
            PendingSign =0,
            Signed = 1,
            Rejected = 2,
            RejectedForRevision = 3,
            RevisionSubmitted =4,
            Draft = 6
        }
        public enum EngagementStatus
        {
            PendingEvaluation = 0,
            PendingAmendEvaluation = 1,
            Shortlisted = 2,
            Approved = 3,
            Rejected = 4,
            RejectForAmendment = 5,
            Awarded = 6,
        }
       

        public enum ProjectType
        {
            Open,
            Direct
        }

        public enum ExperienceLevel
        {
            Intern,
            Entry,
            Intermediate,
            Experienced
        }

        public enum ProjectVisibility
        {
            LandingPage,
            AfterLogin
        }
        
        public enum EngagementType
        {
            Bid,
            Application
        }
        public enum ProjectDurationEstimation
        {
            PhaseAndMilestone,
            RequestForDetails,
        }

        public enum UserConnectivityStatus
        {
            Online,
            Offline,
        }

        public enum MilestoneStatus
        {
            PendingStart,
            InProgress, 
            PendingReview, 
            Approved,
            Rejected, 
            Canceled,
            Paid,
      
        }

        public enum DisputeStatus
        {
            PendingResolve,
            Resolved,
            Rejected,
        }

        public enum AttachmentType
        {
            Attachment,
            AttachmentUrl,
            RejectAttachment,
            
        }
    }
}

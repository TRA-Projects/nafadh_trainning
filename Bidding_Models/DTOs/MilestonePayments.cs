using Bidding_Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class MilestoneOperatorViewDto
    {
        public decimal? TotalPaid { get; set; }
        public decimal? PendingApproval { get; set; }
        public decimal? PaidLastMonth { get; set; }
        public decimal? UpcomingPayments { get; set; }
        public int Disputes { get; set; }
        public int DisputeResolved { get; set; }
        public List<MilestonePaymentDto> MilestonePayments { get; set; }
        public List<MilestoneItemDto> MilestoneData { get; set; }
        public BankAccountDto[] BankAccounts { get; set; }
       
    }
    
    public class MilestonePaymentDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string MilestoneName { get; set; }
        public string FreelancerName { get; set; }
        public int FreelancerId { get; set; }
        public DateTime DueDate { get; set; }
        public decimal? Amount { get; set; }
        public Enums.MilestoneStatus Status { get; set; }
        public string StatusString { get; set; }
        public string Remarks { get; set; }
       
    }

     
    public class PaymentRequestDto
    {
        public int Id { get; set; }
        public int MilistoneId { get; set; }
 
        public string OrderId { get; set; }
        public float Amount { get; set; }
        public int ProfileId { get; set; }
     
        public string TransactionDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public string BankTrackId { get; set; }
        public string BankRefNumber { get; set; }        
        public float MerchantAmount { get; set; }
    
        public string Currency { get; set; }
        public string CardName { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string FailureMessage { get; set; }
        public string CollectionReq { get; set; }
  
        public string Code { get; set; }
       
        public string OrderStatus { get; set; }
       
        public string Payment_mode { get; set; }


    }
    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public int MilistoneId { get; set; }
        public string MilestoneName { get; set; }
        public string OrderId { get; set; }
        public float Amount { get; set; }
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string TransactionDate { get; set; }
        public DateTime CreatedOn { get; set; }


    }

    public class MilestoneItemDto
    {
       
        public string FreelancerName { get; set; }
        public int FreelancerId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public int ContractId { get; set; }
        public string ProjectStatus { get; set; }
        public DateTime Dateline { get; set; }
        public int MilestoneCount { get; set; }
    }
    public class BankAccountDto
    {
        public int Id { get; set; }
        public string AccountHolderName { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchName { get; set; }
        public string Iban { get; set; }
        public string SwiftCode { get; set; }
        public string AccountType { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string ProfileType { get; set; }
    }

    public class WireframeRequestDto
    {
        
    }
    public class WireframeResponseDto
    {
        
    }
    //Request Revision
    public class RevisionRequestDto
    {
        public int Id { get; set; }
        public string Request { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
    // response Revision
    public class RevisionResponseDto
    {
        public int Id { get; set; }
        public string Request { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class MilestoneFreelancerViewDto
    {
        public decimal? TotalPaid { get; set; }
        public decimal? PendingApproval { get; set; }
        public int Disputes { get; set; }
        public int DisputeResolved { get; set; }
        public List<MilestonePaymentDto> MilestonePayments { get; set; }
        public List<MilestoneItemDto> MilestoneData { get; set; }
        public BankAccountDto[] BankAccounts { get; set; }
        public decimal? PaidLastMonth { get; set; }
        public decimal? UpcomingPayments { get; set; }
    }

    public class MilestonePaymentFreelancerDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string MilestoneName { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }

    public class MilestoneItemFreelancerDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int ProjectId { get; set; }
        public int PhaseId { get; set; }
        public string Status { get; set; }
      
        public DateTime DueDate { get; set; }
        public decimal? Amount { get; set; }
        public string MilestoneName { get; set; }
        public string MilestoneDescription { get; set; }
        public string FreelancerComment { get; set; }
        public string OpratoreCommint { get; set; }
        public string Reason { get; set; }
        public string Remark { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public string[] AttachmentUrls { get; set; }
        
        public string[] Attachments { get; set; }
        public string[] RejectAttachments { get; set; }

    }
    public class MilestoneItemFreelancerRequestDto
    {
      
        public string[] Attachments { get; set; }
        public string[] AttachmentUrls { get; set; }
        public string Remarks { get;set; }
        public string AdditionalComments { get; set; }
        
    }

    public class MilestoneItemOperatorRequestDto
    {
        public string AdditionalComments { get; set; }
        public Enums.MilestoneStatus Status { get; set; }
        public string Remarks { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }
        public string[] Attachments { get; set; }
        public string Resolution { get; set; }
    }
    public class MilestoneDisputeRequestDto
    {
        public string AdditionalComments { get; set; }
        public Enums.DisputeStatus Status { get; set; }
        public string Remarks { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }
        public string[] Attachments { get; set; }
        public string Resolution { get; set; }
    }
    public class FreelancerMilestoneItemsListDto
    {
        public int ProjectId { get; set; }
        public int ContractId { get; set; }
        public string ContractStatus { get; set; }
        public string ProjectTitle { get; set; }
        public string ContractCode { get; set; }
     public   MilestoneItemFreelancerDto[] MilestoneItems { get; set; }
   
    }

    public class DiscrepancyRequestFreelacerDto
    {
        
    }
    public class PhaseResponseDto
    {
        public int Id { get; set; }
        public string PhaseName { get; set; }
        public List<MilestoneItemFreelancerDto> MilestoneItems { get; set; }
     
    }
    namespace Bidding_Models.DTOs
    {
        public class DisputeDto
        {
            public int Id { get; set; }

            public int MilestoneId { get; set; }
            public int ContractId { get; set; }
            public string Description { get; set; }
            public string FreelancerName { get; set; }
            public string ProjectTitle { get; set; }
            public string MilestoneTitle { get; set; }
            public List<MilestoneAttachment> Attachments { get; set; }
            public List<string> ResponseAttachments { get; set; }
            public string Status { get; set; }
            public string Resolution { get; set; }
            public string CreatedBy { get; set; }
            public int FreelancerId { get; set; }
            public int OperatorProfileId { get; set; }
            public string Reason { get; set; }
            public string CreatedAt { get; set; }
            public string ResponseComment { get; set; }
            public string ResponseSuggestedResolution { get; set; }
        }

        public class DisputeResponseDto
        {
            public string ResponseComment { get; set; }
            public string ResponseSuggestedResolution { get; set; }
            public List<string> Attachments { get; set; }
        }
    }
}
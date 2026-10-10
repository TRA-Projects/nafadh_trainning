using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding_Models
{
    
    public class AmwalPayResponse
    {
        public string SystemTraceNr { get; set; }
        public string Message { get; set; }
        public int SchemeId { get; set; }
        public string TransactionId { get; set; }
        public bool IsOtpRequired { get; set; }
        public HostResponseData HostResponseData { get; set; }
        public int TerminalId { get; set; }
        public int TransactionTypeId { get; set; }
        public string TransactionTypeDisplayName { get; set; }
        public int MerchantId { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        public int CurrencyId { get; set; }
        public string MerchantName { get; set; }
        public string TransactionTime { get; set; }
        public string CustomerId { get; set; }
        public string CustomerTokenId { get; set; }
        public string SecureHashValue { get; set; }
        public string ResponseCode { get; set; }
        public string MerchantReference { get; set; }
        public string GatewayTransactionReference { get; set; }
        public string Emv { get; set; }
        public string ProcessorInformationTransactionId { get; set; }
        public string PaymentId { get; set; }
        public string Rrn { get; set; }
        public string TrackId { get; set; }
        public string Result { get; set; }
        public string Reason { get; set; }
        public bool IsSuccess { get; set; }
        public string DomainUrl { get; set; }
        public int RequestSource { get; set; }
        public string OriginalTransactionGuidId { get; set; }
        public string ReturnUrl { get; set; }
        public string ApplePayContactInfo { get; set; }
    }

    public class HostResponseData
    {
        public string TransactionId { get; set; }
        public string Rrn { get; set; }
        public string TrackId { get; set; }
        public string PaymentId { get; set; }
        public string Auth { get; set; }
    }
}

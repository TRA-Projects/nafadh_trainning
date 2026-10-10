using System;

namespace Bidding_API.ExceptionHandler
{
    public sealed class SupportTicketInfo
    {
        public string TicketId { get; set; }
        public DateTime OccurredAtUtc { get; set; }
        public string LogFilePath { get; set; }
        public byte[] LogBytes { get; set; }
        public string UserMessage { get; set; }
    }

    public sealed class ClientErrorReportRequest
    {
        public string TicketId { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
        public string Url { get; set; }
        public string UserAgent { get; set; }
        public string Locale { get; set; }
        public string Extra { get; set; }
    }
}

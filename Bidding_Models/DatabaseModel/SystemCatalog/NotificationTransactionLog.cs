using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.SystemCatalog
{
    /// <summary>Outbound email/SMS transaction log for search and audit.</summary>
    [Table("NotificationTransactionLogs")]
    public class NotificationTransactionLog
    {
        public long Id { get; set; }

        /// <summary><c>Email</c> or <c>SMS</c>.</summary>
        [Required]
        [MaxLength(16)]
        public string ChannelType { get; set; }

        /// <summary>Email address or mobile number actually used.</summary>
        [Required]
        [MaxLength(256)]
        public string Receiver { get; set; }

        [MaxLength(512)]
        public string Subject { get; set; }

        public string MessageBody { get; set; }

        /// <summary>Locale used when rendering the template: <c>en</c> or <c>ar</c>.</summary>
        [MaxLength(8)]
        public string PreferredLanguage { get; set; }

        /// <summary>Template action code when applicable (e.g. EVALUATION_INVITATION).</summary>
        [MaxLength(64)]
        public string ActionCode { get; set; }

        public int? ProfileId { get; set; }

        /// <summary>Set when the row was produced by an admin-authored custom notification.</summary>
        public long? CustomNotificationId { get; set; }

        public bool IsSuccess { get; set; }

        [MaxLength(1024)]
        public string FailureReason { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}

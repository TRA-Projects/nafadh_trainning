using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.Notifications
{
    /// <summary>In-app inbox row for a system notification, rendered in the recipient's preferred language.</summary>
    [Table("UserNotifications")]
    public class UserNotification
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }

        public long? CustomNotificationId { get; set; }

        [ForeignKey(nameof(CustomNotificationId))]
        public CustomNotification CustomNotification { get; set; }

        [Required]
        [MaxLength(8)]
        public string Locale { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}

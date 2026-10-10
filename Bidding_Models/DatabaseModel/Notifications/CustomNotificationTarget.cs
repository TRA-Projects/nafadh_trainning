using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.Notifications
{
    [Table("CustomNotificationTargets")]
    public class CustomNotificationTarget
    {
        public long Id { get; set; }

        public long CustomNotificationId { get; set; }

        [ForeignKey(nameof(CustomNotificationId))]
        public CustomNotification CustomNotification { get; set; }

        /// <summary>See <c>CustomNotificationTargetTypes</c>.</summary>
        [Required]
        [MaxLength(32)]
        public string TargetType { get; set; }

        [MaxLength(450)]
        public string UserId { get; set; }

        public int? UserGroupId { get; set; }

        public int? OrganizationId { get; set; }

        /// <summary>Display label captured at save time (user/group/organization name).</summary>
        [MaxLength(256)]
        public string Label { get; set; }

        /// <summary>See <c>CustomNotificationRegistrationFilters</c>; used by JobSeekers / AllOrganizations targets.</summary>
        [MaxLength(16)]
        public string RegistrationFilter { get; set; }
    }
}

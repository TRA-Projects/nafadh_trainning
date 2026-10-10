using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.SystemCatalog
{
    /// <summary>Field-level system audit trail (System Administration).</summary>
    [Table("SystemAuditTrails")]
    public class SystemAuditTrail
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Module { get; set; }

        [Required]
        [MaxLength(128)]
        public string EntityType { get; set; }

        [Required]
        [MaxLength(128)]
        public string EntityReference { get; set; }

        [Required]
        [MaxLength(128)]
        public string Field { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        /// <summary>Create | Update | Delete</summary>
        [Required]
        [MaxLength(32)]
        public string Action { get; set; }

        /// <summary>UI | Scheduler</summary>
        [Required]
        [MaxLength(32)]
        public string Source { get; set; }

        /// <summary>Logged-in user id (JWT UserId claim).</summary>
        [MaxLength(64)]
        public string UpdatedByUserId { get; set; }

        public DateTime UpdatedDate { get; set; }
    }

    public static class SystemAuditActions
    {
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
    }

    public static class SystemAuditSources
    {
        public const string Ui = "UI";
        public const string Scheduler = "Scheduler";
    }

    public static class SystemAuditModules
    {
        public const string InterviewManagement = "InterviewManagement";
        public const string IdentityAccess = "IdentityAccess";
    }

    public static class SystemAuditEntityTypes
    {
        public const string InterviewRecord = "InterviewRecord";
    }
}

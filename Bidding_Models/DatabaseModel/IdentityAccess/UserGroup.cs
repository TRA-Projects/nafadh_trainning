using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.IdentityAccess
{
    [Table("UserGroups")]
    public class UserGroup
    {
        public int Id { get; set; }

        public int UserTypeId { get; set; }

        [ForeignKey(nameof(UserTypeId))]
        public UserType UserType { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [Required]
        [MaxLength(100)]
        public string NameEn { get; set; }

        [Required]
        [MaxLength(100)]
        public string NameAr { get; set; }

        public bool IsActive { get; set; } = true;

        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

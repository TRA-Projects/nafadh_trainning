using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.IdentityAccess
{
    [Table("AspNetRolePages")]
    public class RolePage
    {
        public int Id { get; set; }

        public string RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public global::Bidding_Models.DatabaseModel.Roles Role { get; set; }

        public int AppPageId { get; set; }

        [ForeignKey(nameof(AppPageId))]
        public AppPage AppPage { get; set; }

        public int ActionFlags { get; set; } = PageActionFlags.View;

        [System.ComponentModel.DataAnnotations.MaxLength(32)]
        public string DataScope { get; set; } = PageDataScopes.Mine;
    }
}

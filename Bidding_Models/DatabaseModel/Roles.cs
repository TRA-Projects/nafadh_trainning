using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class Roles : IdentityRole
    {
        public int? UserGroupId { get; set; }

        [ForeignKey(nameof(UserGroupId))]
        public IdentityAccess.UserGroup UserGroup { get; set; }

        public int? OrganizationId { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public IdentityAccess.Organization Organization { get; set; }

        /// <summary>Template/group role parent; custom org roles point at their template (or parent custom role).</summary>
        public string ParentRoleId { get; set; }

        [ForeignKey(nameof(ParentRoleId))]
        public Roles ParentRole { get; set; }

        public bool IsTemplate { get; set; }

        public bool IsActive { get; set; } = true;

        public List<AppUserRoles> AppUserRolesList { get; set; }

        public List<RolesClaims> RolesClaimsList { get; set; }

        public List<IdentityAccess.RolePage> RolePagesList { get; set; }

        [NotMapped]
        public IList<Claim> RoleClaims{ get; set; }
    }
}

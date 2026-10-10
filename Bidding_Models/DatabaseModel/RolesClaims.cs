using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
  public  class RolesClaims : IdentityRoleClaim<string>
    {
        public Roles Role { get; set; }

        public ClaimMaster ClaimMaster { get; set; }
        public int ClaimMasterId { get; set; }

        [NotMapped]
        public override string ClaimType { get ; set ; }
        [NotMapped]
        public override string ClaimValue { get; set; }
    }
}

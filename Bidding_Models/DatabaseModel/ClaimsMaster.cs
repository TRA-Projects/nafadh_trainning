using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class ClaimMaster
    {
        public int Id { get; set; }
        public string ClaimName { get; set; }

        public string NameEn { get; set; }

        public string NameAr { get; set; }

        public bool IsActive { get; set; } = true;

        public List<RolesClaims> RolesClaimsList { get; set; }
    }
}

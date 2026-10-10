using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class AppUserRoles : IdentityUserRole<string>
    {
        public Roles Role { get; set; }
        public AppUser User { get; set; }

    }
}

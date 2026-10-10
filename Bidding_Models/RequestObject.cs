using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
   public class RequestObject<T>
    {
        public int EntityId { get; set; }

        public string EncryptedEntityId { get; set; }
        public string UserId { get; set; }
        public RolesEnum UserRole { get; set; }
        public string RoleName { get; set; }

        public int ProfileId { get; set; }
        public int ParentProfileId { get; set; }

        public int ProfileType { get; set; }
        public T Object { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class RequestUsersPermission
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public string Permissions { get; set; }
    }
}

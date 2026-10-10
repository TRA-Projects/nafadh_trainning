using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding_Models.DatabaseModel
{
    public class UserRefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpirey { get; set; }
        public DateTime TokenRenewDate { get; set; }
        public AppUser User { get; set; }
        public string UserId { get; set; }

        [NotMapped]
        public DateTime TokenExpiryDate { get; set; }
    }
}

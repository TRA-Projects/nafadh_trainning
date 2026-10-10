using Bidding_Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
  public  class LoginResponse
    {
        public AppUser User { get; set; }
        public string _token { get; set; }
        public DateTime TokenExpiry { get; set; }
        public string _refreshToken { get; set; }
    }
}

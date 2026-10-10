using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.Application_Models
{
  public   class UserDetailForChat
    {
        public  string ConnectionId { get; set; }
        public  string UserID { get; set; }
        public  string UserName { get; set; }

        public bool IsConnected { get; set; }
    }


    public  class UserDetailListForChat
    {
         public UserDetailListForChat()
        {
            ConnectedUsers = new List<UserDetailForChat>();
        }
        public static List<UserDetailForChat> ConnectedUsers { get; set; }
   
    }
}

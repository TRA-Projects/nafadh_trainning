using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.Application_Models
{
   public class MessageDetailForChat
    {
        public string FromUserID { get; set; }
        public string FromUserName { get; set; }
        public string ToUserID { get; set; }
        public string ToUserName { get; set; }
        public string Message { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
   public class ContactUsMessage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}

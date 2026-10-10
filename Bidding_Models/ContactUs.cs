using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
   public class ContactUs
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

        public string To
        {
            get
            {
                return "suggestions@constructionhub.om";
            }
        }
    }
}

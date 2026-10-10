using Bidding_Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
    public class SuggestedRequests
    {
        public int Count { get; set; }
        public int? GroupBy { get; set; }
        public List<Request> RequestsList { get; set; }

        public Request Requests { get; set; }
    }
}

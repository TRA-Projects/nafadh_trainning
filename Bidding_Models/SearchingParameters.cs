using System;
using System.Collections.Generic;

namespace Bidding_Models
{
    public class SearchingParameters
    {
        public bool IsText { get; set; }

        public string Datatype { get; set; }
        public string DisplayName { get; set; }
        public IDictionary<string, string> SearchingParams { get; set; }

        //public IDictionary<string, string> Values { get; set; }


        public List<Tuple<string,string>> Values { get; set; }

        public bool issort { get; set; }

        public string sort { get; set; }
        public bool IsMultiple { get; set; } = false;
        public bool ForceRemoveAny { get; set; } = false;
    }
}

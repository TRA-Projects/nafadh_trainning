using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class OpportunityDocumentType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Code { get; set; }

        public List<RequestDocument> RequestDocumentList { get; set; }

    }
}

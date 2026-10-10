using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class ProposalClarificationDocuments
    {
        public int Id { get; set; }
        public string DocName { get; set; }

        public ProposalClarification ProposalClarification { get; set; }
        public int ProposalClarificationId { get; set; }
    }
}

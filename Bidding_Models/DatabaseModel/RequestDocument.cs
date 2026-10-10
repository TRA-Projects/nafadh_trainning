using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class RequestDocument
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string DocName { get; set; }
        public Request Request { get; set; }
        public int RequestId { get; set; }

        public string FileType { get; set; }
        public string Description { get; set; }

        public int? OpportunityDocumentTypeId { get; set; }
        public  OpportunityDocumentType OpportunityDocumentType { get; set; }

        [NotMapped]
        public bool IsDeleted { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class RequestRequiredDocument
    {
        public int Id { get; set; }

  
        public string DocumentName { get; set; }
      
        public string DocumentType { get; set; }
        public int RequestId { get; set; }
        public Request Request { get; set; }

        public DocumentType DocumentTypes { get; set; }
        public int? DocumentTypesId { get; set; }
        public List<BidDocument> BidDocumentList { get; set; }

        [NotMapped]
        public IFormFile Uplodedfile { get; set; }
        [NotMapped]
        public DocumentWrapper DocumentWrapper { get; set; }

        
    }
}

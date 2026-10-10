using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
   public class DocumentWrapper
    {
        
        public int RequestDocumentId { get; set; }
        public byte[] filebytes { get; set; }
        public string filename { get; set; }

        public string name { get; set; }

        public string Filetype { get; set; }

        public int RequestId { get; set; }

        public string DocName { get; set; }


        public string Description { get; set; }

        public string Filedata { get; set; }

        public int OpportunityDocumentTypeId { get; set; }

        public string TempFileName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class DocumentType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public bool IsActive { get; set; } = true;

        public List<RequestRequiredDocument> RequestRequiredDocumentList { get; set; }
        public RequestNDADocument RequestNDADocumentList { get; set; }
    }
}

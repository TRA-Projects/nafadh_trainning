using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class NDADocument
    {
        public int Id { get; set; }
        public string DocName { get; set; }

        [NotMapped]
        public string DocFileName
        {
            get
            {
                if (!String.IsNullOrEmpty(DocName))
                {

                    return DocName.Substring(DocName.IndexOf("_") + 1);

                }

                return String.Empty;

            }
        }
        public NDAStatus Status { get; set; }

        [NotMapped]
        public string StatusString
        {
            get { string st=String.Empty;
                switch (Status)
                {
                    case NDAStatus.Send:
                        st = "Submitted";
                        break;
                    case NDAStatus.Approve:
                        st = "Approved";
                        break;
                    case NDAStatus.Reject:
                        st = "Rejected";
                        break;
                    default:
                        break;
                }

                return st;

            }
        }
        public string Comments { get; set; }
        public bool IsReSubmit { get; set; }
        public NDA NDA { get; set; }
        public int NDAId { get; set; }

        public DateTime UpdateDateTime { get; set; }

        public RequestNDADocument RequestNDADocument { get; set; }
        public int? RequestNDADocumentId { get; set; }

        [NotMapped]
        public DocumentWrapper DocumentWrapper { get; set; }


        //[NotMapped]
        //public bool IsDeleted { get; set; }
    }
}

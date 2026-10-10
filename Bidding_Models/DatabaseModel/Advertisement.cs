using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class Advertisement : EntityTrail
    {
        public Advertisement()
        {
            AdvertisementList = new List<Advertisement>();
        }
        public int Id { get; set; }
        public string AdverType { get; set; }
        public string Content { get; set; }
        public string Position { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ImgUrl { get; set; }

        [NotMapped]
        public string ImgName
        {
            get
            {

                if (!String.IsNullOrEmpty(ImgUrl))
                {

                    return ImgUrl.Substring(ImgUrl.IndexOf("_") + 1);

                }

                return String.Empty;
            }
        }
        public bool IsActived { get; set; }
        public List<Advertisement> AdvertisementList { get; set; }

        [NotMapped]
        public PagedResult<Advertisement> PagedResult { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }
        [NotMapped]
        public string Active { get; set; }

        [NotMapped]
        public List<DocumentWrapper> DocumentWrapperList { get; set; }
    }
}

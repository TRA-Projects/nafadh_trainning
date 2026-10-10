using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class Banner : EntityTrail
    {
       
        public int Id { get; set; }

        public string Name { get; set; }
        public string ImgURL { get; set; }
        public bool IsActive { get; set; }

        public bool HasText { get; set; }
        public string  Text { get; set; }
        public string Paragraph { get; set; }

        public bool HasButton { get; set; }
        public string ButtonName { get; set; }
        public string  ButtonLink { get; set; }

        public int BannerType { get; set; } //1-Image ; 2-Video

        public string VideoLink { get; set; }

        public int OrderNo { get; set; }

        [NotMapped]

        public IFormFile Image { get; set; }
        [NotMapped]
        public string ImgURLName
        {
            get
            {

                if (!String.IsNullOrEmpty(ImgURL))
                {

                    return ImgURL.Substring(ImgURL.IndexOf("_") + 1);

                }

                return String.Empty;
            }
        }
        [NotMapped]
        public string ImgString { get; set; }
        [NotMapped]
        public List<DocumentWrapper> DocumentWrapperList { get; set; }

    }
}

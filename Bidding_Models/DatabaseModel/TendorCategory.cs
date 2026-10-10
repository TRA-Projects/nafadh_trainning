using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class TendorCategory : EntityTrail
    {
        public TendorCategory()
        {
            ChildCategoryList = new List<TendorCategory>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public string ImgUrl { get; set; }
        public bool IsTrendingCategory { get; set; }
        public bool IsActived { get; set; }
        public bool IsForFreelancerModule { get; set; } = false;

        public TendorCategory ParentCategory { get; set; }
        public int? ParentCategoryId { get; set; }

        public List<VendorCategories> VendorCategoriesList { get; set; }                
        public List<Question> QuestionsList { get; set; }
        public List<TendorCategory> ChildCategoryList { get; set; }

        //[NotMapped]
        //public IFormFile File { get; set; }

        [NotMapped]
        public string OldImgUrl { get; set; }
        [NotMapped]
        public PagedResult<TendorCategory> PagedResult { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }       
        [NotMapped]
        public string Active { get; set; }
        [NotMapped]
        public string isTrendingCategory { get; set; }
    }
}

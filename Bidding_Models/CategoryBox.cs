using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding_Models
{
    public class CategoryBox
    {
        public int TenderCategoryId { get; set; }
        public string TenderName { get; set; }
        public string TenderNameArabic { get; set; }
        public int RequestCount { get; set; }        
    }
}

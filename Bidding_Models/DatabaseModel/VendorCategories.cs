using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class VendorCategories
    {
      
        public AppUser Vendor { get; set; }
        public string VendorId { get; set; }
        public TendorCategory TendorCategory { get; set; }
        public int TendorCategoryId { get; set; }
    }
}

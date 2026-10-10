using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class VendorSavedRequest
    {
        public Request Request { get; set; }
        public int RequestId { get; set; }
        public AppUser Vendor { get; set; }
        public string VendorId { get; set; }

        public DateTime SavedDateTime { get; set; }

        [NotMapped]
        public bool IsSaved { get; set; }

        [NotMapped]
        public PagedResult<VendorSavedRequest> PagedResult { get; set; }
    }
}

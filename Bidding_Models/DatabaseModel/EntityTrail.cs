using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
  public  class EntityTrail
    {
        
        public string InsertedById { get; set; }

        [NotMapped]
        public AppUser InsertedBy { get; set; }

        public DateTime InsertedDate  { get; set; }

 
        public string UpdatedById { get; set; }

        [NotMapped]
        public AppUser UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

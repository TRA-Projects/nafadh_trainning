using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class ProfileDocument
    {
        public int Id { get; set; }
        public string DocName { get; set; }
        public Profile Profile { get; set; }
        public int ProfileId { get; set; }

        // 1 For PortFolio , 2 For Certification
        public int Type { get; set; }

        [NotMapped]
        public bool IsDeleted { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    [Table("OperatorUsers")]
   public class OperatorUser
    {   
        public int Id { get; set; }
        public string UserId { get; set; }
        public int StaffId { get; set; }
        public int ProfileId { get; set; }
    }
}

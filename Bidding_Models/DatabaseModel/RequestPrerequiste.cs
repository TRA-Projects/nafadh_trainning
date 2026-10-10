using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class RequestPrerequiste
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Request Request { get; set; }
        public int RequestId { get; set; }
    }
}

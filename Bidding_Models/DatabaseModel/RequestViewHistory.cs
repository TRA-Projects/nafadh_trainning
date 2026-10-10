using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
  public  class RequestViewHistory
    {
        public int Id { get; set; }

        public Request Request { get; set; }
        public int RequestId { get; set; }
        public int ViewType { get; set; } //1 = View , 2 = Download

        public int ProfileType { get; set; }
        public string SeenById { get; set; }
        public AppUser SeenBy { get; set; }



        public DateTime SeenDate { get; set; }

        [NotMapped]
        public string EncryptedRequestId { get; set; }

        [NotMapped]
        public int ViewCount { get; set; }

        [NotMapped]
        public string SeenByName { get; set; }

    }
}

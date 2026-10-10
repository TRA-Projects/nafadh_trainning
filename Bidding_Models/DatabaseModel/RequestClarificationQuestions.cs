using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class RequestClarificationQuestion : EntityTrail
    {
        public int Id { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }
        public string Question { get; set; }
        [NotMapped]
        public string Name { get; set; }
        public string Answer { get; set; }
        [ForeignKey(nameof(RequestId))]
        public Request Request { get; set; }
        public int RequestId { get; set; }

        public bool ISPublic { get; set; } = false;

        public string FileName { get; set; }

        public List<Notification> NotificationList { get; set; }

        [NotMapped]
        public string CreatedByName { get; set; }

        [NotMapped]
        public string RequestEncryptedId { get; set; }

        [NotMapped]
        public IFormFile Uplodedfile { get; set; }
        [NotMapped]
        public string TempFileName { get; set; }
    }
}

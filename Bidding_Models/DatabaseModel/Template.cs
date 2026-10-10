using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class Template : EntityTrail
    {
        public int Id { get; set; }

       //[AllowHtml]
        public string Text { get; set; }
        public int Type { get; set; }
        public int NotificationTypeId { get; set; }
        public bool ForUser { get; set; }
        public bool ForVendor { get; set; }
        public bool ForAdmin { get; set; }

        public string EmailSubject { get; set; }

        public LanguageEnum Language { get; set; }
        public NotificationType NotificationType { get; set; }

        public bool IsActive { get; set; }

        [NotMapped]
        public PagedResult<Template> PagedResult { get; set; }

        [NotMapped]
        public string EncryptedNotificationTypeId { get; set; }
    }
}

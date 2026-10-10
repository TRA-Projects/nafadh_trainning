using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
  public  class NotificationTypeSetting : EntityTrail
    {
        public int Id { get; set; }
        public NotificationType NotificationType { get; set; }
        public int NotificationTypeId { get; set; }
        public bool IsSendSMS { get; set; }
        public bool IsSendEmail { get; set; }

        [NotMapped]
        public PagedResult<NotificationTypeSetting> PagedResult { get; set; }

        [NotMapped]
        public string NotificationTypeName { get; set; }


    }
}

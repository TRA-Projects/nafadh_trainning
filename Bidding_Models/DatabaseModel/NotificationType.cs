using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class NotificationType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public NotificationTypeSetting NotificationTypeSettings { get; set; }
        public List<Notification> NotificationList { get; set; }


        [NotMapped]
        public PagedResult<NotificationType> PagedResult { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
  public  class SMSEmailNotification
    {
        public int Id { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }

        public string CC { get; set; }

        /// <summary>
        ///   1=SMS
        ///   2=Email
        /// </summary>
        public NotiifcationChannelEnum ChannelType { get; set; }

        public NotificationTypesEnum NotificationType { get; set; }

        public List<SMSEmailNotificationParam> SMSEmailNotificationParamList { get; set; }

        public DateTime InsertedDateTime { get; set; }

        public DateTime SendingDateTime { get; set; }

        public bool IsSend { get; set; }
        public string Reason { get; set; }

        public RolesEnum Role { get; set; }

        public int Status { get; set; } = 0;

        [NotMapped]
        public string Message { get; set; }

    }
}

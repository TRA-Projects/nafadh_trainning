using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class SMSEmailNotificationParam
    {
        public int Id { get; set; }
        public SMSEmailNotification SMSEmailNotification { get; set; }
        public int SMSEmailNotificationId { get; set; }
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
        public int Order { get; set; }

        public bool IsSubjectParam { get; set; } = false;
    }
}

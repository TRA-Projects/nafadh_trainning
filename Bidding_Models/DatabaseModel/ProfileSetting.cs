using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class ProfileSetting
    {

        public int Id { get; set; }
        public bool SendEmail { get; set; }
        public bool SendSms { get; set; }
        public bool ShowEmail { get; set; }
        public bool ShowMobileNo { get; set; }


        //public bool IsEnglish { get; set; }


        public bool IsSendReminderEmail { get; set; } = true;
        public Profile Profile { get; set; }
        public int ProfileId { get; set; }

      
        public LanguageEnum PreferredLanguage { get; set; }

    }
}

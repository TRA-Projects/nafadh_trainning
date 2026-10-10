
using Bidding_Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bidding_Core.Interfaces
{
  public interface ISMSEmailNotification
    {
        bool Save(SMSEmailNotification model);
        bool AddRange(List<SMSEmailNotification> model);
        //int SendEmails();
        int SendEmails();

        Task<int> SendEmailsAsync();
        int SendSMS();

       
    }
}

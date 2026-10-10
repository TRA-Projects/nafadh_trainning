using Bidding_Models.DatabaseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Data.SqlClient;

namespace Bidding_DLL.Data_Access_Layer
{
  public  class SMSEmailNotificationDAC
    {
        private readonly CHDBContext context;


        public SMSEmailNotificationDAC(CHDBContext Context)
        {
            context = Context;
        }

    


        public bool Add(SMSEmailNotification model)
        {
            using (context)
            {
                 context.SMSEmailNotifications.Add(model);
                context.SaveChanges();
                return true;
            }
        }

        public bool Add(List<SMSEmailNotification> model)
        {
            try
            {
                using (context)
                {
                    context.SMSEmailNotifications.AddRange(model);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
          
        }
        public bool Update(SMSEmailNotification model)
        {
            using (context)
            {
                SMSEmailNotification SMSEmail = context.SMSEmailNotifications.FirstOrDefault(s => s.Id == model.Id);

                if (SMSEmail!=null)
                {
                    SMSEmail.IsSend = model.IsSend;
                    SMSEmail.Reason = model.Reason;
                    SMSEmail.SendingDateTime = model.SendingDateTime;
                    context.Entry(SMSEmail).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }

                return false;

            }
        }


        public bool UpdateBulk(List<SMSEmailNotification> model)
        {
            using (context)
            {
                try
                {
                    //  context.Entry<List<SMSEmailNotification>>(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SMSEmailNotifications.UpdateRange(model);

                    context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                   
                

          

            }
        }





        public List<SMSEmailNotification> GetUnsendEmailRecords()
        {
            using (context)
            {
             //   List<SMSEmailNotification> SMSEmail = context.SMSEmailNotifications.Include(c => c.SMSEmailNotificationParamList).ToList();
                List<SMSEmailNotification> SMSEmail = context.SMSEmailNotifications.Include(c=>c.SMSEmailNotificationParamList).Where(s => s.IsSend == false && s.Status==0 && s.ChannelType == Bidding_Models.NotiifcationChannelEnum.Email && s.Reason == null ).OrderByDescending(d=>d.Id).Take(100).ToList();

                context.AttachRange(SMSEmail);
                SMSEmail.ForEach(c => c.Status = 2); //In Progress

                context.SaveChanges();


                return SMSEmail;
            }
        }


        public List<SMSEmailNotification> GetUnsendSMSRecords()
        {
            using (context)
            {
                List<SMSEmailNotification> SMSEmail = context.SMSEmailNotifications.Include(c => c.SMSEmailNotificationParamList).Where(s => s.IsSend == false  && s.Status==0 && s.ChannelType == Bidding_Models.NotiifcationChannelEnum.SMS && s.Reason == null).OrderByDescending(d => d.Id).Take(5).ToList();
                context.AttachRange(SMSEmail);
                SMSEmail.ForEach(c => c.Status = 2); //In Progress
                return SMSEmail;
            }
        }
    }
}

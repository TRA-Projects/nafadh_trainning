using ConstructionHub_Core.Helpers;
using Bidding_Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bidding_Models.DatabaseModel;
using Bidding_Models;
using Bidding_DLL.Data_Access_Layer;
using System.Threading.Tasks;

namespace Bidding_Core.Repository
{
    public class SMSEmailNotificationRepository : ISMSEmailNotification
    {

        private readonly SMSEmailNotificationDAC sMSEmailNotificationDAC;
        private readonly TemplateDAC templateDAC;

        public SMSSettings SMSSettings { get; }
        public EmailSettings EmailSettings { get; }

        public SMSEmailNotificationRepository(SMSEmailNotificationDAC SMSEmailNotificationDAC, TemplateDAC TemplateDAC, SMSSettings SMSSettings, EmailSettings EmailSettings)
        {
            sMSEmailNotificationDAC = SMSEmailNotificationDAC;
            templateDAC = TemplateDAC;
            this.SMSSettings = SMSSettings;
            this.EmailSettings = EmailSettings;
        }
        public bool AddRange(List<SMSEmailNotification> model)
        {
            return sMSEmailNotificationDAC.Add(model);
        }

        public int SendEmails()
        {
          
            List<SMSEmailNotification> emaillst = sMSEmailNotificationDAC.GetUnsendEmailRecords();


        
            if (emaillst!=null && emaillst.Count > 0 )
            {
                
                foreach (var item in emaillst)
                {
                    Template EmailTemplate = null;
                    if (item.Role == RolesEnum.User)
                    {
                        EmailTemplate = templateDAC.GetbyTypeandCarrieranduserType(Convert.ToInt32(item.NotificationType), Convert.ToInt32(NotiifcationChannelEnum.Email), false, true, false);
                            
                    }
                    else if (item.Role == RolesEnum.Vendor)
                    {
                        EmailTemplate = templateDAC.GetbyTypeandCarrieranduserType(Convert.ToInt32(item.NotificationType), Convert.ToInt32(NotiifcationChannelEnum.Email), false, false, true);

                    }
                    else if (item.Role == RolesEnum.Admin)
                    {
                        EmailTemplate = templateDAC.GetbyTypeandCarrieranduserType(Convert.ToInt32(item.NotificationType), Convert.ToInt32(NotiifcationChannelEnum.Email), true, false, false);

                    }
                    if (EmailTemplate != null)
                    {
                        string Email_Subject = String.Empty;
                        if (item.SMSEmailNotificationParamList.Where(p=>p.IsSubjectParam).Count() > 0)
                        {
                            object[] subjectparam = item.SMSEmailNotificationParamList.Where(p => p.IsSubjectParam)?.OrderBy(c => c.Order).Select(c => c.ParamValue).ToArray();
                            Email_Subject = String.Format(EmailTemplate.EmailSubject?.ToString(), subjectparam);
                        }
                        else {

                            Email_Subject = EmailTemplate.EmailSubject?.ToString();
                        }
                        

                        

                        if (item.SMSEmailNotificationParamList?.Count > 0)
                        {
                            object[] param = item.SMSEmailNotificationParamList.Where(p => !p.IsSubjectParam)?.OrderBy(c => c.Order).Select(c => c.ParamValue).ToArray();
                            string Body = String.Format(EmailTemplate.Text.ToString(), param);
                            Email email = new Email(EmailSettings);
                            item.IsSend =email.SendEmail(item,Email_Subject, Body, item.Email,item.CC);
                            if (item.IsSend)
                            {
                                item.Status = 1;
                                item.SendingDateTime = DateTime.Now;
                            }
                            else
                            {
                                //SMSCAPI smsapi = new SMSCAPI(SMSSettings);
                                //smsapi.SendSMS(item,SMSSettings.SMSUserName, SMSSettings.SMSPassword, SMSSettings.DeveloperNumber, "Some Issue in Email Service. Kindly Check the issue.", SMSSettings.SID);
                                //item.Reason = "Not Send.";
                            }
                                
                        }
                        else
                        {

                            string Body = String.Format(EmailTemplate.Text.ToString());
                            Email email = new Email(EmailSettings);
                            item.IsSend =email.SendEmail(item,Email_Subject, Body, item.Email, item.CC);
                            if (item.IsSend)
                            {
                                item.Status = 1;
                                item.SendingDateTime = DateTime.Now;
                            }
                            else
                            {
                                //SMSCAPI smsapi = new SMSCAPI(SMSSettings);
                                //smsapi.SendSMS(item,SMSSettings.SMSUserName, SMSSettings.SMSPassword, SMSSettings.DeveloperNumber, "Some Issue in Email Service. Kindly Check the issue.", SMSSettings.SID);
                                //item.Reason = "Not Send.";
                            }

                            //item.Reason = "No Parameter Found.";
                        }

                    }
                    else
                    {
                        //SMSCAPI smsapi = new SMSCAPI(SMSSettings);
                        //smsapi.SendSMS(item,SMSSettings.SMSUserName, SMSSettings.SMSPassword, SMSSettings.DeveloperNumber, item.NotificationType.ToString() + " Email Template not esist for " + item.Role , SMSSettings.SID);
                        //item.Reason = item.NotificationType.ToString() + " Email Template not esist for " + item.Role;
                    }



                }
                sMSEmailNotificationDAC.UpdateBulk(emaillst);

            }
            return emaillst.Count(c=>c.IsSend == true);
        }


        public async Task<int> SendEmailsAsync()
        {
            try
            {

           

            List<SMSEmailNotification> emaillst = sMSEmailNotificationDAC.GetUnsendEmailRecords();



            if (emaillst != null && emaillst.Count > 0)
            {

                foreach (var item in emaillst)
                {
                    Template EmailTemplate = null;
                    if (item.Role == RolesEnum.User)
                    {
                        EmailTemplate = templateDAC.GetbyTypeandCarrieranduserType(Convert.ToInt32(item.NotificationType), Convert.ToInt32(NotiifcationChannelEnum.Email), false, true, false);

                    }
                    else if (item.Role == RolesEnum.Vendor)
                    {
                        EmailTemplate = templateDAC.GetbyTypeandCarrieranduserType(Convert.ToInt32(item.NotificationType), Convert.ToInt32(NotiifcationChannelEnum.Email), false, false, true);

                    }
                    else if (item.Role == RolesEnum.Admin)
                    {
                        EmailTemplate = templateDAC.GetbyTypeandCarrieranduserType(Convert.ToInt32(item.NotificationType), Convert.ToInt32(NotiifcationChannelEnum.Email), true, false, false);

                    }
                    if (EmailTemplate != null)
                    {
                        string Email_Subject = String.Empty;
                        if (item.SMSEmailNotificationParamList.Where(p => p.IsSubjectParam).Count() > 0)
                        {
                            object[] subjectparam = item.SMSEmailNotificationParamList.Where(p => p.IsSubjectParam)?.OrderBy(c => c.Order).Select(c => c.ParamValue).ToArray();
                            Email_Subject = String.Format(EmailTemplate.EmailSubject?.ToString(), subjectparam);
                        }
                        else
                        {

                            Email_Subject = EmailTemplate.EmailSubject?.ToString();
                        }




                        if (item.SMSEmailNotificationParamList?.Count > 0)
                        {
                              
                                object[] param = item.SMSEmailNotificationParamList.Where(p => !p.IsSubjectParam)?.OrderBy(c => c.Order).Select(c => c.ParamValue).ToArray();
                            string Body = String.Format(EmailTemplate.Text.ToString(), param);
                            Email email = new Email(EmailSettings);
                            item.IsSend = await email.SendEmailAsync(item, Email_Subject, Body, item.Email, item.CC);
                            if (item.IsSend)
                            {
                                item.Status = 1;
                                item.SendingDateTime = DateTime.Now;
                            }
                            else
                            {
                                //SMSCAPI smsapi = new SMSCAPI(SMSSettings);
                                //smsapi.SendSMS(item, SMSSettings.SMSUserName, SMSSettings.SMSPassword, SMSSettings.DeveloperNumber, "Some Issue in Email Service. Kindly Check the issue.", SMSSettings.SID);
                                //item.Reason = "Not Send.";
                            }

                        }
                        else
                        {

                            string Body = String.Format(EmailTemplate.Text.ToString());
                            Email email = new Email(EmailSettings);
                            item.IsSend = await email.SendEmailAsync(item, Email_Subject, Body, item.Email, item.CC);
                            if (item.IsSend)
                            {
                                item.Status = 1;
                                item.SendingDateTime = DateTime.Now;
                            }
                            else
                            {
                                //SMSCAPI smsapi = new SMSCAPI(SMSSettings);
                                //smsapi.SendSMS(item, SMSSettings.SMSUserName, SMSSettings.SMSPassword, SMSSettings.DeveloperNumber, "Some Issue in Email Service. Kindly Check the issue.", SMSSettings.SID);
                                //item.Reason = "Not Send.";
                            }

                            //item.Reason = "No Parameter Found.";
                        }

                    }
                    else
                    {
                        //SMSCAPI smsapi = new SMSCAPI(SMSSettings);
                        //smsapi.SendSMS(item, SMSSettings.SMSUserName, SMSSettings.SMSPassword, SMSSettings.DeveloperNumber, item.NotificationType.ToString() + " Email Template not esist for " + item.Role, SMSSettings.SID);
                        //item.Reason = item.NotificationType.ToString() + " Email Template not esist for " + item.Role;
                    }



                }
                sMSEmailNotificationDAC.UpdateBulk(emaillst);

            }
            return emaillst.Count(c => c.IsSend == true);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int SendSMS()
        {
           
           
            List<SMSEmailNotification> Smslst = sMSEmailNotificationDAC.GetUnsendSMSRecords();

            if (Smslst != null && Smslst.Count > 0)
            {
                SMSCAPI smsapi = new SMSCAPI(SMSSettings);
                foreach (var item in Smslst)
                {
                    Template SMSTemplate = null;
                    if (item.Role == RolesEnum.User)
                    {
                        SMSTemplate = templateDAC.GetbyTypeandCarrieranduserType(Convert.ToInt32(item.NotificationType), Convert.ToInt32(NotiifcationChannelEnum.SMS), false, true, false);

                    }
                    else if (item.Role == RolesEnum.Vendor)
                    {
                        SMSTemplate = templateDAC.GetbyTypeandCarrieranduserType(Convert.ToInt32(item.NotificationType), Convert.ToInt32(NotiifcationChannelEnum.SMS), false, false, true);

                    }
                    else if (item.Role == RolesEnum.Admin)
                    {
                        SMSTemplate = templateDAC.GetbyTypeandCarrieranduserType(Convert.ToInt32(item.NotificationType), Convert.ToInt32(NotiifcationChannelEnum.SMS), true, false, false);

                    }
                    if (item.SMSEmailNotificationParamList?.Count > 0)
                    {
                        object[] param = item.SMSEmailNotificationParamList.OrderBy(c => c.Order).Select(c => c.ParamValue).ToArray();
                        if (SMSTemplate!=null)
                        {
                            string Message = String.Format(SMSTemplate.Text.ToString(), param);
                            if (!String.IsNullOrEmpty(item.ContactNo))
                            {
                                item.IsSend = smsapi.SendSMS(item,SMSSettings.SMSUserName, SMSSettings.SMSPassword, item.ContactNo, Message, SMSSettings.SID);
                                if (item.IsSend)
                                {
                                    item.Status = 1;
                                    item.SendingDateTime = DateTime.Now;
                                }
                              
                            }
                            else
                            {
                                item.Reason = "Contact No is Empty";
                            }
                           
                        }
                        else
                        {
                            item.Reason = "Template Not Found.";
                        }
                       

                    }
                    else
                    {

                        if (SMSTemplate!=null && item.SMSEmailNotificationParamList?.Count == 0)
                        {
                            string Message = String.Format(SMSTemplate.Text.ToString());
                            if (!String.IsNullOrEmpty(item.ContactNo))
                            {
                                item.IsSend = smsapi.SendSMS(item,SMSSettings.SMSUserName, SMSSettings.SMSPassword, item.ContactNo, Message, SMSSettings.SID);
                                if (item.IsSend)
                                {
                                    item.Status = 1;
                                    item.SendingDateTime = DateTime.Now;
                                }
                               
                            }
                            else
                            {
                                item.Reason = "Contact No is Empty";
                            }
                        }
                        else
                        {
                            item.Reason = "No Parameter Found.";
                        }
                        
                    }



                }
                sMSEmailNotificationDAC.UpdateBulk(Smslst);

            }
            return Smslst.Count(c => c.IsSend == true);

       
        }

        public bool Save(SMSEmailNotification model)
        {
            if (model.Id > 0)
            {
                return sMSEmailNotificationDAC.Update(model);
            }
            else
            {
                return sMSEmailNotificationDAC.Add(model);
            }
        }

      
    }
}

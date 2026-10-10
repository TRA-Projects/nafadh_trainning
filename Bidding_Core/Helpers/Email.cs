using Bidding_DLL.Common;
using Bidding_Models.DatabaseModel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionHub_Core.Helpers
{


    public class Email
    {

        public Email(EmailSettings emailSettings)
        {
            EmailSettings = emailSettings;
        }

        public EmailSettings EmailSettings { get; }

        static bool mailSent = false;
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
            mailSent = true;
        }
        public  bool SendEmail(SMSEmailNotification obj,string subject, string body, string to, string CC = null)
        {
            try
            {
                var test = "";

                string fromEmail = EmailSettings.FromEmail;
                var message = new MailMessage
                {

                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = body == null ? "Body is empty" : body,
                    IsBodyHtml = true,
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                };
             

                if (to != null && !String.IsNullOrEmpty(to))
                {
                   
                        List<string> toemails = CommonMethods.DeserializeString(to);
                        if (toemails != null && toemails.Count > 0)
                        {
                            foreach (var item in toemails)
                            {
                                message.To.Add(new MailAddress(item));
                            }

                        }
                   
                   
                }
                if (CC != null && !String.IsNullOrEmpty(CC))
                {
                   

                    List<string> ccemails = CommonMethods.DeserializeString(CC);
                    if (ccemails != null && ccemails.Count > 0)
                    {
                        foreach (var item in ccemails)
                        {
                            message.CC.Add(new MailAddress(item));
                        }
                      
                    }                    
                }

                using (SmtpClient smtpClient = new SmtpClient(EmailSettings.Host, EmailSettings.Port))
                {
                
                    smtpClient.Credentials = new NetworkCredential(EmailSettings.Username, EmailSettings.Password);
                    smtpClient.EnableSsl = EmailSettings.EnableSsl;
                    //smtpClient.SendAsync(message,);

                  
                    smtpClient.Send(message);
                    return true;
                }
            }
            catch (Exception excep)
            {
                if (!String.IsNullOrEmpty(excep.Message?.ToString()))
                {
                    obj.Reason = excep.Message.ToString();
                }
                else
                {
                    obj.Reason = excep.InnerException.ToString();
                }
              


                return false;
            }
        }


        public async Task<bool> SendEmailAsync(SMSEmailNotification obj, string subject, string body, string to, string CC = null)
        {
            try
            {
                var test = "";

                string fromEmail = EmailSettings.FromEmail;
                var message = new MailMessage
                {

                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = body == null ? "Body is empty" : body,
                    IsBodyHtml = true,
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                };


                if (to != null && !String.IsNullOrEmpty(to))
                {

                    List<string> toemails = CommonMethods.DeserializeString(to);
                    if (toemails != null && toemails.Count > 0)
                    {
                        foreach (var item in toemails)
                        {
                            message.To.Add(new MailAddress(item));
                        }

                    }


                }
                if (CC != null && !String.IsNullOrEmpty(CC))
                {


                    List<string> ccemails = CommonMethods.DeserializeString(CC);
                    if (ccemails != null && ccemails.Count > 0)
                    {
                        foreach (var item in ccemails)
                        {
                            message.CC.Add(new MailAddress(item));
                        }

                    }
                }

                using (SmtpClient smtpClient = new SmtpClient(EmailSettings.Host, EmailSettings.Port))
                {
                 
                    smtpClient.Credentials = new NetworkCredential(EmailSettings.Username, EmailSettings.Password);
                    smtpClient.EnableSsl = EmailSettings.EnableSsl;
                    
                    await smtpClient.SendMailAsync(message);
                    return true;
                }
            }
            catch (Exception excep)
            {
                if (!String.IsNullOrEmpty(excep.Message?.ToString()))
                {
                    obj.Reason = excep.Message.ToString();
                }
                else
                {
                    obj.Reason = excep.InnerException.ToString();
                }



                return false;
            }
        }

        public async Task<bool> SendEmailAsync(string subject, string body, string to, string CC = null)
        {
            try
            {
                var test = "";

                string fromEmail = EmailSettings.FromEmail;
                var message = new MailMessage
                {

                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = body == null ? "Body is empty" : body,
                    IsBodyHtml = true,
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                };


                if (to != null && !String.IsNullOrEmpty(to))
                {

                    List<string> toemails = CommonMethods.DeserializeString(to);
                    if (toemails != null && toemails.Count > 0)
                    {
                        foreach (var item in toemails)
                        {
                            message.To.Add(new MailAddress(item));
                        }

                    }


                }
                if (CC != null && !String.IsNullOrEmpty(CC))
                {


                    List<string> ccemails = CommonMethods.DeserializeString(CC);
                    if (ccemails != null && ccemails.Count > 0)
                    {
                        foreach (var item in ccemails)
                        {
                            message.CC.Add(new MailAddress(item));
                        }

                    }
                }

                using (SmtpClient smtpClient = new SmtpClient(EmailSettings.Host, EmailSettings.Port))
                {

                    smtpClient.Credentials = new NetworkCredential(EmailSettings.Username, EmailSettings.Password);
                    smtpClient.EnableSsl = EmailSettings.EnableSsl;

                    await smtpClient.SendMailAsync(message);
                    return true;
                }
            }
            catch (Exception excep)
            {
                return false;
            }
        }

        /// <summary>
        /// Sends an email with a binary attachment (e.g. support-ticket .txt). Addresses are literal email strings.
        /// </summary>
        public async Task<bool> SendEmailWithAttachmentAsync(
            string subject,
            string body,
            IEnumerable<string> toAddresses,
            byte[] attachmentBytes,
            string attachmentFileName,
            string CC = null)
        {
            try
            {
                string fromEmail = EmailSettings.FromEmail;
                string username = EmailSettings.Username;
                using var message = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject ?? "Support ticket",
                    Body = body ?? "Body is empty",
                    IsBodyHtml = true,
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                };

                if (toAddresses != null)
                {
                    foreach (var addr in toAddresses)
                    {
                        if (!string.IsNullOrWhiteSpace(addr))
                        {
                            message.To.Add(new MailAddress(addr.Trim()));
                        }
                    }
                }

                if (message.To.Count == 0)
                {
                    return false;
                }

                if (CC != null && !string.IsNullOrEmpty(CC))
                {
                    List<string> ccemails = CommonMethods.DeserializeString(CC);
                    if (ccemails != null && ccemails.Count > 0)
                    {
                        foreach (var item in ccemails)
                        {
                            message.CC.Add(new MailAddress(item));
                        }
                    }
                }

                Attachment attachment = null;
                System.IO.MemoryStream attachmentStream = null;
                try
                {
                    if (attachmentBytes != null && attachmentBytes.Length > 0)
                    {
                        attachmentStream = new System.IO.MemoryStream(attachmentBytes);
                        attachment = new Attachment(
                            attachmentStream,
                            string.IsNullOrWhiteSpace(attachmentFileName) ? "ticket.txt" : attachmentFileName,
                            "text/plain");
                        message.Attachments.Add(attachment);
                    }

                    using (SmtpClient smtpClient = new SmtpClient(EmailSettings.Host, EmailSettings.Port))
                    {
                        smtpClient.Credentials = new NetworkCredential(username, EmailSettings.Password);
                        smtpClient.EnableSsl = EmailSettings.EnableSsl;
                        smtpClient.Timeout = 30000;
                        await smtpClient.SendMailAsync(message);
                        return true;
                    }
                }
                finally
                {
                    attachment?.Dispose();
                    attachmentStream?.Dispose();
                }
            }
            catch (Exception ex)
            {
                // Do not swallow — support-ticket flow needs the SMTP reason in apilogs.
                throw new InvalidOperationException(
                    $"SMTP send failed (host={EmailSettings?.Host}:{EmailSettings?.Port}, ssl={EmailSettings?.EnableSsl}, from={(EmailSettings?.Username ?? "").Length}c): {ex.Message}",
                    ex);
            }
        }
       
    }
}

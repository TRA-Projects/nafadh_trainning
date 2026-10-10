using System;
using System.IO;
using System.Net;
using System.Text;
using Bidding_Models.DatabaseModel;
using ConstructionHub_Core.Helpers;
using Microsoft.Extensions.Configuration;

namespace Bidding_Core.Helpers
{
    public class NewSmscapi
    {
        private readonly IConfiguration _configuration;
        public NewSmscapi(SMSSettings smsSettings, IConfiguration configuration)
        {
            SmsSettings = smsSettings;
            _configuration = configuration;
        }
   

        public SMSSettings SmsSettings { get; }
        public bool SendSms(
            SMSEmailNotification notification,
            string user,
            string password,
            string mobileNumber,
            string message,
            string senderId)
        {
         
            var finalNumber = mobileNumber.StartsWith("968")
                ? mobileNumber
                :_configuration["SMSSettings:CountryCode"] + mobileNumber;

            var postData = $"Username={user}"
                           + $"&Password={password}"
                           + $"&MSISDNs={finalNumber}"
                           + $"&Message={message}"
                           + $"&SourceRef=TEST"
                           + $"&Priority=1"
                           + $"&DR=Y"
                           + $"&mtype=LNG"
                           + $"&Sender={senderId}";

            try
            {
       
                var request = (HttpWebRequest)WebRequest.Create(_configuration["SMSSettings:ServiceProvider"]);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                var dataBytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = dataBytes.Length;
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(dataBytes, 0, dataBytes.Length);
                }
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
             
                    var result = reader.ReadToEnd();
                }

                return true;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    notification.Reason = ex.Message;
                }
                else if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    notification.Reason = ex.InnerException.Message;
                }

                return false;
            }
        }
    }
}
using Bidding_Models.DatabaseModel;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ConstructionHub_Core.Helpers
{
    public  class SMSCAPI
    {
        public SMSCAPI(SMSSettings SMSSettings)
        {
            this.SMSSettings = SMSSettings;
        }
        private WebProxy objProxy1 = null;

        public SMSSettings SMSSettings { get; }

        public  bool SendSMS(SMSEmailNotification obj, string User, string password, string Mobile_Number, string Message,string SID)
        {
            ////string stringpost = null;

            //string msg = Regex.Replace(Message, "<.*?>", String.Empty);

            //System.Object stringpost = "Username=" + User + "&Password=" + password + "&MSISDNs=" + (Mobile_Number.StartsWith("968") ? Mobile_Number : SMSSettings.CountryCode + Mobile_Number) + "&Message=" + msg + "&SourceRef=TEST&Priority=1" + "&Sender=" + SID;

            //HttpWebRequest objWebRequest = null;
            //HttpWebResponse objWebResponse = null;
            //StreamWriter objStreamWriter = null;
            //StreamReader objStreamReader = null;

            //try
            //{

            //    string stringResult = null;

            //    objWebRequest = (HttpWebRequest)WebRequest.Create("https://tamimahsms.com/API/SendSMS");
            //    objWebRequest.Method = "POST";

            //    if ((objProxy1 != null))
            //    {
            //        objWebRequest.Proxy = objProxy1;
            //    }


            //    // Use below code if you want to SETUP PROXY.
            //    //Parameters to pass: 1. ProxyAddress 2. Port
            //    //You can find both the parameters in Connection settings of your internet explorer.

            //    //WebProxy myProxy = new WebProxy("YOUR PROXY", PROXPORT);
            //    //myProxy.BypassProxyOnLocal = true;
            //    //wrGETURL.Proxy = myProxy;

            //    objWebRequest.ContentType = "application/x-www-form-urlencoded";

            //    objStreamWriter = new StreamWriter(objWebRequest.GetRequestStream());
            //    objStreamWriter.Write(stringpost);
            //    objStreamWriter.Flush();
            //    objStreamWriter.Close();

            //    objWebResponse = (HttpWebResponse)objWebRequest.GetResponse();
            //    objStreamReader = new StreamReader(objWebResponse.GetResponseStream());
            //    stringResult = objStreamReader.ReadToEnd();

            //    objStreamReader.Close();

            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            //finally
            //{

            //    if ((objStreamWriter != null))
            //    {
            //        objStreamWriter.Close();
            //    }
            //    if ((objStreamReader != null))
            //    {
            //        objStreamReader.Close();
            //    }
            //    objWebRequest = null;
            //    objWebResponse = null;
            //    objProxy1 = null;
            //}

            //System.Object stringpost = "User=" + User + "&passwd=" + password + "&mobilenumber=" + 
            //    (Mobile_Number.StartsWith("968") ? Mobile_Number : SMSSettings.CountryCode + Mobile_Number) +
            //    "&message=" + Message + " &DR=Y &mtype=" + "N" +  " &sid=" + SID;


            //System.Object stringpost = "User=" + User + "&passwd=" + password + "&mobilenumber=" +
            //  (Mobile_Number.StartsWith("968") ? Mobile_Number : SMSSettings.CountryCode + Mobile_Number) +
            //  "&message=" + Message + " &DR=Y &mtype=" + "LNG" + " &sid=" + SID;

            System.Object stringpost = "Username=" + User + "&Password=" + password + "&MSISDNs=" + (Mobile_Number.StartsWith("968") ? Mobile_Number : SMSSettings.CountryCode + Mobile_Number) + "&Message=" + Message + "&SourceRef=TEST&Priority=1&DR=Y &mtype=LNG" + " &Sender=" + SID;
            //            User = xxxxxx & passwd = xxxxxxxxxxxx & mobilenumber = xxxxxxxxxx & message = xxxxxxxxx & sid = xxxxxxxx //& mtype = N & DR = Y
            HttpWebRequest objWebRequest = null;
            HttpWebResponse objWebResponse = null;
            System.IO.StreamWriter objStreamWriter = null;
            System.IO.StreamReader objStreamReader = null; 
            try
            { 
                objWebRequest = (HttpWebRequest)HttpWebRequest.Create(SMSSettings.ServiceProvider);
                objWebRequest.Method = "POST";
                objWebRequest.ContentType = "application/x-www-form-urlencoded";
                //objWebRequest.ContentLength = data.Length;
                byte[] data = Encoding.UTF8.GetBytes(stringpost.ToString());
                objStreamWriter = new StreamWriter(objWebRequest.GetRequestStream());
                objStreamWriter.Write(stringpost);
                objStreamWriter.Flush();
                objStreamWriter.Close();

                objWebResponse = (HttpWebResponse)objWebRequest.GetResponse();
                objStreamReader = new StreamReader(objWebResponse.GetResponseStream());
                string stringResult = objStreamReader.ReadToEnd();


                //HttpWebResponse myHttpWebResponse = (HttpWebResponse)objWebRequest.GetResponse(); 
                //Stream responseStream = myHttpWebResponse.GetResponseStream(); 
                //StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default); 
                //string pageContent = myStreamReader.ReadToEnd();

                //myStreamReader.Close();
                objStreamReader.Close();

                objWebResponse.Close();

                return true;
            }
            catch (Exception ex)
            {
                if (!String.IsNullOrEmpty(ex.Message?.ToString()))
                {
                    obj.Reason = ex.Message.ToString();
                }
                else
                {
                    obj.Reason = ex.InnerException.ToString();
                }
                return false;
            }
            finally
            {

                if ((objStreamWriter != null))
                {
                    objStreamWriter.Close();
                }
                if ((objStreamReader != null))
                {
                    objStreamReader.Close();
                }
                objWebRequest = null;
                objWebResponse = null;
                objProxy1 = null;
            }

        }

        private void StreamReader(Stream stream)
        {
            throw new NotImplementedException();
        }

        

    }



}
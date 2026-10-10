using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Bidding_API.Helpers
{
    public class SMSCAPI
    {
        private WebProxy objProxy1 = null;

        public string SendSMS(string User, string password, string Mobile_Number, string Message,string SID)
        {
            //string stringpost = null;

            System.Object stringpost = "Username=" + User + "&Password=" + password + "&MSISDNs=" + (Mobile_Number.StartsWith("968") ? Mobile_Number : SMSSettings.CountryCode + Mobile_Number) + "&Message=" + Message + "&SourceRef=TEST&Priority=1" + "&Sender=" + SID;

            HttpWebRequest objWebRequest = null;
            HttpWebResponse objWebResponse = null;
            System.IO.StreamWriter objStreamWriter = null;
            System.IO.StreamReader objStreamReader = null;

            try
            {

                objWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://www.smscountry.com/SMSCwebservice_bulk.aspx");
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

                return stringResult; 
            }
            catch (Exception ex)
            {
                return ex.Message;
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
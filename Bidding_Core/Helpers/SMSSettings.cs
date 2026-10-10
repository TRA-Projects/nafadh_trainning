using Bidding_Common;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConstructionHub_Core.Helpers
{
    public  class SMSSettings
    {
        private readonly IConfiguration configuration;

        public  SMSSettings(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        public  string SMSUserName
        {
            get
            {
                return configuration["SMSSettings:SMSUserName"].Decrypt();
             //   return "gcs_esnad";
            }
        }


        public  string SMSPassword
        {
            get
            {
                return configuration["SMSSettings:SMSPassword"].Decrypt();
                // return "Globcom2020@";
            }
        }

        public  string SID
        {
            get
            {
                return configuration["SMSSettings:SID"].Decrypt();
              //  return "ESNAD.OM";
            }
        }

        public  string CountryCode
        {
            get
            {
                return configuration["SMSSettings:CountryCode"];
               // return "968";
            }
        }

        public string ServiceProvider
        {
            get
            {
                return configuration["SMSSettings:ServiceProvider"];
                // return "968";
            }
        }

        public string DeveloperNumber
        {
            get
            {
                return configuration["SMSSettings:DeveloperNumber"];
                // return "968";
            }
        }




    }
}
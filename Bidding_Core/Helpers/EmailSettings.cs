using Bidding_Common;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConstructionHub_Core.Helpers
{
    public class EmailSettings
    {
        private readonly IConfiguration configuration;

        public EmailSettings(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public  string Host
        {
            get
            {
                return configuration["EmailSettings:Host"].ToString();
               // return "smtp.office365.com";
            }
        }


        public bool SMTPAuth
        {
            get
            {
                return Convert.ToBoolean(configuration["EmailSettings:SMTPAuth"]);
                // return true;
            }
        }

        public virtual string Username
        {
            get
            {
                return configuration["EmailSettings:Username"].ToString().Decrypt();
                //return "support@globcom-oman.com";
            }
        }

        public virtual string Password
        {
            get
            {
                return configuration["EmailSettings:Password"].ToString().Decrypt();
               // return "Oman2020@10";
            }
        }
        public virtual string FromEmail
        {
            get
            {
                return configuration["EmailSettings:Username"].ToString().Decrypt();
                //return "support@globcom-oman.com";
            }
        }
        public  int Port
        {
            get
            {
                return Convert.ToInt32(configuration["EmailSettings:Port"]);
               // return 587;
            }
        }


        public bool EnableSsl
        {
            get
            {
                return Convert.ToBoolean(configuration["EmailSettings:EnableSsl"]);
                // return 587;
            }
        }

        public string VerifyDomain
        {
            get
            {
                return configuration["EmailSettings:VerifyDomain"];
                // return "globcom-oman.com";
            }
        }
    }
}
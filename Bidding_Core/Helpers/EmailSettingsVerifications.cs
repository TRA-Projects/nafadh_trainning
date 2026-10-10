using Bidding_Common;
using Microsoft.Extensions.Configuration;
using System;

namespace ConstructionHub_Core.Helpers
{
    public class EmailSettingsVerifications : EmailSettings
    {
        private readonly IConfiguration configuration;

        public EmailSettingsVerifications(IConfiguration configuration) : base(configuration) 
        {
            this.configuration = configuration;
        }
        
       

        public override string FromEmail
        {
            get
            {
                return configuration["EmailSettings:Verifications:Username"].ToString().Decrypt();
                //return "support@globcom-oman.com";
            }
        }

    }
}
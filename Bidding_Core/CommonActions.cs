using Bidding_Core;
using Humanizer;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
namespace Bidding_Core
{
   public  class CommonActions : ICommonActions
    {
        private readonly IConfiguration configuration;

      
        public CommonActions(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public DateTime CalculateRequestExpiryDate(DateTime start)
        {
            int addtionaldays = 0;
            DateTime tempstartDate = start;
            //     int days = configuration.GetSection<int>("RequestExpireDays");
            int days = this.configuration.GetValue<int>("RequestExpireDays");
            DateTime endDate = start.AddDays(days);
            while (tempstartDate != endDate)
            {
                if (tempstartDate.DayOfWeek == DayOfWeek.Saturday || tempstartDate.DayOfWeek == DayOfWeek.Friday)
                {
                    addtionaldays++;
                }

                tempstartDate = tempstartDate.AddDays(1);
            }


            if (addtionaldays > 0)
            {
                endDate = endDate.AddDays(addtionaldays);
            }

            return endDate;

        }

        public  int GetVenodeRemiderDays() {
         return configuration.GetValue<int>("VendorRemiderDaysBeforeLastSubmission");
        }

        public int GetUserReminderDays()
        {
            return this.configuration.GetValue<int>("UserReminderDaysBeforeExpiry");
        }

        public string GetWebProtocol()
        {
            return this.configuration.GetValue<string>("WebProtocol");
        }

        public string GetWebHost()
        {
            return this.configuration.GetValue<string>("WebHost");
        }

        public string GetProjectManagersString(string projectManagers)
        {
            if (projectManagers!=null)
            {
                List<string> projectlist = JsonConvert.DeserializeObject<List<string>>(projectManagers);
                return string.Join(",", projectlist);
            }
            return null;
          
        }
        public string ConvertNumbertoWord(int number = 2)
        {
           
           
          return number.ToWords(new System.Globalization.CultureInfo("en-US")); // 543 -> five hundred forty-three

        }
     



    }
}

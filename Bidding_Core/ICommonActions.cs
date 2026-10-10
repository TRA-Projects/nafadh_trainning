using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Core
{
   public interface ICommonActions
    {
        DateTime CalculateRequestExpiryDate(DateTime start);
        int GetVenodeRemiderDays();
        int GetUserReminderDays();
        string GetWebProtocol();
        string GetWebHost();
        string GetProjectManagersString(string projectManagers);
        string ConvertNumbertoWord(int number);

    }
}

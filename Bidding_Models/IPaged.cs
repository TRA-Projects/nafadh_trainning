using System;
using System.Collections.Generic;

namespace Bidding_Models
{
    public interface IPaged
    {
        public int ShowPagesCount { get; }
        int StartPage { get; set; }

        int StopPage { get; set; }

       int CurrentPage { get; set; }

       int PageCount { get; set; }

       int PageSize { get; set; }

       int RowCount { get; set; }
         string ActionName { get; set; }

         string ControllerName { get; set; }

         string UserId { get; set; }

        string RoleName { get; set; }


        IDictionary<string, string> allRouteData { get; set; }
        List<SearchingParameters> SearchingParameters { get; set; }



    }
}

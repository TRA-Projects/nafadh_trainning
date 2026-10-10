using Bidding_Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models
{
   public class PagedResultBase <T>: IPaged
    {
        public PagedResultBase()
        {
            allRouteData = new Dictionary<string, string>();
        }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string EncryptedRequestId { get; set; }
        public int RequestId { get; set; }

        public string EncryptedBidId { get; set; }
        public int BidId { get; set; }

        public int ProfileId { get; set; }
        public int ParentProfileId { get; set; }

        public string RoleName { get; set; }

        public string SearchValue { get; set; }

        public List<VendorCategories> VendorCategories { get; set; }
        public int StartPage { get; set; }

        public int StopPage { get; set; }

        public int CurrentPage { get; set; }
        
        public int PageCount { get; set; }

        public int PageSize { get; set; } = 12;
        
        public int RowCount { get; set; }
        
        public int FirstRowOnPage
        {

            get { return (CurrentPage - 1) * PageSize + 1; }
        }
        
        public int LastRowOnPage
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }
        public int ShowPagesCount
        {
            get { return 5; }
        }

        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        //public List<Tuple<string, string, dynamic>> SearchingParams { get; set; }
        //public List<Tuple<string, string, dynamic>> OrderingParams { get; set; }
        public T SearchParametersObject { get; set; }

        public List<SearchingParameters> SearchingParameters { get; set; }

        public IDictionary<string, string> allRouteData { get; set; }

        public string Sortby { get; set; }
        public string Direction { get; set; }

        public List<DatatableSearchParams> SearchingParametersForDatatable { get; set; }
        public int ProfileType { get; set; }

        public int StartCount { get; set; }

    }

    public class DatatableSearchParams
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}

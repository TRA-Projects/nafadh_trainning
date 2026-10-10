using Bidding_Models.DatabaseModel;
using System.Collections.Generic;

namespace Bidding_Models
{
    public class Dashboard
    {
        public Dashboard()
        {
            NewRequests = new List<Request>();
            ActiveRequests = new List<Request>();
            EvaluationRequests = new List<Request>();
            AwardedRequests = new List<Request>();
            CategoryBoxes = new List<CategoryBox>();

        }
        public List<CategoryBox> CategoryBoxes { get; set; }
        public List<Request> NewRequests { get; set; }
        public List<Request> ActiveRequests { get; set; }
        public List<Request> EvaluationRequests { get; set; }
        public List<Request> AwardedRequests { get; set; }
        public int AllRequests { get; set; }
        public int AllRequestsByUserId { get; set; }
        public int ActiveRequest { get; set; }
        public int ActiveRequestsById { get; set; }

        public int PendingRequest { get; set; }
        public int EvaluatingRequest { get; set; }
        public int EvaluatingRequestsById { get; set; }

        public int AwardedRequest { get; set; }
        public int AwardedRequestById { get; set; }

        public int ExpiredRequest { get; set; }

        public int AllProjects { get; set; }

        public int PendingProjects { get; set; }

        public int InitiatedProjects { get; set; }

        public int ClosedProjects { get; set; }

        public int QuoutedRFQS { get; set; }

        public int SavedRFQS { get; set; }

        public int NoofVendors { get; set; }

        public int NoofClients { get; set; }
        public int NoofPendingVerificationsVendors { get; set; }

        public int TotalRequest { get; set; }
        public int TotalParticipants { get; set; }

        public int TotalActiveUsers { get; set; }

       public List<TrainigDash> Trainig = new List<TrainigDash>();


        public int ClientsContractor { get; set; }
        public int ClientsOperator { get; set; }

        public int VendorSme { get; set; }
        public int Vendorfreelancer { get; set; }

        public string ChartUsersDataJson { get; set; }
        public string ChartOpportunityStatusDataJson { get; set; }
        public string ChartAwardedOpportunitiesDataJson { get; set; }
        public string ChartServiceCategoriesDataJson { get; set; }
        public string ChartAwardedValuesDataJson { get; set; }
        public string ChartAwardedOpportunitiesOperatorDataJson { get; set; }        
        public string ChartAwardedOpportunitiesAmountOperatorDataJson { get; set; }
        public List<string> OperatorsListForOpportunityStatusChartCategories { get; set; }        
        public List<string> OperatorsListForAwardedOpportunities { get; set; }        
        public List<string> OperatorsListForCategoryChart { get; set; }        
        public List<DashboardChartTraining> dashboardChartTraining { get; set; }
    }

    public class TrainigDash
    {
        public int value { get; set; }

        public string name { get; set; }
    }

}

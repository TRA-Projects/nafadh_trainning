using System;
using System.Collections.Generic;

namespace Bidding_Models.DTOs
{
    public class AdminDashboardResponseDto
    {
        public AdminDashboardResponseDto()
        {
            ProjectStatusSummary = new AdminDashboardProjectStatusSummaryDto
            {
                Active = 0,
                PendingApproval = 0,
                Completed = 0
            };
            BudgetSummary = new AdminDashboardBudgetSummaryDto
            {
                TotalSpent = 0m,
                SpentPercentage = 0
            };
            BalanceSummary = new AdminDashboardBalanceSummaryDto
            {
                RemainingBalance = 0m,
                RemainingPercentage = 0
            };
            PaymentStatusOverview = new AdminDashboardPaymentStatusOverviewDto
            {
                Successful = 0m,
                Processing = 0m,
                Failed = 0m,
                LastPaidDate = DateTime.MinValue
            };
            RecentTransactions = new List<AdminDashboardTransactionDto>();
            ActiveProjects = new List<AdminDashboardProjectDto>();
        }
        public AdminDashboardProjectStatusSummaryDto ProjectStatusSummary { get; set; }
        public AdminDashboardBudgetSummaryDto BudgetSummary { get; set; }
        public AdminDashboardBalanceSummaryDto BalanceSummary { get; set; }
        public AdminDashboardPaymentStatusOverviewDto PaymentStatusOverview { get; set; }
        public List<AdminDashboardTransactionDto> RecentTransactions { get; set; }
        public List<AdminDashboardProjectDto> ActiveProjects { get; set; }
    }
    public class AdminDashboardProjectStatusSummaryDto
    {
        public int Active { get; set; }
        public int PendingApproval { get; set; }
        public int Completed { get; set; }
    }

    public class AdminDashboardBudgetSummaryDto
    {
        public decimal TotalSpent { get; set; }
        public double SpentPercentage { get; set; }
    }

    public class AdminDashboardBalanceSummaryDto
    {
        public decimal RemainingBalance { get; set; }
        public double RemainingPercentage { get; set; }
    }

    public class AdminDashboardPaymentStatusOverviewDto
    {
        public decimal Successful { get; set; }
        public decimal Processing { get; set; }
        public decimal Failed { get; set; }
        public DateTime LastPaidDate { get; set; }
    }

    public class AdminDashboardTransactionDto
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } 
    }

    public class AdminDashboardProjectDto
    {
        public string Title { get; set; }
        public string Freelancer { get; set; }
        public string ProjectType { get; set; }
        public decimal Budget { get; set; }
        public string Status { get; set; } 
        public int MilestoneProgressPercent { get; set; }
    }
}
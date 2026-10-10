using System;
using System.Collections.Generic;

namespace Bidding_Models.DTOs
{
    public class SmeDashboardResponseDto
    {
        public SmeDashboardResponseDto()
        {
            ProjectStatusSummary = new SmeDashboardProjectStatusSummaryDto
            {
                Active = 0,
                PendingApproval = 0,
                Completed = 0
            };
            BudgetSummary = new SmeDashboardBudgetSummaryDto
            {
                TotalSpent = 0m,
                SpentPercentage = 0
            };
            BalanceSummary = new SmeDashboardBalanceSummaryDto
            {
                RemainingBalance = 0m,
                RemainingPercentage = 0
            };
            PaymentStatusOverview = new SmeDashboardPaymentStatusOverviewDto
            {
                Successful = 0m,
                Processing = 0m,
                Failed = 0m,
                LastPaidDate = DateTime.MinValue
            };
            RecentTransactions = new List<SmeDashboardTransactionDto>();
            ActiveProjects = new List<SmeDashboardProjectDto>();
        }
        public SmeDashboardProjectStatusSummaryDto ProjectStatusSummary { get; set; }
        public SmeDashboardBudgetSummaryDto BudgetSummary { get; set; }
        public SmeDashboardBalanceSummaryDto BalanceSummary { get; set; }
        public SmeDashboardPaymentStatusOverviewDto PaymentStatusOverview { get; set; }
        public List<SmeDashboardTransactionDto> RecentTransactions { get; set; }
        public List<SmeDashboardProjectDto> ActiveProjects { get; set; }
    }
    public class SmeDashboardProjectStatusSummaryDto
    {
        public int Active { get; set; }
        public int PendingApproval { get; set; }
        public int Completed { get; set; }
    }

    public class SmeDashboardBudgetSummaryDto
    {
        public decimal TotalSpent { get; set; }
        public double SpentPercentage { get; set; }
    }

    public class SmeDashboardBalanceSummaryDto
    {
        public decimal RemainingBalance { get; set; }
        public double RemainingPercentage { get; set; }
    }

    public class SmeDashboardPaymentStatusOverviewDto
    {
        public decimal Successful { get; set; }
        public decimal Processing { get; set; }
        public decimal Failed { get; set; }
        public DateTime LastPaidDate { get; set; }
    }

    public class SmeDashboardTransactionDto
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } 
    }

    public class SmeDashboardProjectDto
    {
        public string Title { get; set; }
        public string Freelancer { get; set; }
        public string ProjectType { get; set; }
        public decimal Budget { get; set; }
        public string Status { get; set; } 
        public int MilestoneProgressPercent { get; set; }
    }
}
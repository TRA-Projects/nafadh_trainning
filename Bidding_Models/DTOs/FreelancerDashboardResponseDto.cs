using System;
using System.Collections.Generic;

namespace Bidding_Models.DTOs
{
    public class FreelancerDashboardResponseDto
    {
        public FreelancerDashboardResponseDto()
        {
            ProjectStatus = new FreelancerDashboardProjectStatusDto
            {
                TotalProjects = 0,
                CompletedProjects = 0,
                ActiveProjects = 0,
                UnderEvaluationProjects = 0
            };
            Earnings = new FreelancerDashboardEarningsDto
            {
                TotalEarning = 0m,
                PendingThisMonth = 0m
            };
            Payment = new FreelancerDashboardPaymentDto
            {
                WithdrawalBalance = 0m,
                NextPaymentReleaseDate = DateTime.MinValue
            };
            ProjectStatistics = new List<FreelancerDashboardProjectStatisticPointDto>
            {
                new FreelancerDashboardProjectStatisticPointDto { Month = "Jan", Completed = 0, InProgress = 0 },
                new FreelancerDashboardProjectStatisticPointDto { Month = "Feb", Completed = 0, InProgress = 0 },
                new FreelancerDashboardProjectStatisticPointDto { Month = "Mar", Completed = 0, InProgress = 0 },
                new FreelancerDashboardProjectStatisticPointDto { Month = "Apr", Completed = 0, InProgress = 0 },
                new FreelancerDashboardProjectStatisticPointDto { Month = "May", Completed = 0, InProgress = 0 },
                new FreelancerDashboardProjectStatisticPointDto { Month = "Jun", Completed = 0, InProgress = 0 },
                new FreelancerDashboardProjectStatisticPointDto { Month = "Jul", Completed = 0, InProgress = 0 },
                new FreelancerDashboardProjectStatisticPointDto { Month = "Aug", Completed = 0, InProgress = 0 },
                new FreelancerDashboardProjectStatisticPointDto { Month = "Sep", Completed = 0, InProgress = 0 },
                new FreelancerDashboardProjectStatisticPointDto { Month = "Oct", Completed = 0, InProgress = 0 },
                new FreelancerDashboardProjectStatisticPointDto { Month = "Nov", Completed = 0, InProgress = 0 },
                new FreelancerDashboardProjectStatisticPointDto { Month = "Dec", Completed = 0, InProgress = 0 }
            };
            ActiveProjects = new List<FreelancerDashboardActiveProjectDto>();
        }
        public FreelancerDashboardProjectStatusDto ProjectStatus { get; set; }
        public FreelancerDashboardEarningsDto Earnings { get; set; }
        public FreelancerDashboardPaymentDto Payment { get; set; }
        public List<FreelancerDashboardProjectStatisticPointDto> ProjectStatistics { get; set; }
        public List<FreelancerDashboardActiveProjectDto> ActiveProjects { get; set; }
    }

    public class FreelancerDashboardProjectStatusDto
    {
        public int TotalProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int UnderEvaluationProjects { get; set; }
    }

    public class FreelancerDashboardEarningsDto
    {
        public decimal TotalEarning { get; set; }
        public decimal PendingThisMonth { get; set; }
    }

    public class FreelancerDashboardPaymentDto
    {
        public decimal WithdrawalBalance { get; set; }
        public DateTime NextPaymentReleaseDate { get; set; }
    }

    public class FreelancerDashboardProjectStatisticPointDto
    {
        public string Month { get; set; } // e.g. "Jan", "Feb"
        public int Completed { get; set; }
        public int InProgress { get; set; }
    }

    public class FreelancerDashboardActiveProjectDto
    {
        public string Title { get; set; }
        public string ClientName { get; set; }
        public string ProjectType { get; set; }
        public string ApplicationType { get; set; }
        public decimal Price { get; set; }
        public string DeliverIn { get; set; } // e.g. "2 Days", "12 Days"
        public int ProgressPercent { get; set; }
    }
}
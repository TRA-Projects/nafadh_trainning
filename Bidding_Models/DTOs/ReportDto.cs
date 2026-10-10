using System;
using System.Collections.Generic;
using System.Text;
using static Bidding_Models.Enums;

namespace Bidding_Models.DTOs
{
    public class ReportDto
    {

        public ReportDto()
        {
            SiteStatus = new SiteStatusDto
            {
                activeUser = 10,
                sessionDuration = "22m",
                bounceRate = "12%",
                topFeaturesUsed = "70%",
                prjConversionRate = "80%",
                totalUsers = 1000
            };

            UserStatistics = new List<UserActivityDto>
            {
                new UserActivityDto { Month = "Jan", SME = 2, Freelancer = 1 },
                new UserActivityDto { Month = "Feb", SME = 1, Freelancer = 2 },
                new UserActivityDto { Month = "Mar", SME = 3, Freelancer = 0 },
                new UserActivityDto { Month = "Apr", SME = 2, Freelancer = 1 },
                new UserActivityDto { Month = "May", SME = 1, Freelancer = 2 },
                new UserActivityDto { Month = "Jun", SME = 2, Freelancer = 1 },
                new UserActivityDto { Month = "Jul", SME = 3, Freelancer = 0 },
                new UserActivityDto { Month = "Aug", SME = 2, Freelancer = 2 },
                new UserActivityDto { Month = "Sep", SME = 1, Freelancer = 1 },
                new UserActivityDto { Month = "Oct", SME = 2, Freelancer = 0 },
                new UserActivityDto { Month = "Nov", SME = 1, Freelancer = 2 },
                new UserActivityDto { Month = "Dec", SME = 2, Freelancer = 1 }
            };

            RecentReports = new List<RecentReportDataDto>(){
            new RecentReportDataDto
            {
                ReportName = "Monthly Performance",
                ReportType = "Performance",
                ReportDate = DateTime.UtcNow.AddDays(-30).ToShortDateString(),
                Format = "PDF"
            },
            new RecentReportDataDto
            {
                ReportName = "Project Status",
                ReportType = "Status",
                ReportDate = DateTime.UtcNow.AddDays(-15).ToShortDateString(),
                Format = "Excel"
            },
            new RecentReportDataDto
            {
                ReportName = "Financial Overview",
                ReportType = "Financial",
                ReportDate = DateTime.UtcNow.AddDays(-7).ToShortDateString(),
                Format = "CSV"
            },
            new RecentReportDataDto
            {
                ReportName = "User Engagement",
                ReportType = "Engagement",
                ReportDate = DateTime.UtcNow.AddDays(-5).ToShortDateString(),
                Format = "PDF"
            },
            new RecentReportDataDto
            {
                ReportName = "System Audit",
                ReportType = "Audit",
                ReportDate = DateTime.UtcNow.AddDays(-2).ToShortDateString(),
                Format = "Excel"
            },

           };
        }


        public FinancialStatusDto FinancialStatus { get; set; } = new FinancialStatusDto
        {
            TotalRevenue = "OMR 325,000 ",
            TotalFreelancerPaid = "OMR 246,000",
            PlatformCommissionEarned = "OMR 45,000",
            AvgProjectValue = "OMR 870",
            AveragePayoutTime = "3.4 days"
        };

        public SiteStatusDto SiteStatus { get; set; }

        public List<RecentReportDataDto> RecentReports { get; set; }

        public List<UserActivityDto> UserStatistics { get; set; }

        public List<FeatureUsageDto> FeatureUsage { get; set; } = new List<FeatureUsageDto>
        {
            new FeatureUsageDto { FeatureName = "Chat", UsagePercentage = 32 },
            new FeatureUsageDto { FeatureName = "Proposal", UsagePercentage = 19 },
            new FeatureUsageDto { FeatureName = "Others", UsagePercentage = 50 },          
        };

        public List<InsightDto> SiteInsights { get; set; } = new List<InsightDto>
        {            
            new InsightDto { InsightDescription = "Freelancer activity increased 18% in the last 30 days.", InsightType = "up" },
            new InsightDto { InsightDescription = "SME project posting dipped after mid-april.", InsightType = "down" },
            new InsightDto { InsightDescription = "Peak logins between 10:00-13:00 daily.", InsightType = "neutral" }
        };

        public List<RevenuActivityDto> RevenueActivity { get; set; } = new List<RevenuActivityDto>
        {
            new RevenuActivityDto { Month = "Jan", Revenue = 5000, Commission = 500, Withdrawals = 2000 },
            new RevenuActivityDto { Month = "Feb", Revenue = 6000, Commission = 600, Withdrawals = 2500 },
            new RevenuActivityDto { Month = "Mar", Revenue = 7000, Commission = 700, Withdrawals = 3000 },
            new RevenuActivityDto { Month = "Apr", Revenue = 8000, Commission = 800, Withdrawals = 3500 },
            new RevenuActivityDto { Month = "May", Revenue = 9000, Commission = 900, Withdrawals = 4000 },
            new RevenuActivityDto { Month = "Jun", Revenue = 10000, Commission = 1000, Withdrawals = 4500 },
            new RevenuActivityDto { Month = "Jul", Revenue = 11000, Commission = 1100, Withdrawals = 5000 },
            new RevenuActivityDto { Month = "Aug", Revenue = 12000, Commission = 1200, Withdrawals = 5500 },
            new RevenuActivityDto { Month = "Sep", Revenue = 13000, Commission = 1300, Withdrawals = 6000 },
            new RevenuActivityDto { Month = "Oct", Revenue = 14000, Commission = 1400, Withdrawals = 6500 },
            new RevenuActivityDto { Month = "Nov", Revenue = 15000, Commission = 1500, Withdrawals = 7000 },
            new RevenuActivityDto { Month = "Dec", Revenue = 16000, Commission = 1600, Withdrawals = 7500 }
        };

        public List<CategoryDistributionDto> CategoryDistribution { get; set; } = new List<CategoryDistributionDto>
        {
            new CategoryDistributionDto { CategoryName = "Website", DistributionPercentage = 30 },
            new CategoryDistributionDto { CategoryName = "App", DistributionPercentage = 25 },
            new CategoryDistributionDto { CategoryName = "Others", DistributionPercentage = 20 },            
        };


        public List<InsightDto> FinancialInsights { get; set; } = new List<InsightDto>
        {
            new InsightDto { InsightDescription = "Commission from design category up 18% this quarter.", InsightType = "up" },
            new InsightDto { InsightDescription = "Low engagement in logistics category - revenue dropped 12%.", InsightType = "down" },
            new InsightDto { InsightDescription = "Avg payout cycle improved from 5.2 to 3.4 days.", InsightType = "neutral" }
        };

    }

    public class SiteStatusDto
    {
        public int activeUser { get; set; }
        public string sessionDuration { get; set; }
        public string bounceRate { get; set; }
        public string topFeaturesUsed { get; set; }
        public string prjConversionRate { get; set; }
        public int totalUsers { get; set; }

    }

    public class FinancialStatusDto
    {
        public string TotalRevenue { get; set; }
        public string TotalFreelancerPaid { get; set; }
        public string PlatformCommissionEarned { get; set; }
        public string AvgProjectValue { get; set; }
        public string AveragePayoutTime { get; set; }        

    }


    public class RecentReportDataDto
    {
        public string ReportName { get; set; }
        public string ReportType { get; set; }
        public string ReportDate { get; set; }
        public string Format { get; set; }
    }

    public class UserActivityDto
    {
        public string Month { get; set; }
        public int SME { get; set; }
        public int Freelancer { get; set; }

    }


    public class RevenuActivityDto
    {
        public string Month { get; set; }
        public decimal Revenue { get; set; }
        public decimal Commission { get; set; }
        public decimal Withdrawals { get; set; }
    }


    public class  FeatureUsageDto
    {
        public string FeatureName { get; set; }
        public int UsagePercentage { get; set; }        

    }

    public class CategoryDistributionDto
    {
        public string CategoryName { get; set; }
        public int DistributionPercentage { get; set; }

    }

    public class InsightDto
    {
         public string InsightDescription { get; set; }
         public string InsightType { get; set; }
    }
}
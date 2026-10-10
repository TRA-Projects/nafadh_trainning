using System;

namespace Bidding_Models.Jobseekers
{
    /// <summary>Sample record used by the UI showcase pages (form, grid, popup). Served from an in-memory mock.</summary>
    public class ShowcaseItemDto
    {
        public long Id { get; set; }
        public string Title_en { get; set; }
        public string Title_ar { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string ContactEmail { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsUrgent { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public bool CanEdit { get; set; }
    }

    public class UpsertShowcaseItemRequestDto
    {
        public string Title_en { get; set; }
        public string Title_ar { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string ContactEmail { get; set; }
        public string PhoneNumber { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsUrgent { get; set; }
        public string Description { get; set; }
    }
}

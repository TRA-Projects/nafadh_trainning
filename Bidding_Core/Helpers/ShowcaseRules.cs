using System;
using System.Linq;
using System.Text.RegularExpressions;
using Bidding_Models.Jobseekers;

namespace Bidding_Core.Helpers
{
    /// <summary>Validation and edit-lock rules for the UI showcase sample records. Mirrored by the Angular form.</summary>
    public static class ShowcaseRules
    {
        public const int TitleMaxLength = 200;
        public const int DescriptionMaxLength = 1000;
        public const decimal MinAmount = 1m;

        public const string StatusDraft = "Draft";
        public const string StatusSubmitted = "Submitted";
        public const string StatusApproved = "Approved";

        public static readonly string[] Categories = { "Training", "Equipment", "Travel" };
        public static readonly string[] Statuses = { StatusDraft, StatusSubmitted, StatusApproved };

        private static readonly Regex EmailRegex = new Regex(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$");
        private static readonly Regex PhoneRegex = new Regex(@"^\+?\d+$");

        /// <summary>Approved records are read-only; they cannot be edited or deleted.</summary>
        public static bool CanEdit(string status) =>
            !string.Equals(status, StatusApproved, StringComparison.OrdinalIgnoreCase);

        /// <summary>Returns the first validation error, or <c>null</c> when the request is valid.</summary>
        public static string Validate(UpsertShowcaseItemRequestDto body)
        {
            if (body == null) return "Request body is required.";

            var titleEn = (body.Title_en ?? string.Empty).Trim();
            var titleAr = (body.Title_ar ?? string.Empty).Trim();
            if (titleEn.Length == 0 || titleAr.Length == 0) return "Title is required in English and Arabic.";
            if (titleEn.Length > TitleMaxLength || titleAr.Length > TitleMaxLength)
                return $"Title must be {TitleMaxLength} characters or fewer.";

            if (!Categories.Contains((body.Category ?? string.Empty).Trim())) return "Select a valid category.";
            if (!Statuses.Contains((body.Status ?? string.Empty).Trim())) return "Select a valid status.";

            var email = (body.ContactEmail ?? string.Empty).Trim();
            if (email.Length == 0) return "Contact email is required.";
            if (!EmailRegex.IsMatch(email)) return "Enter a valid email address.";

            var phone = (body.PhoneNumber ?? string.Empty).Trim();
            if (phone.Length > 0 && !PhoneRegex.IsMatch(phone)) return "Enter a valid phone number.";

            if (body.Amount == null) return "Amount is required.";
            if (body.Amount < MinAmount) return $"Amount must be at least {MinAmount}.";

            if (body.StartDate == null || body.EndDate == null) return "Start and end dates are required.";
            if (body.EndDate.Value.Date < body.StartDate.Value.Date) return "End date must be on or after the start date.";

            if ((body.Description ?? string.Empty).Length > DescriptionMaxLength)
                return $"Description must be {DescriptionMaxLength} characters or fewer.";

            return null;
        }
    }
}

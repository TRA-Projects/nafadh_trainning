using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Bidding_API.Common
{
    /// <summary>
    /// CORS and redirect origins for the Jobseekers and Freelancers frontends.
    /// Configure under <c>FrontendOrigins:Jobseekers</c> and <c>FrontendOrigins:Freelancers</c>.
    /// </summary>
    public static class FrontendOriginsConfiguration
    {
        public const string SectionName = "FrontendOrigins";
        public const string JobseekersKey = "Jobseekers";
        public const string FreelancersKey = "Freelancers";

        public static string GetJobseekersOrigin(IConfiguration configuration) =>
            configuration.GetSection(SectionName)[JobseekersKey];

        public static string GetFreelancersOrigin(IConfiguration configuration) =>
            configuration.GetSection(SectionName)[FreelancersKey];

        /// <summary>
        /// All non-empty configured frontend origins (deduplicated, case-insensitive).
        /// Falls back to legacy <c>frontendDomain</c> / <c>frontendDomains</c> when section is missing.
        /// </summary>
        public static string[] GetAllowedOrigins(IConfiguration configuration)
        {
            var section = configuration.GetSection(SectionName);
            if (section.Exists())
            {
                var namedOrigins = new[]
                    {
                        section[JobseekersKey],
                        section[FreelancersKey],
                    }
                    .Where(origin => !string.IsNullOrWhiteSpace(origin))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                if (namedOrigins.Length > 0)
                {
                    return namedOrigins;
                }
            }

            var legacyOrigins = configuration.GetSection("frontendDomains").Get<string[]>();
            if (legacyOrigins != null && legacyOrigins.Length > 0)
            {
                return legacyOrigins
                    .Where(origin => !string.IsNullOrWhiteSpace(origin))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }

            var singleOrigin = configuration.GetValue<string>("frontendDomain");
            return string.IsNullOrWhiteSpace(singleOrigin)
                ? Array.Empty<string>()
                : new[] { singleOrigin };
        }

        /// <summary>
        /// Origin used for server-side redirects (e.g. email verification).
        /// Prefers legacy <c>frontendDomain</c>, then Freelancers, then Jobseekers.
        /// </summary>
        public static string GetRedirectOrigin(IConfiguration configuration)
        {
            var legacy = configuration.GetValue<string>("frontendDomain");
            if (!string.IsNullOrWhiteSpace(legacy))
            {
                return legacy;
            }

            var freelancers = GetFreelancersOrigin(configuration);
            if (!string.IsNullOrWhiteSpace(freelancers))
            {
                return freelancers;
            }

            return GetJobseekersOrigin(configuration);
        }
    }
}

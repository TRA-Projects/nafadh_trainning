using System;
using System.Collections.Generic;
using System.Linq;
using Bidding_Models.Jobseekers;

namespace Bidding_Core.Helpers
{
    public static class NotificationPlaceholderRegistry
    {
        private static readonly IReadOnlyDictionary<string, PlaceholderDef[]> ByAction =
            new Dictionary<string, PlaceholderDef[]>(StringComparer.OrdinalIgnoreCase)
            {
                [NotificationTemplateActionCodes.AskToApply] = new[]
                {
                    Common("jobseeker_name"),
                    Common("job_title"),
                    Common("company_name"),
                    Common("operator_name"),
                    Common("portal_link"),
                },
                [NotificationTemplateActionCodes.JobApplied] = new[]
                {
                    Common("jobseeker_name"),
                    Common("job_title"),
                    Common("company_name"),
                    Common("application_date"),
                    Common("portal_link"),
                },
                [NotificationTemplateActionCodes.ApplicationStatusUpdated] = new[]
                {
                    Common("jobseeker_name"),
                    Common("job_title"),
                    Common("company_name"),
                    Common("application_status"),
                    Common("portal_link"),
                },
                [NotificationTemplateActionCodes.InterviewScheduled] = new[]
                {
                    Common("jobseeker_name"),
                    Common("job_title"),
                    Common("company_name"),
                    Common("interview_date"),
                    Common("interview_location"),
                    Common("portal_link"),
                },
                [NotificationTemplateActionCodes.OfferSubmitted] = new[]
                {
                    Common("jobseeker_name"),
                    Common("job_title"),
                    Common("company_name"),
                    Common("portal_link"),
                },
                [NotificationTemplateActionCodes.OfferReissued] = new[]
                {
                    Common("jobseeker_name"),
                    Common("job_title"),
                    Common("company_name"),
                    Common("portal_link"),
                },
                [NotificationTemplateActionCodes.OfferCandidateResponse] = new[]
                {
                    Common("operator_name"),
                    Common("jobseeker_name"),
                    Common("job_title"),
                    Common("company_name"),
                    Common("response_status"),
                    Common("portal_link"),
                },
                [NotificationTemplateActionCodes.OfferInquiryReceived] = new[]
                {
                    Common("recipient_name"),
                    Common("jobseeker_name"),
                    Common("operator_name"),
                    Common("job_title"),
                    Common("company_name"),
                    Common("inquiry_preview"),
                    Common("portal_link"),
                },
                [NotificationTemplateActionCodes.OpInterviewInvitation] = new[]
                {
                    Common("jobseeker_name"),
                    Common("company_name"),
                    Common("opportunity_title"),
                    Common("interview_date"),
                    Common("interview_time"),
                    Common("location_or_link"),
                    Common("instructions"),
                    Common("portal_link"),
                },
                [NotificationTemplateActionCodes.OpUpdInterview] = new[]
                {
                    Common("jobseeker_name"),
                    Common("company_name"),
                    Common("opportunity_title"),
                    Common("interview_date"),
                    Common("interview_time"),
                    Common("location_or_link"),
                    Common("instructions"),
                    Common("portal_link"),
                },
                [NotificationTemplateActionCodes.RegistrationEmailVerification] = new[]
                {
                    Common("jobseeker_name"),
                    Common("verification_link"),
                },
                [NotificationTemplateActionCodes.EvaluationInvitation] = new[]
                {
                    Common("jobseeker_name"),
                    Common("evaluation_title"),
                    Common("assessment_date"),
                    Common("instructions"),
                    Common("portal_link"),
                },
                [NotificationTemplateActionCodes.EvaluationUpdate] = new[]
                {
                    Common("jobseeker_name"),
                    Common("evaluation_title"),
                    Common("assessment_date"),
                    Common("instructions"),
                    Common("portal_link"),
                },
            };

        public static IReadOnlyList<NotificationPlaceholderDto> GetPlaceholders(string actionCode)
        {
            if (string.IsNullOrWhiteSpace(actionCode) || !ByAction.TryGetValue(actionCode.Trim(), out var defs))
            {
                return Array.Empty<NotificationPlaceholderDto>();
            }

            return defs.Select(d => new NotificationPlaceholderDto
            {
                Token = d.Token,
                LabelEn = d.LabelEn,
                LabelAr = d.LabelAr,
                SampleValue = d.SampleValue,
            }).ToList();
        }

        public static HashSet<string> GetAllowedTokens(string actionCode)
        {
            return new HashSet<string>(
                GetPlaceholders(actionCode).Select(p => p.Token),
                StringComparer.OrdinalIgnoreCase);
        }

        public static IEnumerable<string> FindUnknownTokens(string actionCode, params string[] contents)
        {
            var allowed = GetAllowedTokens(actionCode);
            var found = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var content in contents ?? Array.Empty<string>())
            {
                if (string.IsNullOrEmpty(content))
                {
                    continue;
                }

                foreach (System.Text.RegularExpressions.Match match in NotificationTemplateTokenParser.TokenRegex.Matches(content))
                {
                    var token = match.Groups[1].Value;
                    if (!allowed.Contains(token))
                    {
                        found.Add(token);
                    }
                }
            }

            return found.OrderBy(t => t, StringComparer.OrdinalIgnoreCase);
        }

        private static PlaceholderDef Common(string snakeName)
        {
            return PlaceholderCatalog.All[snakeName];
        }

        private sealed class PlaceholderDef
        {
            public string Token { get; set; }
            public string LabelEn { get; set; }
            public string LabelAr { get; set; }
            public string SampleValue { get; set; }
        }

        private static class PlaceholderCatalog
        {
            public static readonly IReadOnlyDictionary<string, PlaceholderDef> All =
                new Dictionary<string, PlaceholderDef>(StringComparer.OrdinalIgnoreCase)
                {
                    ["jobseeker_name"] = Def("jobseeker_name", "Jobseeker name", "اسم الباحث عن عمل", "Ahmad Al-Rashid"),
                    ["job_title"] = Def("job_title", "Job title", "المسمى الوظيفي", "Software Engineer"),
                    ["company_name"] = Def("company_name", "Company name", "اسم الشركة", "Acme Corp"),
                    ["operator_name"] = Def("operator_name", "Operator name", "اسم المشغل", "HR Team"),
                    ["recipient_name"] = Def("recipient_name", "Recipient name", "اسم المستلم", "HR Team"),
                    ["response_status"] = Def("response_status", "Offer response status", "حالة رد العرض", "Accepted"),
                    ["inquiry_preview"] = Def("inquiry_preview", "Inquiry message preview", "معاينة رسالة الاستفسار", "Could you clarify the start date?"),
                    ["application_date"] = Def("application_date", "Application date", "تاريخ التقديم", "2026-07-11"),
                    ["application_status"] = Def("application_status", "Application status", "حالة الطلب", "Shortlisted"),
                    ["interview_date"] = Def("interview_date", "Interview date", "تاريخ المقابلة", "2026-07-15 10:00"),
                    ["interview_location"] = Def("interview_location", "Interview location", "مكان المقابلة", "Riyadh Office"),
                    ["portal_link"] = Def("portal_link", "Portal link", "رابط البوابة", "https://portal.example.com/jobs"),
                    ["opportunity_title"] = Def("opportunity_title", "Opportunity title", "عنوان الفرصة", "Software Engineer"),
                    ["interview_time"] = Def("interview_time", "Interview time", "وقت المقابلة", "10:00 AM"),
                    ["location_or_link"] = Def("location_or_link", "Location or meeting link", "المكان أو رابط الاجتماع", "Riyadh Office"),
                    ["instructions"] = Def("instructions", "Instructions", "التعليمات", "Please arrive 15 minutes early."),
                    ["verification_link"] = Def("verification_link", "Verification link", "رابط التحقق", "https://portal.example.com/verify"),
                    ["evaluation_title"] = Def("evaluation_title", "Evaluation title", "عنوان التقييم", "Technical assessment"),
                    ["assessment_date"] = Def("assessment_date", "Assessment date", "تاريخ التقييم", "2026-07-15 10:00"),
                };

            private static PlaceholderDef Def(string token, string en, string ar, string sample)
            {
                return new PlaceholderDef
                {
                    Token = token,
                    LabelEn = en,
                    LabelAr = ar,
                    SampleValue = sample,
                };
            }
        }
    }
}

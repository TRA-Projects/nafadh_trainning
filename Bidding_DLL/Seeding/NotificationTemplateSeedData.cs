using System;
using System.Collections.Generic;

namespace Bidding_DLL.Seeding
{
    public static class NotificationTemplateSeedData
    {
        public sealed class SeedRow
        {
            public string ActionCode { get; set; }
            public string LabelEn { get; set; }
            public string LabelAr { get; set; }
            public string EmailSubjectEn { get; set; }
            public string EmailSubjectAr { get; set; }
            public string EmailBodyEn { get; set; }
            public string EmailBodyAr { get; set; }
            public string SmsBodyEn { get; set; }
            public string SmsBodyAr { get; set; }
            public int SortOrder { get; set; }
        }

        public static IReadOnlyList<SeedRow> Templates { get; } = new List<SeedRow>
        {
            new SeedRow
            {
                ActionCode = "ASK_TO_APPLY",
                LabelEn = "Ask to apply",
                LabelAr = "دعوة للتقديم",
                EmailSubjectEn = "You are invited to apply for {job_title}",
                EmailSubjectAr = "دعوة للتقديم على {job_title}",
                EmailBodyEn = "<p>Dear {jobseeker_name},</p><p>{company_name} invites you to apply for <strong>{job_title}</strong>.</p><p><a href=\"{portal_link}\">View opportunity</a></p>",
                EmailBodyAr = "<p>عزيزي {jobseeker_name}،</p><p>تدعوك {company_name} للتقديم على <strong>{job_title}</strong>.</p><p><a href=\"{portal_link}\">عرض الفرصة</a></p>",
                SmsBodyEn = "Dear {jobseeker_name}, you are invited to apply for {job_title}. {portal_link}",
                SmsBodyAr = "عزيزي {jobseeker_name}، تمت دعوتك للتقديم على {job_title}. {portal_link}",
                SortOrder = 1,
            },
            new SeedRow
            {
                ActionCode = "JOB_APPLIED",
                LabelEn = "Job application submitted",
                LabelAr = "تم تقديم الطلب",
                EmailSubjectEn = "Application received for {job_title}",
                EmailSubjectAr = "تم استلام طلبك على {job_title}",
                EmailBodyEn = "<p>Dear {jobseeker_name},</p><p>Your application for <strong>{job_title}</strong> at {company_name} was received on {application_date}.</p>",
                EmailBodyAr = "<p>عزيزي {jobseeker_name}،</p><p>تم استلام طلبك على <strong>{job_title}</strong> لدى {company_name} بتاريخ {application_date}.</p>",
                SmsBodyEn = "Your application for {job_title} at {company_name} was received.",
                SmsBodyAr = "تم استلام طلبك على {job_title} لدى {company_name}.",
                SortOrder = 2,
            },
            new SeedRow
            {
                ActionCode = "APPLICATION_STATUS_UPDATED",
                LabelEn = "Application status updated",
                LabelAr = "تحديث حالة الطلب",
                EmailSubjectEn = "Application status update — {job_title}",
                EmailSubjectAr = "تحديث حالة الطلب — {job_title}",
                EmailBodyEn = "<p>Dear {jobseeker_name},</p><p>Your application for {job_title} at {company_name} is now: <strong>{application_status}</strong>.</p><p><a href=\"{portal_link}\">View application</a></p>",
                EmailBodyAr = "<p>عزيزي {jobseeker_name}،</p><p>حالة طلبك على {job_title} لدى {company_name}: <strong>{application_status}</strong>.</p><p><a href=\"{portal_link}\">عرض الطلب</a></p>",
                SmsBodyEn = "Status update for {job_title}: {application_status}. {portal_link}",
                SmsBodyAr = "تحديث حالة {job_title}: {application_status}. {portal_link}",
                SortOrder = 3,
            },
            new SeedRow
            {
                ActionCode = "INTERVIEW_SCHEDULED",
                LabelEn = "Interview scheduled",
                LabelAr = "جدولة مقابلة",
                EmailSubjectEn = "Interview scheduled for {job_title}",
                EmailSubjectAr = "تم جدولة مقابلة لـ {job_title}",
                EmailBodyEn = "<p>Dear {jobseeker_name},</p><p>Your interview for {job_title} at {company_name} is scheduled on {interview_date} at {interview_location}.</p>",
                EmailBodyAr = "<p>عزيزي {jobseeker_name}،</p><p>مقابلتك لـ {job_title} لدى {company_name} بتاريخ {interview_date} في {interview_location}.</p>",
                SmsBodyEn = "Interview for {job_title} on {interview_date} at {interview_location}.",
                SmsBodyAr = "مقابلة {job_title} بتاريخ {interview_date} في {interview_location}.",
                SortOrder = 4,
            },
            new SeedRow
            {
                ActionCode = "OFFER_SUBMITTED",
                LabelEn = "Offer submitted",
                LabelAr = "تقديم عرض",
                EmailSubjectEn = "Job offer for {job_title}",
                EmailSubjectAr = "عرض وظيفي لـ {job_title}",
                EmailBodyEn = "<p>Dear {jobseeker_name},</p><p>{company_name} has submitted an offer for <strong>{job_title}</strong>.</p><p><a href=\"{portal_link}\">Review offer</a></p>",
                EmailBodyAr = "<p>عزيزي {jobseeker_name}،</p><p>قدمت {company_name} عرضاً لـ <strong>{job_title}</strong>.</p><p><a href=\"{portal_link}\">مراجعة العرض</a></p>",
                SmsBodyEn = "You have a job offer for {job_title} from {company_name}. {portal_link}",
                SmsBodyAr = "لديك عرض وظيفي لـ {job_title} من {company_name}. {portal_link}",
                SortOrder = 5,
            },
            new SeedRow
            {
                ActionCode = "OFFER_REISSUED",
                LabelEn = "Offer re-issued",
                LabelAr = "إعادة إصدار العرض",
                EmailSubjectEn = "Revised offer for {job_title}",
                EmailSubjectAr = "عرض معدّل لـ {job_title}",
                EmailBodyEn = "<p>Dear {jobseeker_name},</p><p>{company_name} has re-issued a revised offer for <strong>{job_title}</strong>.</p><p><a href=\"{portal_link}\">Review offer</a></p>",
                EmailBodyAr = "<p>عزيزي {jobseeker_name}،</p><p>أعادت {company_name} إصدار عرض معدّل لـ <strong>{job_title}</strong>.</p><p><a href=\"{portal_link}\">مراجعة العرض</a></p>",
                SmsBodyEn = "Revised offer for {job_title} from {company_name}. {portal_link}",
                SmsBodyAr = "عرض معدّل لـ {job_title} من {company_name}. {portal_link}",
                SortOrder = 11,
            },
            new SeedRow
            {
                ActionCode = "OFFER_CANDIDATE_RESPONSE",
                LabelEn = "Offer candidate response",
                LabelAr = "رد المرشح على العرض",
                EmailSubjectEn = "Offer update — {jobseeker_name} ({response_status})",
                EmailSubjectAr = "تحديث العرض — {jobseeker_name} ({response_status})",
                EmailBodyEn = "<p>Dear {operator_name},</p><p>{jobseeker_name} responded to the offer for <strong>{job_title}</strong> at {company_name}.</p><p><strong>Status:</strong> {response_status}</p><p><a href=\"{portal_link}\">View offer</a></p>",
                EmailBodyAr = "<p>عزيزي {operator_name}،</p><p>ردّ {jobseeker_name} على عرض <strong>{job_title}</strong> لدى {company_name}.</p><p><strong>الحالة:</strong> {response_status}</p><p><a href=\"{portal_link}\">عرض التفاصيل</a></p>",
                SmsBodyEn = "{jobseeker_name} responded to {job_title}: {response_status}. {portal_link}",
                SmsBodyAr = "ردّ {jobseeker_name} على {job_title}: {response_status}. {portal_link}",
                SortOrder = 12,
            },
            new SeedRow
            {
                ActionCode = "OFFER_INQUIRY_RECEIVED",
                LabelEn = "Offer inquiry received",
                LabelAr = "استفسار بخصوص العرض",
                EmailSubjectEn = "New inquiry about {job_title}",
                EmailSubjectAr = "استفسار جديد بخصوص {job_title}",
                EmailBodyEn = "<p>Dear {recipient_name},</p><p>There is a new inquiry about the offer for <strong>{job_title}</strong> ({company_name}).</p><p>{inquiry_preview}</p><p><a href=\"{portal_link}\">Open conversation</a></p>",
                EmailBodyAr = "<p>عزيزي {recipient_name}،</p><p>هناك استفسار جديد بخصوص عرض <strong>{job_title}</strong> ({company_name}).</p><p>{inquiry_preview}</p><p><a href=\"{portal_link}\">فتح المحادثة</a></p>",
                SmsBodyEn = "New inquiry about {job_title}. {portal_link}",
                SmsBodyAr = "استفسار جديد بخصوص {job_title}. {portal_link}",
                SortOrder = 13,
            },
            new SeedRow
            {
                ActionCode = "OP_INTERVIEW_INVITATION",
                LabelEn = "Operator interview invitation",
                LabelAr = "دعوة مقابلة (المشغّل)",
                EmailSubjectEn = "Interview invitation — {opportunity_title}",
                EmailSubjectAr = "دعوة مقابلة — {opportunity_title}",
                EmailBodyEn = "<p>Dear {jobseeker_name},</p><p>{company_name} invites you to an interview for <strong>{opportunity_title}</strong>.</p><p><strong>Date:</strong> {interview_date}<br/><strong>Time:</strong> {interview_time}<br/><strong>Location / link:</strong> {location_or_link}</p><p>{instructions}</p><p><a href=\"{portal_link}\">View interview details</a></p>",
                EmailBodyAr = "<p>عزيزي {jobseeker_name}،</p><p>تدعوك {company_name} لإجراء مقابلة لـ <strong>{opportunity_title}</strong>.</p><p><strong>التاريخ:</strong> {interview_date}<br/><strong>الوقت:</strong> {interview_time}<br/><strong>المكان / الرابط:</strong> {location_or_link}</p><p>{instructions}</p><p><a href=\"{portal_link}\">عرض تفاصيل المقابلة</a></p>",
                SmsBodyEn = "Interview invitation from {company_name} for {opportunity_title} on {interview_date} at {interview_time}. {portal_link}",
                SmsBodyAr = "دعوة مقابلة من {company_name} لـ {opportunity_title} بتاريخ {interview_date} الساعة {interview_time}. {portal_link}",
                SortOrder = 6,
            },
            new SeedRow
            {
                ActionCode = "OP_UPD_INTERVIEW",
                LabelEn = "Operator interview update",
                LabelAr = "تحديث مقابلة (المشغّل)",
                EmailSubjectEn = "Updated interview — {opportunity_title}",
                EmailSubjectAr = "تحديث موعد المقابلة — {opportunity_title}",
                EmailBodyEn = "<p>Dear {jobseeker_name},</p><p>Your interview with {company_name} for <strong>{opportunity_title}</strong> has been updated.</p><p><strong>Date:</strong> {interview_date}<br/><strong>Time:</strong> {interview_time}<br/><strong>Location / link:</strong> {location_or_link}</p><p>{instructions}</p><p><a href=\"{portal_link}\">View interview details</a></p>",
                EmailBodyAr = "<p>عزيزي {jobseeker_name}،</p><p>تم تحديث موعد مقابلتك مع {company_name} لـ <strong>{opportunity_title}</strong>.</p><p><strong>التاريخ:</strong> {interview_date}<br/><strong>الوقت:</strong> {interview_time}<br/><strong>المكان / الرابط:</strong> {location_or_link}</p><p>{instructions}</p><p><a href=\"{portal_link}\">عرض تفاصيل المقابلة</a></p>",
                SmsBodyEn = "Interview update from {company_name} for {opportunity_title}: {interview_date} at {interview_time}. {portal_link}",
                SmsBodyAr = "تحديث مقابلة من {company_name} لـ {opportunity_title}: {interview_date} الساعة {interview_time}. {portal_link}",
                SortOrder = 7,
            },
            new SeedRow
            {
                ActionCode = "REGISTRATION_EMAIL_VERIFICATION",
                LabelEn = "Registration email verification",
                LabelAr = "التحقق من البريد بعد التسجيل",
                EmailSubjectEn = "Verify your email address",
                EmailSubjectAr = "تحقق من بريدك الإلكتروني",
                EmailBodyEn = "<p>Dear {jobseeker_name},</p><p>Thank you for registering. Please click the link below to verify your email:</p><p><a href=\"{verification_link}\">Verify email</a></p><p>If you did not request this, please ignore this message.</p>",
                EmailBodyAr = "<p>عزيزي {jobseeker_name}،</p><p>شكراً لتسجيلك. يرجى النقر على الرابط أدناه للتحقق من بريدك الإلكتروني:</p><p><a href=\"{verification_link}\">تحقق من البريد</a></p><p>إذا لم تطلب ذلك، يرجى تجاهل هذه الرسالة.</p>",
                SmsBodyEn = "Verify your email: {verification_link}",
                SmsBodyAr = "تحقق من بريدك: {verification_link}",
                SortOrder = 8,
            },
            new SeedRow
            {
                ActionCode = "EVALUATION_INVITATION",
                LabelEn = "Evaluation invitation",
                LabelAr = "دعوة التقييم",
                EmailSubjectEn = "Evaluation invitation — {evaluation_title}",
                EmailSubjectAr = "دعوة تقييم — {evaluation_title}",
                EmailBodyEn = "<p>Dear {jobseeker_name},</p><p>You are invited to complete <strong>{evaluation_title}</strong>.</p><p><strong>Date:</strong> {assessment_date}</p><p>{instructions}</p><p><a href=\"{portal_link}\">View assessment details</a></p>",
                EmailBodyAr = "<p>عزيزي {jobseeker_name}،</p><p>تمت دعوتك لإجراء <strong>{evaluation_title}</strong>.</p><p><strong>التاريخ:</strong> {assessment_date}</p><p>{instructions}</p><p><a href=\"{portal_link}\">عرض تفاصيل التقييم</a></p>",
                SmsBodyEn = "Evaluation invitation: {evaluation_title} on {assessment_date}. {portal_link}",
                SmsBodyAr = "دعوة تقييم: {evaluation_title} بتاريخ {assessment_date}. {portal_link}",
                SortOrder = 9,
            },
            new SeedRow
            {
                ActionCode = "EVALUATION_UPDATE",
                LabelEn = "Evaluation schedule update",
                LabelAr = "تحديث تفاصيل التقييم",
                EmailSubjectEn = "Evaluation details updated — {evaluation_title}",
                EmailSubjectAr = "تم تحديث تفاصيل التقييم — {evaluation_title}",
                EmailBodyEn = "<p>Dear {jobseeker_name},</p><p>The details for <strong>{evaluation_title}</strong> have been updated.</p><p><strong>Date:</strong> {assessment_date}</p><p>{instructions}</p><p><a href=\"{portal_link}\">View assessment details</a></p>",
                EmailBodyAr = "<p>عزيزي {jobseeker_name}،</p><p>تم تحديث تفاصيل <strong>{evaluation_title}</strong>.</p><p><strong>التاريخ:</strong> {assessment_date}</p><p>{instructions}</p><p><a href=\"{portal_link}\">عرض تفاصيل التقييم</a></p>",
                SmsBodyEn = "Evaluation updated: {evaluation_title} on {assessment_date}. {portal_link}",
                SmsBodyAr = "تحديث تقييم: {evaluation_title} بتاريخ {assessment_date}. {portal_link}",
                SortOrder = 10,
            },
        };
    }
}

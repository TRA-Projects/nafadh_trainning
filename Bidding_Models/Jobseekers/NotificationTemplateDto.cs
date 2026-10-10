using System.Collections.Generic;

namespace Bidding_Models.Jobseekers
{
    public class NotificationTemplateListItemDto
    {
        public int Id { get; set; }
        public string ActionCode { get; set; }
        public string LabelEn { get; set; }
        public string LabelAr { get; set; }
        public bool IsActive { get; set; }
        public bool EmailEnabled { get; set; }
        public bool SmsEnabled { get; set; }
        public int SortOrder { get; set; }
    }

    public class NotificationTemplateDto : NotificationTemplateListItemDto
    {
        public string EmailSubjectEn { get; set; }
        public string EmailSubjectAr { get; set; }
        public string EmailBodyEn { get; set; }
        public string EmailBodyAr { get; set; }
        public string SmsBodyEn { get; set; }
        public string SmsBodyAr { get; set; }
    }

    public class UpdateNotificationTemplateRequestDto
    {
        public string LabelEn { get; set; }
        public string LabelAr { get; set; }
        public string EmailSubjectEn { get; set; }
        public string EmailSubjectAr { get; set; }
        public string EmailBodyEn { get; set; }
        public string EmailBodyAr { get; set; }
        public string SmsBodyEn { get; set; }
        public string SmsBodyAr { get; set; }
        public bool IsActive { get; set; } = true;
        public bool EmailEnabled { get; set; } = true;
        public bool SmsEnabled { get; set; } = true;
    }

    public class NotificationPlaceholderDto
    {
        public string Token { get; set; }
        public string LabelEn { get; set; }
        public string LabelAr { get; set; }
        public string SampleValue { get; set; }
    }

    public class NotificationTemplatePlaceholdersDto
    {
        public string ActionCode { get; set; }
        public List<NotificationPlaceholderDto> Placeholders { get; set; } = new List<NotificationPlaceholderDto>();
    }

    public class RenderedNotificationChannelDto
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class RenderedNotificationDto
    {
        public string ActionCode { get; set; }
        public bool IsActive { get; set; }
        public RenderedNotificationChannelDto Email { get; set; }
        public RenderedNotificationChannelDto Sms { get; set; }
    }
}

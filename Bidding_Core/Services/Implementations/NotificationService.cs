using Bidding_Core.Services.Interfaces;
using Bidding_Models.DatabaseModel;
using System.Threading.Tasks;
using ConstructionHub_Core.Helpers;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Bidding_Core.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly Email _email;
        private static string _emailTemplateCache;
        private static readonly object _lock = new object();
        private const string TemplatePath = "../Bidding_API/wwwroot/EmailTemplates/EmailTemplate.html";

        public NotificationService(EmailSettings emailSettings)
        {
            _email = new Email(emailSettings);
        }

        private string GetEmailTemplate()
        {
            if (_emailTemplateCache == null)
            {
                lock (_lock)
                {
                    if (_emailTemplateCache == null)
                    {
                        _emailTemplateCache = File.ReadAllText(TemplatePath);
                    }
                }
            }
            return _emailTemplateCache;
        }

        private string BuildBody(string mainContent)
        {
            var template = GetEmailTemplate();
            return template.Replace("[[MainContent]]", mainContent);
        }

        public async Task NotifyApplicationSubmitted(ProfileProject projectDetails)
        {
            string subject = $"Application Submitted for Project: {projectDetails.ProjectTitle}";
            string body = BuildBody($"Your application for project <b>{projectDetails.ProjectTitle}</b> has been submitted successfully.");
            await _email.SendEmailAsync(subject, body, projectDetails.Profile?.EmailAddress);
        }

        public async Task NotifyContractCreated(ProfileProject projectDetails)
        {
            string subject = $"Contract Created for Project: {projectDetails.ProjectTitle}";
            string body = BuildBody($"A new contract has been created for your project <b>{projectDetails.ProjectTitle}</b>.");
            await _email.SendEmailAsync(subject, body, projectDetails.Profile?.EmailAddress);
        }

        public async Task NotifyContractSignedOrRejected(ProfileProject projectDetails, bool isRejected)
        {
            string subject = isRejected ? $"Contract Rejected for Project: {projectDetails.ProjectTitle}" : $"Contract Signed for Project: {projectDetails.ProjectTitle}";
            string body = BuildBody(isRejected
                ? $"The contract for project <b>{projectDetails.ProjectTitle}</b> has been rejected."
                : $"The contract for project <b>{projectDetails.ProjectTitle}</b> has been signed successfully.");
            await _email.SendEmailAsync(subject, body, projectDetails.Profile?.EmailAddress);
        }

        public async Task NotifyMilestoneApprovedOrRejected(ProfileProject projectDetails, bool isRejected)
        {
            string subject = isRejected ? $"Milestone Rejected for Project: {projectDetails.ProjectTitle}" : $"Milestone Approved for Project: {projectDetails.ProjectTitle}";
            string body = BuildBody(isRejected
                ? $"A milestone for project <b>{projectDetails.ProjectTitle}</b> has been rejected."
                : $"A milestone for project <b>{projectDetails.ProjectTitle}</b> has been approved.");
            await _email.SendEmailAsync(subject, body, projectDetails.Profile?.EmailAddress);
        }

        public async Task NotifyMilestoneSubmitted(ProfileProject projectDetails)
        {
            string subject = $"Milestone Submitted for Project: {projectDetails.ProjectTitle}";
            string body = BuildBody($"A new milestone has been submitted for your project <b>{projectDetails.ProjectTitle}</b>.");
            await _email.SendEmailAsync(subject, body, projectDetails.Profile?.EmailAddress);
        }

        public async Task NotifyPayment(ProfileProject projectDetails)
        {
            string subject = $"Payment Received for Project: {projectDetails.ProjectTitle}";
            string body = BuildBody($"A payment has been received for your project <b>{projectDetails.ProjectTitle}</b>.");
            await _email.SendEmailAsync(subject, body, projectDetails.Profile?.EmailAddress);
        }

        public async Task NotifyPostingNewProject(ProfileProject projectDetails)
        {
            string subject = $"New Project Posted";
            string body = BuildBody($"Your new project <b>{projectDetails.ProjectTitle}</b> has been posted successfully.");
            await _email.SendEmailAsync(subject, body, projectDetails.Profile?.EmailAddress);
        }

        public async Task NotifyPublishingNewProject(ProfileProject projectDetails)
        {
            string subject = $"Project Published";
            string body = BuildBody($"Your project <b>{projectDetails.ProjectTitle}</b> has been published and is now visible to freelancers.");
            await _email.SendEmailAsync(subject, body, projectDetails.Profile?.EmailAddress);
        }
    }
}
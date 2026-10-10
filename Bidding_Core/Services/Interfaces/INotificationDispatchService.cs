using System.Collections.Generic;
using System.Threading.Tasks;
using Bidding_Models.DatabaseModel;
using Bidding_Models.Jobseekers;

namespace Bidding_Core.Services.Interfaces
{
    public interface INotificationDispatchService
    {
        Task<bool> SendRegistrationEmailVerificationAsync(AppUser user);

        /// <summary>
        /// Re-sends the exact payload stored on a prior notification transaction log row
        /// and writes a new log entry for the attempt.
        /// </summary>
        Task<NotificationTransactionLogRowDto> ResendFromLogAsync(long logId);

        /// <summary>Sends one custom-notification email and adds a log row (caller saves).</summary>
        Task<bool> SendCustomEmailAsync(string receiver, string subject, string htmlBody, string locale, int? profileId, long customNotificationId);

        /// <summary>Sends one custom-notification SMS and adds a log row (caller saves).</summary>
        Task<bool> SendCustomSmsAsync(string phone, string body, string locale, int? profileId, long customNotificationId);

        /// <summary>Adds a failed log row for a channel that could not reach the recipient (caller saves).</summary>
        void LogCustomSkipped(string channelType, string receiver, string locale, int? profileId, long customNotificationId, string reason);
    }
}

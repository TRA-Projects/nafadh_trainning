using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bidding_Models.Jobseekers;

namespace Bidding_Core.Services.Interfaces
{
    public interface ICustomNotificationService
    {
        Task<PagedListResponseDto<CustomNotificationRowDto>> GetPageAsync(
            int? pageIndex,
            int? pageSize,
            string q,
            string status,
            string channel);

        Task<CustomNotificationDto> GetAsync(long id);

        Task<CustomNotificationDto> CreateAsync(UpsertCustomNotificationRequestDto body, string callerUserId);

        Task<CustomNotificationDto> UpdateAsync(long id, UpsertCustomNotificationRequestDto body);

        Task<CustomNotificationDto> CancelAsync(long id);

        Task<CustomNotificationRecipientPreviewDto> PreviewRecipientsAsync(IEnumerable<CustomNotificationTargetDto> targets);

        /// <summary>Sends every scheduled notification whose time has come. Returns how many were processed.</summary>
        Task<int> ProcessDueAsync(DateTime utcNow);

        Task<PagedListResponseDto<UserNotificationDto>> GetMyNotificationsAsync(
            string userId,
            int? pageIndex,
            int? pageSize,
            bool unreadOnly);

        Task<int> GetMyUnreadCountAsync(string userId);

        Task<UserNotificationDto> MarkReadAsync(string userId, long id);

        Task<int> MarkAllReadAsync(string userId);
    }
}

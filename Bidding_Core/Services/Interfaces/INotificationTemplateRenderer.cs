using System.Collections.Generic;
using System.Threading.Tasks;
using Bidding_Models.Jobseekers;

namespace Bidding_Core.Services.Interfaces
{
    public interface INotificationTemplateRenderer
    {
        Task<RenderedNotificationDto> RenderAsync(
            string actionCode,
            string locale,
            IReadOnlyDictionary<string, string> values);

        void InvalidateCache(string actionCode = null);
    }
}

using Bidding_Models.DatabaseModel;
using System.Threading.Tasks;

namespace Bidding_Core.Services.Interfaces
{
    public interface INotificationService
    {
        Task NotifyPostingNewProject(ProfileProject projectDetails);
        Task NotifyPublishingNewProject(ProfileProject projectDetails);
        Task NotifyApplicationSubmitted(ProfileProject projectDetails);
        Task NotifyContractCreated(ProfileProject projectDetails);
        Task NotifyContractSignedOrRejected(ProfileProject projectDetails, bool isRejected);
        Task NotifyMilestoneSubmitted(ProfileProject projectDetails);
        Task NotifyMilestoneApprovedOrRejected(ProfileProject projectDetails, bool isRejected);
        Task NotifyPayment(ProfileProject projectDetails);

    }

}
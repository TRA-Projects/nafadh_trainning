using System.Threading;
using System.Threading.Tasks;
using Bidding_Models.DTOs;

namespace Bidding_Core.Services.Interfaces
{
    public interface INafathJobSeekerService
    {
        /// <summary>When false, registration must not gate on this integration.</summary>
        Task<bool> IsRegistrationCheckEnabledAsync();

        /// <summary>When true, return fixtures instead of calling the remote host.</summary>
        Task<bool> IsUseMockAsync();

        /// <summary>
        /// Fetch Nafath JobSeeker details for a civil ID.
        /// Logs request/response. Never throws for remote failures — check <see cref="NafathJobSeekerFetchResult.Success"/>.
        /// </summary>
        Task<NafathJobSeekerFetchResult> GetDetailsByCivilIdAsync(string civilId, CancellationToken cancellationToken = default);

        /// <summary>Map Nafath education into MOL profile summary using configured degree ranks.</summary>
        Task ApplyEducationToMolSummaryAsync(FreelancerProfileFromMolDto dto, NafathJobSeekerDetailsDto data);
    }
}

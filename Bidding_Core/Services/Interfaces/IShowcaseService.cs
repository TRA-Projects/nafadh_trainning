using Bidding_Models.Jobseekers;

namespace Bidding_Core.Services.Interfaces
{
    public interface IShowcaseService
    {
        PagedListResponseDto<ShowcaseItemDto> GetPage(int? pageIndex, int? pageSize, string q, string category, string status);
        ShowcaseItemDto Get(long id);
        ShowcaseItemDto Create(UpsertShowcaseItemRequestDto body);
        ShowcaseItemDto Update(long id, UpsertShowcaseItemRequestDto body);
        void Delete(long id);
    }
}

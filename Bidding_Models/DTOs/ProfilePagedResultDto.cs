using System.Collections.Generic;
using Bidding_Models.DatabaseModel;

namespace Bidding_Models.DTOs
{
    public class ProfilePagedResultDto
    {
        public List<Profile> Items { get; set; } = new List<Profile>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}

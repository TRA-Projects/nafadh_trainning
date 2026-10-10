using System.Collections.Generic;

namespace Bidding_Models.DTOs
{
    public class CreateOrUpdateEndorsementRequestDto
    {
        public int ProjectId { get; set; }
        public List<string> Skills { get; set; }
    }
}
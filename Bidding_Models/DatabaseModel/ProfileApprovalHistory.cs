using System;
using System.Collections.Generic;
using System.Text;
using static Bidding_Models.Enums;

namespace Bidding_Models.DatabaseModel
{
    public class ProfileApprovalHistory
    {
        public int Id { get; set; }
        public Profile Profile { get; set; }

        public int ProfileId { get; set; }

        public ProfileStatus Status { get; set; }

        public string Comments { get; set; }

        public AppUser ActionBy { get; set; }
        public string ActionById { get; set; }
        public DateTime ActionDate { get; set; }

        public DateTime? LastSubmittedDate { get; set; }

    }
}

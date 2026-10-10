using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("ProfileWorkScopes")]
    public class ProfileWorkScope
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        /// <summary>Numeric id from <c>WRK_SCOPE</c> config values (frontend <c>ServicesOffered[].Id</c>).</summary>
        public int WorkScopeId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

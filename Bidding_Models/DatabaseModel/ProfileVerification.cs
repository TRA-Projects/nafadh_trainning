using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class ProfileVerification
    {
        public int Id { get; set; }

        public Profile Profile { get; set; }
        public int ProfileId { get; set; }


        [NotMapped]
        public PagedResult<ProfileVerification> PagedResult { get; set; }

        [NotMapped]
        public string EncryptedProfileId { get; set; }
    }
}

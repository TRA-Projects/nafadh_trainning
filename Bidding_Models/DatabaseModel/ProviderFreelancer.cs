using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class ProviderFreelancer
    {

        public int Id { get; set; }
        [ForeignKey(nameof(Providers))]
        public int ProviderId { get; set; }
        public Providers Providers { get; set; }


        public string PublicKey { get; set; }
        public string SecretKey { get; set; }

        public Profile Profile { get; set; }
        public int ProfileId { get; set; }

        public bool IsActive { get; set; }






    }
}

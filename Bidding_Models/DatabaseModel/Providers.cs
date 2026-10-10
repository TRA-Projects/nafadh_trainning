using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class Providers 
    {
       
        public int Id { get; set; }
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public bool IsActive { get; set; }

        public List<ProviderFreelancer> ProviderFreelancer { get; set; }
    }
}

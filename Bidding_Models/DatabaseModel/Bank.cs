using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class Bank 
    {
       
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public List<AccountDetails> AccountDetails { get; set; }
    }
}

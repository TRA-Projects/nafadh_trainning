using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
	[Owned]
   public class Capital
    {
		public string CashCapital { get; set; }
		public string AssetCapital { get; set; }
		public string TotalCapital { get; set; }
		public string ShareValue { get; set; }
		public string TotalShares { get; set; }
		public string CapitalVerified { get; set; }
	}
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
	[Owned]
   public class RegistrationStatus { 
		public string NameAr { get; set; }
		public string NameEn { get; set; }
		public string Code { get; set; }
	}
}

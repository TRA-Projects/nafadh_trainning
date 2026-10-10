using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
	[Owned]
   public class Contact
    {
		public string Phone { get; set; }
		public string Fax { get; set; }
		public string Email { get; set; }
		public string Mobile { get; set; }
	}
}

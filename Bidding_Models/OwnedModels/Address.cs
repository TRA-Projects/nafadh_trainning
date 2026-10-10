using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{

	[Owned]
  public  class Address
    {
        public Address()
        {
			Town = new AddressComponent();
			State = new AddressComponent();
			Governorate = new AddressComponent();
		}
		public AddressComponent Town { get; set; }
		public AddressComponent State { get; set; }
		public AddressComponent Governorate { get; set; }
		public string StreetNameEn { get; set; }
		public string BuildingNumber { get; set; }
		public string PostalCode { get; set; }
		public string PoBox { get; set; }
		public string Latitude { get; set; }
		public string Longitude { get; set; }
	}

	[Owned]
	public class AddressComponent
    {
		public string NameAr { get; set; }
		public string NameEn { get; set; }
		public string SpecialZone { get; set; }
		public string Code { get; set; }

	}
}

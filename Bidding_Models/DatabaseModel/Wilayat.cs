using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class Wilayat
    {
        public Wilayat()
        {
            Governorate = new Governorate();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public Governorate Governorate { get; set; }
        public int GovernorateId { get; set; }

        public List<City> CityList { get; set; }

        public List<Request> RequestList { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class City 
    {
        public City()
        {
            Wilayat = new Wilayat();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public Wilayat Wilayat { get; set; }
        public int WilayatId { get; set; }

        public List<Request> RequestList { get; set; }
    }
}

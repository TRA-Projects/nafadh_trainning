using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class Governorate
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string ArabicName { get; set; }
        public List<Wilayat> WilayatList { get; set; }

        public List<Request> RequestList { get; set; }
    }
}

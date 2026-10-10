using System.Collections.Generic;

namespace Bidding_Models
{   
    public class ApexChartSubSerie
    {
        public string name { get; set; }
        public List<decimal> data { get; set; } = new List<decimal>();
    }
}

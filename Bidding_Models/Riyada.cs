using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
    public class Riyada
    {
        public string Result { get; set; }
        public string CRNumber { get; set; }

        public string SMEName { get; set; }

        public string SMETypeName { get; set; }

        public string RegistrationDate { get; set; }

        public string IsSMERegistered { get; set; }

        public string IsRiyadaCard { get; set; }

        public string CardExpiryDate { get; set; }


        public List<RiyadaActivities> Activity { get; set; }

        public List<RiyadaCRInvestor> CRInvestor { get; set; }

    }

    public class RiyadaActivities
    {

        public string ISIC { get; set; }

        public string Name { get; set; }
    }

    public class RiyadaCRInvestor
    {

        public string NationalID { get; set; }
        public string Name { get; set; }
        public string FullTimeRate { get; set; }
    }
}

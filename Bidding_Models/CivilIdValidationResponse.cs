using System;

namespace Bidding_Models
{
    public class CivilIdValidationResponse
    {
        public int? Status { get; set; }
        public string Message { get; set; }
        public FreelancerDataFromMol Data { get; set; }
    }
}





using Bidding_Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
    public class SystemAwardingOpportunity
    {
        public SystemAwardingOpportunity()
        {
          
        }
        public int Id { get; set; }
        public Request Request { get; set; }
        public string RequestId { get; set; }
   

        public string ParticipantsString { get; set; }
        public List<RequestParticipents> ParticipantsList { get; set; }
        public string AwardedParticipantsString { get; set; }

    }


}

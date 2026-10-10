using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
   public class Feedback
    {
        public int Id { get; set; }

        public string Comment { get; set; }
        public int Rating { get; set; }

        public int ServiceRating { get; set; }
       

        public bool Isfirsttime { get; set; }


        public bool IsExpacted { get; set; }

        public string NotExpactedReason { get; set; }
        public bool Issatisfy { get; set; }

        public string NotsatisfyReason { get; set; }

        public string Expectation { get; set; }

        public string Thoughts { get; set; }
        public string Improvment { get; set; }

        public string RecommendedLevel { get; set; }


        public AppUser FromUser { get; set; }
        public string FromUserId { get; set; }
        public AppUser ForUser { get; set; }
        public string ForUserId { get; set; }

       


        public Bid Bid { get; set; }
        public int? BidId { get; set; }


        [NotMapped]
        public string usertpe { get; set; }

        [NotMapped]
        public string EncryptedBidId { get; set; }

        [NotMapped]
        public string RequestName { get; set; }


    }
}

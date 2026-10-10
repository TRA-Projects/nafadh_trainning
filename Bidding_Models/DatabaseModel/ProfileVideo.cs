using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class ProfileVideo
    {
        public int Id { get; set; }

        public Profile Profile { get; set; }
        public int ProfileId { get; set; }

        public string VideoUrl { get; set; }

        public VideoCategory Category { get; set; }

        public string VideoName { get; set; }



    }
    public enum VideoCategory
    {
        All,
        Design,
        Code,
        Integration,
        Production
    } 

}

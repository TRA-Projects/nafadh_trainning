using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class UserSpecalization
    {
        public int Id { get; set; }
        public Profile Profile { get; set; }
        public int ProfileId { get; set; }
        public Specalization Specalization { get; set; }
        public int SpecalizationId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class Specalization : EntityTrail
    {
        public Specalization()
        {

        }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public List<UserSpecalization> UserSpecalizationList { get; set; }


    }
}

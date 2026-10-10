using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class Department : EntityTrail
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
        public Profile Profile { get; set; }
        public int ProfileId { get; set; }

        public List<Staff> StaffList { get; set; }

        public List<Profile> ProfileList { get; set; }

        public bool HiddenDepartment { get; set; }
    }
}

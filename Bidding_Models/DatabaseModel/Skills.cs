using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class Skills : EntityTrail
    {
        public Skills()
        {

        }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public List<UserSkills> UserSkillsList { get; set; }


    }
}

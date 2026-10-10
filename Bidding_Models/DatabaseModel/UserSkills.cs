using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class UserSkills
    {
        public int Id { get; set; }
        public AppUser User { get; set; }
        public string UserId { get; set; }
        public Skills Skill { get; set; }
        public int SkillId { get; set; }
        public int? ProfileProjectId { get; set; } 
    }
}

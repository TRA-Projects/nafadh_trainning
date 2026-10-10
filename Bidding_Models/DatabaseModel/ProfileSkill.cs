using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel
{
    public class ProfileSkill
    {
        public int Id { get; set; }

        [NotMapped]
        public int Slno { get; set; }
        public Profile Profile { get; set; }
        public int? ProfileId { get; set; }
        
        public string SkillName { get; set; }
        
        public string SkillLevel { get; set; }

        /// <summary>True when the skill was created from Nafath/MOL language data.</summary>
        public bool FromIntegration { get; set; }
    }
}
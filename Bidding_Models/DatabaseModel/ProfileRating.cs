using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DatabaseModel
{
    public class ProfileRating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SenderProfileId { get; set; }

        [Required]
        public int ReceiverProfileId { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Testimonial { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public Profile SenderProfile { get; set; }
        public Profile ReceiverProfile { get; set; }
        public ProfileProjectContract Contract { get; set; }
    }
    public class FreelancerEndorsement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SenderProfileId { get; set; }
        
        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int ReceiverProfileId { get; set; }

        public DateTime CreatedDate { get; set; }

        public Profile SenderProfile { get; set; }
        public Profile ReceiverProfile { get; set; }
        public ProfileProject Project { get; set; }
        public ICollection<FreelancerEndorsementSkill> Skills { get; set; }
    }
    public class FreelancerEndorsementSkill
    {
        public int FreelancerEndorsementId { get; set; }
        [Required]
        public string Skill { get; set; }
        public FreelancerEndorsement Endorsement { get; set; }
    }
}
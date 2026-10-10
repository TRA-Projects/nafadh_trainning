using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel
{
    public class ProjectPhase
    {
        
            [Key]
            public int Id { get; set; }

            [Required, MaxLength(100)]
            public string PhaseName { get; set; }

            [MaxLength(500)]
            public string PhaseDescription { get; set; }

            [Required]
            public decimal Budget { get; set; }

            [Required]
            public int ProjectId { get; set; }

            [ForeignKey("ProjectId")]
            public ProfileProject Project { get; set; }
        
    }
}
using Bidding_Models.DatabaseModel;
using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class ProfileVideoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Profile Id is required")]
        public int ProfileId { get; set; }
        [Required(ErrorMessage = "Video Name is required")]
        public string VideoName { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Video Link is required")]
        public string VideoLink { get; set; }
    }
}
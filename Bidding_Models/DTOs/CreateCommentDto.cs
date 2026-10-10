using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class CreateCommentDto
    {
        [Required(ErrorMessage = "Text is required")]
        public string Text { get; set; }
    }
}
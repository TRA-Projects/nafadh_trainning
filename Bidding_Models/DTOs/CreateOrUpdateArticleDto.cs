using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class CreateOrUpdateArticleDto
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Short text is required")]
        public string ShortText { get; set; }
        [Required(ErrorMessage = "Full text is required")]
        public string FullText { get; set; }
        [Required(ErrorMessage = "Image file name is required")]
        public string ImageFileName { get; set; }
        [Required(ErrorMessage = "Article category ID is required")]
        public int ArticleCategoryId { get; set; }
        public int? ProfileId { get; set; }
        public bool IsPublished { get; set; } = false;
        public string YoutubeVideo { get; set; }

    }
}
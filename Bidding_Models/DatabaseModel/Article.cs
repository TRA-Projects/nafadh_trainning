using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel
{
    public class Article
    {     
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; } = DateTime.Now;
        public string Title { get; set; }
        public string ShortText { get; set; }
        public string FullText { get; set; }
        public string ImageFileName { get; set; }
        public string YoutubeVideo { get; set; }
        public int ArticleCategoryId { get; set; }
        public int ProfileId { get; set; }        
        public bool IsPublished { get; set; } = false;
        public ArticleCategory ArticleCategory { get; set; }
        public Profile Profile { get; set; }
        public ICollection<LikedArticle> Likes { get; set; }
        public ICollection<FavoriteArticle> Favorites { get; set; }
        public ICollection<ArticleComment> Comments { get; set; }
        [NotMapped]
        public int NumberOfLikes { 
            get
            {
                return Likes?.Count ?? 0;
            }
        }
        [NotMapped]
        public int NumberOfComments
        {
            get
            {
                return Comments?.Count ?? 0;
            }
        }
        [NotMapped]
        public bool IsLiked { get; set; }
        [NotMapped]
        public bool IsFavorite { get; set; }
    }
    public class ArticleCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameArabic { get; set; }
    }
    public class LikedArticle
    {
        public int ProfileId { get; set; }
        public long ArticleId { get; set; }
    }
    public class FavoriteArticle
    {
        public int ProfileId { get; set; }
        public long ArticleId { get; set; }
        public Article Article { get; set; }
    }
    public class ArticleComment
    {
        public long Id { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public int AuthorId { get; set; }
        public string Text { get; set; }
        public long ArticleId { get; set; }
        public Profile Author { get; set; }
    }
}

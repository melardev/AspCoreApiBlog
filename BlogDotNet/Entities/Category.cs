using System;
using System.Collections.Generic;

namespace BlogDotNet.Entities
{
    public class Category
    {
        public Category()
        {
            ArticleCategories = new HashSet<ArticleCategory>();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt;
        public DateTime UpdatedAt;

        public ICollection<ArticleCategory> ArticleCategories { get; set; }
        //public ICollection<Article> Articles { get; set; }
    }
}
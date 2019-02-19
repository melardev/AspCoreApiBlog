using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogDotNet.Entities
{
    public class Tag
    {
        public Tag()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        [Required] public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        public ICollection<ArticleTag> ArticlesTags { get; set; }
        //public ICollection<Article> Articles { get; set; }
    }
}
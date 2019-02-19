using System;

namespace BlogDotNet.Entities
{
    public class Like
    {
        public DateTime CreatedAt { get; set; }
        public string Id { get; set; }
        public string ArticleId { get; set; }
        public string UserId { get; set; }

        public Article Article { get; set; }
        public ApplicationUser User { get; set; }
    }
}
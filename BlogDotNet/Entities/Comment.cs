using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogDotNet.Entities
{
    public class Comment
    {
        public Comment()
        {
            //CreatedAt = DateTime.Now;
        }

        public string Id { get; set; }
        public string Content { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Article Article { get; set; }
        public string ArticleId { get; set; }

        public Comment RepliedComment { get; set; }
        public string RepliedCommentId { get; set; }

        [NotMapped] public bool IsReply { get; set; }
        public virtual HashSet<Comment> Replies { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }


        public string RenderContent()
        {
            throw new NotImplementedException();
        }
    }
}
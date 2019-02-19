using System;
using BlogDotNet.Dtos.Responses.Article;
using BlogDotNet.Models.ViewModels.User;

namespace BlogDotNet.Dtos.Responses.Comment
{
    public class CommentDetailsDto : SuccessResponse
    {
        
        public ArticleIdSlugAndTitle Article { get; set; }
        public string Content { get; set; }

        public UserBasicEmbeddedInfoDto User { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // public string ArticleId { get; set; }
        // public string ArticleSlug { get; set; }
        public string RepliedCommentId { get; set; }

        public bool IsReply { get; set; }

        // public Comment Comment {get; set;}
        public static CommentDetailsDto Build(Entities.Comment comment)
        {
            return new CommentDetailsDto
            {
                // ArticleSlug = comment.Article.Slug,
                // ArticleId = comment.ArticleId,
                Article = new ArticleIdSlugAndTitle
                {
                    Id = comment.ArticleId,
                    Title = comment.Article.Title,
                    Slug = comment.Article.Slug,
                },
                Content = comment.Content,
                User = UserBasicEmbeddedInfoDto.Build(comment.User),
                IsReply = comment.RepliedComment != null,
                RepliedCommentId = comment.RepliedCommentId,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };
        }
    }
}
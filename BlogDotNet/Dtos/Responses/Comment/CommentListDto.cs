using System.Collections.Generic;
using BlogDotNet.Dtos.Responses.Article;
using BlogDotNet.Models;
using BlogDotNet.Models.ViewModels;
using BlogDotNet.Models.ViewModels.User;
using MySqlX.XDevAPI.Common;

namespace BlogDotNet.Dtos.Responses.Comment
{
    class CommentListDto : SuccessResponse
    {
        public bool Success { get; set; }
        public PageMeta PageMeta { get; set; }
        public ICollection<CommentDetailsDto> Comments { get; set; }

        public static CommentListDto Build(ICollection<Entities.Comment> comments,
            string basePath,
            int currentPage, int pageSize, int totalItemCount)
        {
            ICollection<CommentDetailsDto> result = new List<CommentDetailsDto>();
            foreach (var comment in comments)
            {
                result.Add(
                    new CommentDetailsDto
                    {
                        Article = new ArticleIdSlugAndTitle
                        {
                            Id = comment.ArticleId,
                            Title = comment.Article.Title,
                            Slug = comment.Article.Slug,
                        },
                        IsReply = comment.IsReply,
                        RepliedCommentId = comment.RepliedCommentId,
                        Content = comment.Content,
                        CreatedAt = comment.CreatedAt,
                        User = UserBasicEmbeddedInfoDto.Build(comment.User)
                    });
            }

            return new CommentListDto
            {
                Success = true,
                PageMeta = new PageMeta(result.Count, basePath, currentPage: currentPage, pageSize: pageSize,
                    totalItemCount: totalItemCount),
                Comments = result
            };
        }
    }
}
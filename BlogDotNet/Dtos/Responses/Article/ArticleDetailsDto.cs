using System;
using System.Collections.Generic;
using BlogDotNet.Dtos.Responses.Category;
using BlogDotNet.Dtos.Responses.Comment;
using BlogDotNet.Dtos.Responses.Tag;
using BlogDotNet.Models.ViewModels.User;

namespace BlogDotNet.Dtos.Responses.Article
{
    public class ArticleDetailsDto : SuccessResponse
    {
        public string Id { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public UserBasicEmbeddedInfoDto User { get; set; }

        public string Body { get; set; }

        public DateTime ModifiedAt { get; set; }
        public DateTime PublishedAt { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public List<string> Tags { get; set; }
        // public IEnumerable<CommentIntrinsicInfoDto> Comments { get; set; }
        public IEnumerable<CommentDetailsDto> Comments { get; set; }

        public static ArticleDetailsDto Build(Entities.Article article)
        {
            var commentDtos = new List<CommentDetailsDto>();
            foreach (var comment in article.Comments)
            {
                commentDtos.Add(CommentDetailsDto.Build(comment));
            }

            return new ArticleDetailsDto
            {
                Id = article.Id,
                Slug = article.Slug,
                Title = article.Slug,
                Body = article.Body,
                PublishedAt = article.PublishedOn,
                Comments = commentDtos,
                User = UserBasicEmbeddedInfoDto.Build(article.User),
                Tags = TagOnlyNameDto.BuildAsStringList(article.ArticlesTags),
                Categories = CategoryOnlyNameDto.BuildAsStringList(article.ArticleCategories),
            };
        }
    }
}
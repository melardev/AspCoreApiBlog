using System;
using System.Collections.Generic;
using BlogDotNet.Dtos.Responses.Category;
using BlogDotNet.Dtos.Responses.Tag;
using BlogDotNet.Entities;
using BlogDotNet.Models;
using BlogDotNet.Models.ViewModels.User;

namespace BlogDotNet.Dtos.Responses.Article
{
    public class ArticleSummaryDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public int CommentsCount { get; set; }
        public UserBasicEmbeddedInfoDto User { get; set; }
        public List<string> Categories { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public DateTime PublishAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public static ArticleSummaryDto Build(Entities.Article article)
        {
            return new ArticleSummaryDto
            {
                Id = article.Id,
                Title = article.Title,
                Slug = article.Slug,
                Description = article.Description,
                CommentsCount = article.CommentsCount,
                Categories = CategoryOnlyNameDto.BuildAsStringList(article.ArticleCategories),
                User = UserBasicEmbeddedInfoDto.Build(article.User),
                Tags = TagOnlyNameDto.BuildAsStringList(article.ArticlesTags),
                PublishAt = article.PublishedOn,
                UpdatedAt = article.UpdatedAt
            };
        }
    }
}
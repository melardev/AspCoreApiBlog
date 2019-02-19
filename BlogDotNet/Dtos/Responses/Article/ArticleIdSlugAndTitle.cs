using System;
using System.Collections.Generic;
using BlogDotNet.Dtos.Responses.Category;
using BlogDotNet.Dtos.Responses.Tag;
using BlogDotNet.Entities;
using BlogDotNet.Models;
using BlogDotNet.Models.ViewModels.User;

namespace BlogDotNet.Dtos.Responses.Article
{
    public class ArticleIdSlugAndTitle
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        

        public static ArticleIdSlugAndTitle Build(Entities.Article article)
        {
            return new ArticleIdSlugAndTitle
            {
                Id = article.Id,
                Title = article.Title,
                Slug = article.Slug,
            };
        }
    }
}
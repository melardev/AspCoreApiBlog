using System.Collections.Generic;
using BlogDotNet.Entities;
using BlogDotNet.Models;

namespace BlogDotNet.Dtos.Responses.Category
{
    public class CategoryOnlyNameDto
    {
        public string Name { get; set; }

        public static List<string> BuildAsStringList(ICollection<ArticleCategory> articleArticleCategories)
        {
            List<string> result = new List<string>(articleArticleCategories.Count);
            foreach (var articleCategory in articleArticleCategories)
            {
                result.Add(articleCategory.Category?.Name);
            }

            return result;
        }
    }
}
using System.Collections.Generic;
using BlogDotNet.Entities;
using BlogDotNet.Models;

namespace BlogDotNet.Dtos.Responses.Tag
{
    public class TagOnlyNameDto
    {
        public string Name { get; set; }

        public static List<string> BuildAsStringList(IEnumerable<ArticleTag> articleTags)
        {
            //List<string> result = new List<string>(articleTags.Count);
            List<string> result = new List<string>(20);
            foreach (var articleTag in articleTags)
            {
                result.Add(articleTag?.Tag?.Name);
            }

            return result;
        }
    }
}
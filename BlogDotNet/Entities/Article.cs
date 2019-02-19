using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using BlogDotNet.Enums;
using BlogDotNet.Models;

namespace BlogDotNet.Entities
{
    public class Article
    {
        public Article()
        {
            ArticleCategories = new HashSet<ArticleCategory>();
            Likes = new HashSet<Like>();
            ArticlesTags = new HashSet<ArticleTag>();
            Comments = new HashSet<Comment>();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }


        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        [Required] public string Body { get; set; }

        public string Slug { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime PublishedOn { get; set; }

        /*    [Column(TypeName = "int")] public ArticleContentType ArticleContentType { get; set; }
    
            [Column(TypeName = "int")] public PublishType PublishType { get; set; }
    
            [Column(TypeName = "int")] public VisibilityType VisibilityType { get; set; }
    
    */
        public ICollection<Comment> Comments { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
        public ICollection<ArticleCategory> ArticleCategories { get; set; }
        public ICollection<Like> Likes { get; set; }


        public ICollection<ArticleTag> ArticlesTags { get; set; }
        [NotMapped] public int CommentsCount { get; set; }

        public string GetUrl()
        {
            return $"/blog/{Slug}/";
        }


        public static string CreateSlug(string title)
        {
            title = title.ToLowerInvariant().Replace(" ", "-");
            title = RemoveDiacritics(title);
            title = RemoveReservedUrlCharacters(title);

            return title.ToLowerInvariant();
        }

        static string RemoveReservedUrlCharacters(string text)
        {
            var reservedCharacters = new List<string>
            {
                "!", "#", "$", "&", "'", "(", ")", "*", ",", "/", ":", ";", "=", "?", "@", "[", "]", "\"", "%", ".",
                "<", ">", "\\", "^", "_", "'", "{", "}", "|", "~", "`", "+"
            };

            foreach (var chr in reservedCharacters)
            {
                text = text.Replace(chr, "");
            }

            return text;
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public string RenderContent()
        {
            var result = Body;
            if (!string.IsNullOrEmpty(result))
            {
                // Set up lazy loading of images/iframes
                result = result.Replace(" src=\"",
                    " src=\"data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\" data-src=\"");

                // Youtube content embedded using this syntax: [youtube:xyzAbc123]
                var video =
                    "<div class=\"video\"><iframe width=\"560\" height=\"315\" title=\"YouTube embed\" src=\"about:blank\" data-src=\"https://www.youtube-nocookie.com/embed/{0}?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allowfullscreen></iframe></div>";
                result = Regex.Replace(result, @"\[youtube:(.*?)\]", m => string.Format(video, m.Groups[1].Value));
            }

            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BlogDotNet.Dtos.Responses.Category;
using BlogDotNet.Dtos.Responses.Tag;
using BlogDotNet.Enums;
using Newtonsoft.Json;

namespace BlogDotNet.Dtos.Requests.Article
{
    public class CreateOrEditArticleDto
    {
        [JsonProperty(PropertyName = "title")]
        [Required]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "body")]
        [Required]
        public string Body { get; set; }

        [JsonProperty(propertyName: "description")]
        [Required]
        public string Description { get; set; }

        // [Required]
        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime PublishedOn { get; set; }

        public VisibilityType VisibilityType { get; set; }

        [JsonProperty(PropertyName = "categories")]
        public IEnumerable<CategoryOnlyNameDto> Categories { get; set; }

        [JsonProperty(PropertyName = "tags")] public ICollection<TagOnlyNameDto> Tags { get; set; }
    }
}
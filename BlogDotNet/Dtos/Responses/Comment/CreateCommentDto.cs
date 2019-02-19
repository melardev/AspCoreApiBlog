using System;
using System.ComponentModel.DataAnnotations;

namespace BlogDotNet.Dtos.Responses.Comment
{
    public class CreateCommentDto
    {
        //[Required]
        public long Id { get; set; }

        [Required] public String Slug { get; set; }

        public String ArticleId { get; set; }
        [Required] public string Content { get; set; }
    }
}
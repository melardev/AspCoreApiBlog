using BlogDotNet.Dtos.Responses.Article;
using BlogDotNet.Models.ViewModels.User;

namespace BlogDotNet.Dtos.Responses.Likes
{
    public class LikeSummaryDto
    {
        public ArticleIdSlugAndTitle Article { get; set; }
        public UserBasicEmbeddedInfoDto User { get; set; }
    }
}
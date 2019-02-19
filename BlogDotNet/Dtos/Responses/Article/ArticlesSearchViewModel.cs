using System.Collections.Generic;

namespace BlogDotNet.Dtos.Responses.Article
{
    public class ArticlesBySearchTerm : ArticleListDtoResponse
    {
        public string SearchTerm { get; set; }
//        public IEnumerable<ArticleSearchResultViewModel> Articles { get; set; }
    }

    public class ArticleSearchResultViewModel
    {
        public ArticleDetailsDto Article { get; set; }
        public IEnumerable<Entities.Category> Categories { get; set; }
        public IEnumerable<Entities.Category> Tags { get; set; }
    }
}
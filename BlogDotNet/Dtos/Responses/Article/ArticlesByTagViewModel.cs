namespace BlogDotNet.Dtos.Responses.Article
{
    public class ArticlesByTagViewModel : ArticleListDtoResponse
    {
        public Entities.Tag Tag { get; set; }
        //public IEnumerable<BlogDotNet.Models.ViewModels.Article.ArticleViewModel> Articles { get; set; }
    }
}
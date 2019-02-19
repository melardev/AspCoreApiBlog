namespace BlogDotNet.Dtos.Responses.Article
{
  public class ArticlesbyCategoryViewModel : ArticleListDtoResponse
  {
    public Entities.Category Category { get; set; }
    //public IEnumerable<Models.Article> Articles { get; set; }
  }
}

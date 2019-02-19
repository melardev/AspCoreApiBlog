namespace BlogDotNet.Dtos.Responses.Article
{
    public class DeleteArticleViewModel
    {
        //Article Article {get; set;}
        public string Id { get; set; }
        public string Title { get; set; }
        public string CreatedAt { get; set; }
        public string Slug { get; set; }
    }
}
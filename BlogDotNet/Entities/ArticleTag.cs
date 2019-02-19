namespace BlogDotNet.Entities
{
    public class ArticleTag
    {
        public string TagId { get; set; }
        public string ArticleId { get; set; }

        public Article Article { get; set; }
        public Tag Tag { get; set; }
    }
}
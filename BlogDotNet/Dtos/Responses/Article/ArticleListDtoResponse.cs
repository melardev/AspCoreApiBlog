using System.Collections.Generic;
using BlogDotNet.Models;

namespace BlogDotNet.Dtos.Responses.Article
{
    public class ArticleListDtoResponse : PagedDto // : AbstractListViewModel
    {
        public IEnumerable<ArticleSummaryDto> Articles { get; set; }
//    public int SortBy {get; set;}


        public static ArticleListDtoResponse Build(List<Entities.Article> articles,
            string basePath,
            int currentPage, int pageSize, int totalItemCount)
        {
            List<ArticleSummaryDto> articleSummaryDtos = new List<ArticleSummaryDto>(articles.Count);
            foreach (var article in articles)
            {
                articleSummaryDtos.Add(ArticleSummaryDto.Build(article));
            }

            return new ArticleListDtoResponse
            {
                PageMeta = new PageMeta(articles.Count, basePath, currentPage: currentPage, pageSize: pageSize,
                    totalItemCount: totalItemCount),
                Articles = articleSummaryDtos
            };
        }
    }
}
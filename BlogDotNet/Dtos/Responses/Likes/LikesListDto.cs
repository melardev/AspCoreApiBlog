using System.Collections.Generic;
using BlogDotNet.Dtos.Responses.Article;
using BlogDotNet.Entities;
using BlogDotNet.Models;
using BlogDotNet.Models.ViewModels.User;
using Microsoft.AspNetCore.Http;

namespace BlogDotNet.Dtos.Responses.Likes
{
    public class LikesListDto : PagedDto
    {
        public IEnumerable<LikeSummaryDto> Likes { get; set; }

        public static LikesListDto Build(List<Like> likes, PathString requestPath, int page, int requestedPageSize,
            int totalLikesCount, bool includeUser = true)
        {
            List<LikeSummaryDto> articleSummaryDtos = new List<LikeSummaryDto>(likes.Count);
            foreach (var like in likes)
            {
                articleSummaryDtos.Add(new LikeSummaryDto
                {
                    Article = ArticleIdSlugAndTitle.Build(like.Article),
                    User = includeUser && like.User != null ? null : UserBasicEmbeddedInfoDto.Build(like.User)
                });
            }

            return new LikesListDto
            {
                PageMeta = new PageMeta(likes.Count, requestPath, currentPage: page, pageSize: requestedPageSize,
                    totalItemCount: totalLikesCount),
                Likes = articleSummaryDtos
            };
        }
    }
}
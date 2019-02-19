using System.Collections.Generic;
using BlogDotNet.Entities;
using BlogDotNet.Models;
using BlogDotNet.Models.ViewModels.User;
using Microsoft.AspNetCore.Http;

namespace BlogDotNet.Dtos.Responses.Subscriptions
{
    public class UserSubscriptionsDto : PagedDto
    {
        public List<UserBasicEmbeddedInfoDto> Followers { get; set; }
        public List<UserBasicEmbeddedInfoDto> Following { get; set; }
        
        public static UserSubscriptionsDto Build(List<ApplicationUser> following, List<ApplicationUser> followers, int totalUserSubscriptionsCount, PathString requestPath,
            int page, int pageSize)
        {
            List<UserBasicEmbeddedInfoDto> followingDtoList = new List<UserBasicEmbeddedInfoDto>(following.Count);
            List<UserBasicEmbeddedInfoDto> followersDtoList = new List<UserBasicEmbeddedInfoDto>(followers.Count);
            foreach (var user in following)
            {
                followingDtoList.Add(UserBasicEmbeddedInfoDto.Build(user));
            }
            
            foreach (var user in followers)
            {
                followingDtoList.Add(UserBasicEmbeddedInfoDto.Build(user));
            }

            return new UserSubscriptionsDto
            {
                PageMeta = new PageMeta(followingDtoList.Count + followers.Count, requestPath, currentPage: page,
                    pageSize: pageSize,
                    totalItemCount: totalUserSubscriptionsCount),
                Following = followingDtoList,
                Followers = followersDtoList
            };
        }

    }
}
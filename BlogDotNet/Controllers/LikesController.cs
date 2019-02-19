using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlogDotNet.Dtos.Responses.Article;
using BlogDotNet.Dtos.Responses.Likes;
using BlogDotNet.Entities;
using BlogDotNet.Models;
using BlogDotNet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogDotNet.Controllers
{
    [Route("api/")]
    public class LikesController : Controller
    {
        private readonly LikesService _likesService;
        private readonly UsersService _usersService;

        public LikesController(LikesService likesService, UsersService usersService)
        {
            _likesService = likesService;
            _usersService = usersService;
        }


        [HttpGet]
        [Route("likes")]
        [Authorize]
        public async Task<IActionResult> GetMyLikes([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            Tuple<int, List<Like>> likes =
                await _likesService.GetUserLikes(await _usersService.GetCurrentUserAsync(), page, pageSize, true, true);
            return StatusCodeAndDtoWrapper.BuildSuccess(LikesListDto.Build(likes.Item2, Request.Path, page, pageSize,
                likes.Item1, true));
        }

        [HttpPost]
        [Route("articles/{slug}/likes")]
        [Authorize]
        public async Task<IActionResult> CreateLike(string slug)
        {
            string userId = _usersService.GetCurrentUserId();
            bool liked = await _likesService.HasUserLikedArticleBySlug(slug, userId);
            Like like = await _likesService.GetLikeByArticleSlugNoInclude(userId, slug);
            if (like != null)
            {
                return StatusCodeAndDtoWrapper.BuildErrorResponse("You have already liked this article");
            }

            ;

            if (await _likesService.CreateLikeByArticleSlug(userId, slug))
            {
                return StatusCodeAndDtoWrapper.BuildSuccess("Article liked successfully");
            }
            else
            {
                return StatusCodeAndDtoWrapper.BuildErrorResponse("You have already liked this article");
            }
        }


        [HttpDelete]
        [Route("articles/{slug}/likes")]
        [Authorize]
        public async Task<IActionResult> DeleteLike(string slug)
        {
            string userId = _usersService.GetCurrentUserId();

            Like like = await _likesService.GetLikeByArticleSlugNoInclude(userId, slug);
            // if (!(await _likesService.HasUserLikedArticleBySlug(slug, userId)))
            if (like == null)
            {
                return StatusCodeAndDtoWrapper.BuildErrorResponse("You are not liking this article");
            }

            if (await _likesService.DeleteLikeByArticleSlug(userId, slug))
            {
                return StatusCodeAndDtoWrapper.BuildSuccess("Article like removed successfully");
            }
            else
            {
                return StatusCodeAndDtoWrapper.BuildErrorResponse("Something went wrong");
            }
        }
    }
}
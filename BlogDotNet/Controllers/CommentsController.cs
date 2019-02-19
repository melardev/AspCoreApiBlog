using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BlogDotNet.Dtos.Responses.Comment;
using BlogDotNet.Entities;
using BlogDotNet.Errors;
using BlogDotNet.Models;
using BlogDotNet.Models.ViewModels.Requests.Comment;
using BlogDotNet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogDotNet.Controllers
{
    //[ApiController]
    [Route("api/")]
    public class CommentsController : Controller
    {
        private UsersService _usersService;

        private readonly CommentsService _commentService;

        // private ArticlesService _articlesService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IConfigurationService _configService;

        public CommentsController(UsersService usersService, IAuthorizationService authorizationService,
            CommentsService commentService,
            IConfigurationService configService)
        {
            _usersService = usersService;
            _authorizationService = authorizationService;
            _configService = configService;
            _commentService = commentService;
        }

        [HttpGet("articles/{slug}/comments")]
        public async Task<IActionResult> GetComments(string slug, [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            Tuple<int, List<Comment>> comments = await _commentService.GetFromArticle(slug);

            return StatusCodeAndDtoWrapper.BuildSuccess(
                CommentListDto.Build(comments.Item2, Request.Path, page, pageSize, comments.Item1)
            );
        }

        [HttpGet("articles/{slug}/comments/{comment_id}")]
        [HttpGet("/comments/{comment_id}")]
        public async Task<IActionResult> GetDetails(string slug, string commentId, CreateOrEditCommentDto model)
        {
            Comment comment = await _commentService.GetCommentByIdAsync(commentId);
            return StatusCodeAndDtoWrapper.BuildSuccess(CommentDetailsDto.Build(comment));
        }

        [Authorize]
        [HttpPost("articles/{slug}/comments")]
        public async Task<IActionResult> Create(string slug, [FromBody]CreateOrEditCommentDto model)
        {
            if (!ModelState.IsValid)
                return StatusCodeAndDtoWrapper.BuilBadRequest(ModelState);

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = await _usersService.GetCurrentUserAsync();
            Comment comment = await _commentService.CreateComment(user, slug, model.Content, userId);

            // return RedirectToAction("GetArticleBySlug", "Articles", new {slug = slug});
            return StatusCodeAndDtoWrapper.BuildSuccess(CommentDetailsDto.Build(comment));
        }

        [HttpDelete]
        // [ValidateAntiForgeryToken]
        [Route("{id}/delete")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> Delete([FromRoute] string id, [FromBody] string articleSlug)
        {
            Comment comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return StatusCodeAndDtoWrapper.BuildGenericNotFound();
            }

            var result = await _authorizationService.AuthorizeAsync(User, comment,
                _configService.GetDeleteCommentPolicyName());
            if (result.Succeeded)
            {
                int result2 = await _commentService.DeleteCommentAsync(id);
                return RedirectToAction("GetArticleBySlug", "Articles", new {slug = articleSlug});
            }
            else
            {
                throw new PermissionDeniedException();
            }
        }
    }
}
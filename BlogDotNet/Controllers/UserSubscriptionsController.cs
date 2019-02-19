using System.Collections.Generic;
using System.Threading.Tasks;
using BlogDotNet.Dtos.Responses.Subscriptions;
using BlogDotNet.Entities;
using BlogDotNet.Models;
using BlogDotNet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogDotNet.Controllers
{
    [Route("/api/")]
    public class UserSubscriptionsController : Controller
    {
        private readonly UserSubscriptionsService _userSubscriptions;
        private readonly UsersService _usersService;

        public UserSubscriptionsController(UserSubscriptionsService userSubscriptions, UsersService usersService)
        {
            _userSubscriptions = userSubscriptions;
            _usersService = usersService;
        }

        [HttpGet]
        [Authorize]
        [Route("users/subscriptions")]
        public async Task<IActionResult> GetMyUserSubscriptions([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var result = await _userSubscriptions.GetUserSubscriptions(_usersService.GetCurrentUserId());
            List<ApplicationUser> following = result.Item1;
            List<ApplicationUser> followers = result.Item2;
            int relationsCount = result.Item3;

            return StatusCodeAndDtoWrapper.BuildSuccess(UserSubscriptionsDto.Build(following, followers, relationsCount,
                Request.Path, page, pageSize));
        }

        [HttpPost]
        [Authorize]
        [Route("users/{username}/followers")]
        public async Task<IActionResult> Follow(string username)
        {
            // TODO: You should not trust the CurrentUserId, because it reads it from the valid jwt
            // You have to call GetCurrentUserAsync()?.Id because if user does not exist Get..Async() will return null
            string followerId = _usersService.GetCurrentUserId();
            ApplicationUser following = await _usersService.GetByUserNameAsync(username);
            
            if (_usersService.GetCurrentUserName() == username)
                return StatusCodeAndDtoWrapper.BuildErrorResponse("You can not follow yourself");
            
            if (!(await _usersService.IsAuthor(following)))
                return StatusCodeAndDtoWrapper.BuildErrorResponse("You can not follow a non-author user");
            
            string followingId = following.Id;
            UserRelation ur = await _userSubscriptions.GetUserSubscription(followingId, followerId);
            if (ur == null)
            {
                if (await _userSubscriptions.CreateUserRelation(followingId, followerId))
                    return StatusCodeAndDtoWrapper.BuildSuccess($"Now you are following {username}");
                else
                    return StatusCodeAndDtoWrapper.BuildErrorResponse($"Something went wrong");
            }
            else
            {
                return StatusCodeAndDtoWrapper.BuildErrorResponse($"You are already following {username}");
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("users/{username}/followers")]
        public async Task<IActionResult> Unfollow(string username)
        {
            string followerId = _usersService.GetCurrentUserId();
            string followingId = (await _usersService.GetByUserNameAsync(username)).Id;
            UserRelation ur = await _userSubscriptions.GetUserSubscription(followingId, followerId);
            if (ur != null)
            {
                if (await _userSubscriptions.DeleteUserSubscription(ur))
                    return StatusCodeAndDtoWrapper.BuildSuccess($"Subscription deleted successfully");
                else
                    return StatusCodeAndDtoWrapper.BuildErrorResponse($"Something went wrong");
            }
            else
            {
                return StatusCodeAndDtoWrapper.BuildErrorResponse($"You are not subscribed to {username}");
            }
        }
    }
}
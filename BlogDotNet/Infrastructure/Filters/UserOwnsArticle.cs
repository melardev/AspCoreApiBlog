using System;
using System.Threading.Tasks;
using BlogDotNet.Entities;
using BlogDotNet.Models;
using BlogDotNet.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogDotNet.Infrastructure.Filters
{
    public class UserOwnsArticle : Attribute, IAuthorizationFilter
    {
        private readonly UsersService _userService;
        private readonly ArticlesService _articlesService;
        private IHttpContextAccessor _httpContextAccessor;

        public UserOwnsArticle(UsersService userService, ArticlesService articlesService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _articlesService = articlesService;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
             var user = _userService.GetByPrincipal(context.HttpContext.User);
             var IsLoggedIn = _httpContextAccessor.HttpContext?.User?.Identity.IsAuthenticated == true;
             // var IsLoggedIn = context.HttpContext?.User?.Identity.IsAuthenticated == true;
             // var IsAdmin = HttpContext.Current.User.IsInRole("Admin");
             // var IsAdmin = Roles.IsUserInRole(User.Identity.Name, "Admin");
             var IsAdmin = context.HttpContext?.User?.IsInRole("Admin");
            /*
                        var id = context.ActionDescriptors.Parameters.Id;
                        Task<Article> article = _articlesService.GetArticleById(id);
                        var heOwnsThatArticle = article.Result.User.Id == user.Id;
            
            
                        if ((heOwnsThatArticle || IsAdmin))
                            context.Result = new StatusCodeResult(StatusCode.Status403Forbidden);
            */
            // return new ForbidResult();
            //return new ChallengeResult();
        }
    }
}
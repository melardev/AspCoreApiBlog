using System.Threading.Tasks;
using BlogDotNet.Entities;
using BlogDotNet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AuthorizationPolicy = BlogDotNet.Enums.AuthorizationPolicy;

namespace BlogDotNet.Infrastructure.Handlers
{
    public class ResourceAuthorizationHandler : AuthorizationHandler<
        ResourceAuthorizationHandler.ResourceAuthorizationRequirement, IUserOwnedResource>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IConfigurationService _configurationService;

        public ResourceAuthorizationHandler(IConfigurationService configurationService,
            UserManager<ApplicationUser> userManager)
        {
            _configurationService = configurationService;
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ResourceAuthorizationRequirement requirement,
            IUserOwnedResource resource)
        {
            var user = await _userManager.GetUserAsync(context.User);

            if (requirement.RoleBased)
            {
                string roleName = requirement.RoleName;

                if (requirement.GetType() == typeof(AllowedToCreateArticleRequirement) ||
                    requirement.GetType() == typeof(AllowedToCreateCommentRequirement))
                {
                    if (roleName == _configurationService.GetAdminRoleName())
                    {
                    }
                }
            }
            else
            {
                // For creation we have no Resource
                var policy = requirement.AuthorizationPolicy;
                if (policy == AuthorizationPolicy.ONLY_ADMIN)
                {
                    if (context.User.Identity != null &&
                        context.User.Identity.Name != null &&
                        context.User.IsInRole(_configurationService.GetAdminRoleName()))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }
                else if (policy == AuthorizationPolicy.ADMIN_AND_OWNER)
                {
                    bool isAdmin = await _userManager.IsInRoleAsync(user, _configurationService.GetAdminRoleName());
                    if (isAdmin || resource.User.Id == user.Id)
                    {
                        context.Succeed(requirement);
                    }
                }
                else if (policy == AuthorizationPolicy.ONLY_OWNER)
                {
                }
                else if (policy == AuthorizationPolicy.AUTHENTICATED_USER)
                {
                    if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }


        public abstract class ResourceAuthorizationRequirement : IAuthorizationRequirement
        {
            public string RoleName { get; set; }
            public bool RoleBased { get; set; }
            public AuthorizationPolicy AuthorizationPolicy { get; set; }

            protected ResourceAuthorizationRequirement(string roleName)
            {
                this.RoleName = roleName;
                this.RoleBased = true;
            }

            protected ResourceAuthorizationRequirement(AuthorizationPolicy authorizationPolicy)
            {
                this.AuthorizationPolicy = authorizationPolicy;
            }
        }

        public class AllowedToEditArticleRequirement : ResourceAuthorizationRequirement
        {
            public AllowedToEditArticleRequirement(AuthorizationPolicy authorizationPolicy) : base(authorizationPolicy)
            {
            }
        }

        public class AllowedToCreateArticleRequirement : ResourceAuthorizationRequirement
        {
            public AllowedToCreateArticleRequirement(string roleName) : base(roleName)
            {
            }
        }


        public class AllowedToCreateCommentRequirement : ResourceAuthorizationRequirement
        {
            public AllowedToCreateCommentRequirement(string roleName) : base(roleName)
            {
            }
        }

        public class AllowedToEditCommentRequirement : ResourceAuthorizationRequirement
        {
            public AllowedToEditCommentRequirement(AuthorizationPolicy authorizationPolicy) : base(authorizationPolicy)
            {
            }
        }

        public class AllowedToDeleteCommentRequirement : ResourceAuthorizationRequirement
        {
            public AllowedToDeleteCommentRequirement(AuthorizationPolicy authorizationPolicy) : base(
                authorizationPolicy)
            {
            }
        }
    }
}
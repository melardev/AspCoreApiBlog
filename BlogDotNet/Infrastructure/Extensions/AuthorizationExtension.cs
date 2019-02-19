using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogDotNet.Infrastructure.Handlers;
using BlogDotNet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogDotNet.Infrastructure.Extensions
{
    public static class AuthorizationExtension
    {
        public static void AddAppAuthorization(this IServiceCollection services,
            IConfiguration configuration)
        {
            IConfigurationService settingsService = services.BuildServiceProvider().GetRequiredService<IConfigurationService>();

            services.AddTransient<IAuthorizationHandler, ResourceAuthorizationHandler>();


            services.AddAuthorization(options => // AuthorizationOptions
            {
                options.AddPolicy("CreateArticlePolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new ResourceAuthorizationHandler.AllowedToCreateArticleRequirement(
                        settingsService.GetWhoIsAllowedToCreateArticles()));
                });

                options.AddPolicy("CreateArticlePolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(
                        new ResourceAuthorizationHandler.AllowedToCreateArticleRequirement(settingsService
                            .GetWhoIsAllowedToCreateArticles()));
                });

                options.AddPolicy("EditArticlePolicy", policy =>
                {
                    policy.AddRequirements(
                        new ResourceAuthorizationHandler.AllowedToEditArticleRequirement(settingsService
                            .GetWhoIsAllowedToEditArticles()));
                });

                options.AddPolicy("DeleteArticlesPolicy", policy =>
                {
                    policy.AddRequirements(
                        new ResourceAuthorizationHandler.AllowedToEditArticleRequirement(settingsService
                            .GetWhoIsAllowedToDeleleArticles()));
                });

                settingsService.GetCreateCommentPolicyName();
                options.AddPolicy("CreateCommentsPolicy", policy =>
                {
                    policy.AddRequirements(
                        new ResourceAuthorizationHandler.AllowedToCreateCommentRequirement(settingsService
                            .GetWhoIsAllowedToCreateComments()));
                });

                options.AddPolicy("CreateCommentsPolicy", policy =>
                {
                    policy.AddRequirements(
                        new ResourceAuthorizationHandler.AllowedToCreateCommentRequirement(settingsService
                            .GetWhoIsAllowedToCreateComments()));
                });

                options.AddPolicy("EditCommentsPolicy", policy =>
                {
                    policy.AddRequirements(
                        new ResourceAuthorizationHandler.AllowedToEditCommentRequirement(settingsService
                            .GetWhoIsAllowedToEditComments()));
                });

                options.AddPolicy("DeleteCommentsPolicy", policy =>
                {
                    policy.AddRequirements(
                        new ResourceAuthorizationHandler.AllowedToDeleteCommentRequirement(settingsService
                            .GetWhoIsAllowedToDeleteComments()));
                });
            });
            
            
        }
    }
}
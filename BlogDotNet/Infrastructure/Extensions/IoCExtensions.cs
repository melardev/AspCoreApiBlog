using BlogDotNet.Infrastructure.Handlers;
using BlogDotNet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogDotNet.Infrastructure.Extensions
{
    public static class IoCExtensions
    {
        public static void AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ArticlesService>();
            services.AddScoped<CommentsService>();
            services.AddScoped<UsersService>();
            services.AddScoped<LikesService>();
            services.AddScoped<UserSubscriptionsService>();
            services.AddScoped<IConfigurationService>();
            // services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Authorization Handler for user owned resources ( articles, comments)
            services.AddTransient<IAuthorizationHandler, ResourceAuthorizationHandler>();
        }
    }
}
using System;
using BlogDotNet.Enums;
using Microsoft.Extensions.Configuration;

namespace BlogDotNet.Services
{
    public class IConfigurationService
    {
        private readonly IConfiguration _configuration;

        public IConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetAdminUserName()
        {
            //_configuration.GetSection("Users")["Admin::Username"];
            // Coalescense expression
            return _configuration["Auth::Admin::Username"] ?? "admin";
        }

        public string GetAdminEmail()
        {
            return _configuration["Auth::Admin::Email"] ?? "admin@no-reply.com";
        }

        public string GetAdminRoleName()
        {
            return _configuration["Auth::Roles::Admin"] ?? "ROLE_ADMIN";
        }

        public string GetAdminPassword()
        {
            return _configuration["Auth::Admin::Password"] ?? "password";
        }

        public int GetDefaultPage()
        {
            return Convert.ToInt32(_configuration["Content::Page::First"] ?? "1");
        }

        public int GetDefaultPageSize()
        {
            return Convert.ToInt32(_configuration["Content::Page::Size"] ?? "5");
        }

        public string GetAuthorRoleName()
        {
            return _configuration["Auth::Roles::Author"] ?? "ROLE_AUTHOR";
        }

        public string GetWhoIsAllowedToCreateArticles()
        {
            return GetAuthorRoleName();
            // return Enum.Parse<AuthorizationPolicy>(_configuration["Auth::Policies:Articles::Create::Who"] ??            "AUTHENTICATED_USER");
        }


        public string GetEditArticlePolicyName()
        {
            return _configuration["Auth::Policies:Articles::Edit::Name"] ?? "EditArticlesPolicy";
        }

        public string GetCreateCommentPolicyName()
        {
            return _configuration["Auth::Policies:Comments::Create::Name"] ?? "CreateCommentsPolicy";
        }

        public string GetDeleteCommentPolicyName()
        {
            return _configuration["Auth::Policies:Comments::Delete::Name"] ?? "DeleteCommentsPolicy";
        }

        public string GetWhoIsAllowedToCreateArticle()
        {
            return GetAuthorRoleName();
            // return Enum.Parse<AuthorizationPolicy>(_configuration["Auth::Policies:Articles::Create::Who"] ?? "AUTHENTICATED_USER");
        }

        public AuthorizationPolicy GetWhoIsAllowedToEditArticles()
        {
            return Enum.Parse<AuthorizationPolicy>(_configuration["Auth::Policies:Articles::Edit::Who"] ??
                                                   "ADMIN_AND_OWNER");
        }

        public AuthorizationPolicy GetWhoIsAllowedToDeleleArticles()
        {
            return Enum.Parse<AuthorizationPolicy>(_configuration["Auth::Policies:Articles::Edit::Who"] ??
                                                   "ONLY_ADMIN");
        }

        public string GetWhoIsAllowedToCreateComments()
        {
            return GetStandardUserRoleName();
            // return Enum.Parse<AuthorizationPolicy>(_configuration["Auth::Policies:Comment::Edit::Who"] ??      "AUTHENTICATED_USER");
        }

        public AuthorizationPolicy GetWhoIsAllowedToDeleteComments()
        {
            return Enum.Parse<AuthorizationPolicy>(_configuration["Auth::Policies:Comment::Delete::Who"] ??
                                                   "ONLY_ADMIN");
        }

        public AuthorizationPolicy GetWhoIsAllowedToEditComments()
        {
            return Enum.Parse<AuthorizationPolicy>(_configuration["Auth::Policies:Comment::Edit::Who"] ??
                                                   "ONLY_ADMIN");
        }

        public string GetStandardUserRoleName()
        {
            return "ROLE_USER";
        }
    }
}
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BlogDotNet.Data;
using BlogDotNet.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BlogDotNet.Extensions
{
    public static class AuthExtensions
    {
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // ===== Add our DbContext ========
            services.AddDbContext<ApplicationDbContext>();

            // ===== Add Identity ========
            //services.AddIdentity<IdentityUser, IdentityRole>()
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // ===== Add Jwt Authentication ========
            var issuer = configuration["Security:Jwt:JwtIssuer"];
            var audience = configuration.GetSection("Security:Jwt:JwtIssuer").Value;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = issuer, //configuration["Security::Jwt::JwtIssuer"],
                        ValidAudience = audience, //configuration["Security::Jwt::JwtAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JWT_SUPER_SECRET")),
                        //new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Security::Jwt::JwtKey"])),
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = false,
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });
        }

        public static void AddIdentityAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    //Configuration.GetConnectionString("DataSources::Sqlite::ConnectionString")
                    configuration.GetSection("DataSources:Sqlite:ConnectionString").Value
                ));
            // options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            /*services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(connectionString))
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();*/
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            /*
             * services.AddIdentityCore<IdentityUser, IdentityRole>(options => {
                options.User.RequireUniqueEmail = true
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

             */

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/users/login";
                    options.LogoutPath = "/logout";
                });
        }
    }
}
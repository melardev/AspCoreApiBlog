using AspNetCore.RouteAnalyzer;
using BlogDotNet.Data;
using BlogDotNet.Extensions;
using BlogDotNet.Infrastructure.Extensions;
using BlogDotNet.Infrastructure.Handlers;
using BlogDotNet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace BlogDotNet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDb(Configuration);
            services.AddJwtAuthentication(Configuration);
            services.AddMvcCoreFramework(Configuration);
            services.AddAppServices(Configuration);
            services.AddAppAuthorization(Configuration);
            services.AddRouteAnalyzer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // In my project Api Ecommerce I showed a different way of enabling Cors in our application
            app.UseCors(builder =>
            {
                builder.AllowAnyHeader();
                // builder.AllowAnyOrigin();
                builder.WithOrigins("*"); // this is the same as AllowAnyOrigin();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
         
            app.UseMvc(routes =>
            {
                routes.MapRouteAnalyzer("/routes");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
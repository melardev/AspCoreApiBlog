using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using BlogDotNet.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogDotNet.Infrastructure.Extensions
{
    public static class DbExtension
    {
        public static void AddDb(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    //Configuration.GetConnectionString("DataSources::Sqlite::ConnectionString")
                    configuration.GetSection("DataSources:Sqlite:ConnectionString").Value
                ));

            /*
             services.AddDbContext<ApplicationDbContext>(options =>
      options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<MyContext>(options=>options.UseSqlServer(Configuration
                ["ConnectionStrings:DefaultConnection"]));
            */
           
        }
    }
}

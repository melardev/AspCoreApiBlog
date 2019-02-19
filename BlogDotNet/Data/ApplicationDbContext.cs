using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlogDotNet.Entities;
using BlogDotNet.Enums;
using BlogDotNet.Errors;
using BlogDotNet.Infrastructure.Extensions;
using BlogDotNet.Services;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogDotNet.Data
{
    // https://stackoverflow.com/questions/20104289/foreign-key-to-microsoft-aspnet-identity-entityframework-identityuser
    // public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Article> Articles { get; set; }


        public virtual DbSet<UserRelation> UserRelations { get; set; }
        public virtual DbSet<Like> Likes { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<ArticleTag> ArticlesTags { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ArticleCategory> ArticlesCategories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            /* Not working in EF Core
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Following)
                .WithMany(u => u.Followers)
                .Map(x => x.MapLeftKey("UserId").MapRightKey("FollowerId").ToTable("UserFollowers"));
            */


            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            var converter = new ValueConverter<VisibilityType, int>(
                v => (int) v,
                v => (VisibilityType) v);
            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("IX_FK_UsersArticles");

                entity.Property(e => e.Body).IsRequired();

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.Slug).IsRequired();

                entity.Property(e => e.Title).IsRequired();

                entity.Property(e => e.UserId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Articles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // entity.Property(e => e.VisibilityType).HasConversion(converter);
                //entity.Property(e => e.PublishType).HasConversion(converter);
            });


            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Description);
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<ArticleCategory>(entity =>
            {
                entity.HasKey(e => new {e.CategoryId, e.ArticleId});

                entity.Property(e => e.CategoryId);

                entity.Property(e => e.ArticleId);

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticleCategories)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.Cascade);
                //.OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.ArticleCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
                //.OnDelete(DeleteBehavior.ClientSetNull);
            });


            modelBuilder.Entity<UserRelation>(entity =>
            {
                entity.HasKey(e => new {e.FollowingUserId, FollowedUserId = e.FollowerUserId});

                entity.Property(e => e.FollowingUserId);

                entity.Property(e => e.FollowerUserId);


                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.Followers)
                    .HasForeignKey(d => d.FollowerUserId)
                    .OnDelete(DeleteBehavior.Cascade);
                //.OnDelete(DeleteBehavior.ClientSetNull)

                // review this 
                entity.HasOne(d => d.Following)
                    .WithMany(p => p.Following)
                    .HasForeignKey(d => d.FollowingUserId)
                    .OnDelete(DeleteBehavior.Cascade);
                //.OnDelete(DeleteBehavior.ClientSetNull)
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasKey(l => new {l.UserId, l.ArticleId});

                entity.Property(e => e.ArticleId);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UserId);

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.Cascade);
                //.OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                //.OnDelete(DeleteBehavior.ClientSetNull);
            });


            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.Description);
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Comment>()
                .HasOne(x => x.RepliedComment)
                .WithMany(x => x.Replies)
                .HasForeignKey(c => c.RepliedCommentId);

            modelBuilder.Entity<ArticleTag>(entity =>
            {
                entity.HasKey(e => new {e.TagId, e.ArticleId});
                //entity.HasIndex(e => e.ArticleId);

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticlesTags)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.ArticlesTags)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }

        public static async void Seed(IServiceProvider services, IConfigurationRoot config)
        {
            await SeedAdminUserAndRole(services);
            await SeedAuthorUsersAndRole(services);
            await SeedAuthenticatedUsersAndRole(services);
            await SeedTags(services);
            await SeedCategories(services);
            await SeedArticles(services);
            await SeedComments(services);
            await SeedLikes(services);
            await SeedRelations(services);
        }

        private static async Task SeedRelations(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                ApplicationDbContext dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                var userRelationsCount = await dbContext.UserRelations.CountAsync();
                var userRelationsToSeed = 50;

                userRelationsToSeed -= userRelationsCount;
                if (userRelationsToSeed <= 0)
                    return;

                List<string> userIds =
                    await dbContext.Users.Select(u => u.Id).OrderBy(ur => Guid.NewGuid()).ToListAsync();

                while (userRelationsToSeed > 0)
                {
                    var follower = dbContext.Users.OrderBy(u => Guid.NewGuid()).First();

                    /* TODO: Why this does not work? I though I left the code working, and now it does not, review it
                     ApplicationUser following = await dbContext.Users.OrderBy(u => Guid.NewGuid())
                        .Where(u => u.Followers != null && u.Followers.Any(fwing => fwing.Follower.Id != follower.Id))
                        .FirstOrDefaultAsync();
                    if (following == null)
                        continue;
                    if (following != null && follower != null &&
                        following.Followers != null &&
                        following.Followers.Any(fwer => fwer.Follower.Id == follower.Id))
                        Debug.WriteLine("Error");
                        
                    */

                    List<string> followingIds = await dbContext.UserRelations
                        .Where(ur => ur.FollowerUserId == follower.Id)
                        .Select(ur => ur.FollowingUserId)
                        .ToListAsync();
                    followingIds.Add(follower.Id);
                    List<string> potentianFollowingIds = new List<string>(userIds);
                    potentianFollowingIds.RemoveAll(pfi => followingIds.Contains(pfi));
                    string followingId = potentianFollowingIds.OrderBy(pfi => new Guid()).FirstOrDefault();
                    dbContext.UserRelations.Add(new UserRelation
                    {
                        Follower = follower,
                        FollowingUserId = followingId,
                    });
                    userRelationsToSeed--;
                    dbContext.ChangeTracker.DetectChanges();
                    await dbContext.SaveChangesAsync();
                }

                
            }
        }

        private static async Task SeedLikes(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                ApplicationDbContext dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                var likesCount = await dbContext.Likes.CountAsync();
                var likesToSeed = 35;

                likesToSeed -= likesCount;
                if (likesToSeed <= 0)
                    return;

                while (likesToSeed > 0)
                {
                    dbContext.Likes.Add(new Like
                    {
                        Article = dbContext.Articles.OrderBy(a => Guid.NewGuid()).First(),
                        User = dbContext.Users.OrderBy(u => Guid.NewGuid()).First()
                    });
                    likesToSeed--;
                }

                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task SeedTags(IServiceProvider services)
        {
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                ApplicationDbContext dbContext = services.GetRequiredService<ApplicationDbContext>();
                var tagCount = await dbContext.Tags.CountAsync();
                var tagsToSeed = 5;
                tagsToSeed -= tagCount;
                if (tagsToSeed <= 0)
                    return;
                var faker = new Faker<Tag>()
                    .RuleFor(t => t.Name, f => f.Lorem.Word())
                    .RuleFor(t => t.Description, f => f.Lorem.Sentences(2));

                List<Tag> tags = faker.Generate(tagsToSeed);
                dbContext.Tags.AddRange(tags);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task SeedCategories(IServiceProvider services)
        {
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                ApplicationDbContext dbContext = services.GetRequiredService<ApplicationDbContext>();
                var categoryCount = await dbContext.Categories.CountAsync();
                var categoriesToSeed = 5;
                categoriesToSeed -= categoryCount;
                if (categoriesToSeed <= 0)
                    return;
                var faker = new Faker<Category>()
                    .RuleFor(t => t.Name, f => f.Lorem.Word())
                    .RuleFor(t => t.Description, f => f.Lorem.Sentences(2));


                List<Category> categories = faker.Generate(categoriesToSeed);
                dbContext.Categories.AddRange(categories);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task SeedComments(IServiceProvider services)
        {
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IConfigurationService settingsService =
                    serviceScope.ServiceProvider.GetService<IConfigurationService>();
                ApplicationDbContext dbContext = services.GetRequiredService<ApplicationDbContext>();
                var faker = new Faker<Comment>()
                    .RuleFor(c => c.Content, f => f.Lorem.Sentences(f.Random.Number(1, 3)))
                    .FinishWith(async (f, c) =>
                    {
                        c.Article = await dbContext.Articles.OrderBy(a => Guid.NewGuid()).FirstAsync();
                        c.User = await dbContext.Users.OrderBy(a => Guid.NewGuid()).FirstAsync();
                        //dbContext.Attach(c.User);
                    });

                var commentsCount = await dbContext.Comments.Where(c => c.RepliedComment == null).CountAsync();
                var commentsToSeed = 35;
                commentsToSeed -= commentsCount;

                if (commentsToSeed > 0)
                {
                    List<Comment> comments = faker.Generate(commentsToSeed);
                    dbContext.Comments.AddRange(comments);
                    await dbContext.SaveChangesAsync();
                }

                var commentRepliesCount = await dbContext.Comments.Where(c => c.RepliedComment != null).CountAsync();
                var commentRepliesToSeed = 20;
                commentRepliesToSeed -= commentRepliesCount;
                if (commentRepliesToSeed > 0)
                {
                    Comment parent = dbContext.Comments.OrderBy(c => Guid.NewGuid()).First();
                    faker.RuleFor(c => c.RepliedComment, f => parent);

                    List<Comment> replies = faker.Generate(commentRepliesToSeed);
                    await dbContext.Comments.AddRangeAsync(replies);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedArticles(IServiceProvider services)
        {
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IConfigurationService settingsService =
                    serviceScope.ServiceProvider.GetService<IConfigurationService>();
                ApplicationDbContext dbContext = services.GetRequiredService<ApplicationDbContext>();
                var articleCount = await dbContext.Articles.CountAsync();
                var articleToSeed = 35;
                articleToSeed -= articleCount;
                if (articleToSeed <= 0)
                    return;

                UserManager<ApplicationUser> userManager =
                    serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                RoleManager<IdentityRole> roleManager =
                    serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                var faker = new Faker<Article>()
                    .RuleFor(a => a.PublishedOn, f => f.Date
                        .Between(DateTime.Now.AddYears(-3), DateTime.Now.AddYears(1)))
                    .RuleFor(a => a.Title, f => f.Lorem.Sentence())
                    .RuleFor(a => a.Description, f => f.Lorem.Sentences(2))
                    .RuleFor(a => a.Body, f => f.Lorem.Paragraphs(2))
                    //.RuleFor(a => a.VisibilityType, f => (VisibilityType) f.Random.Number(0,2))
                    // .RuleFor(a => a.PublishType, f => (PublishType) f.Random.Number(0,1))
                    .FinishWith(async (f, a) =>
                    {
                        /*ApplicationUser user = await dbContext.Users
                            .Where(u => u.Roles.Any(r => userManager.IsInRoleAsync(u, settingsService.GetAuthorRoleName()).Result))
                            .OrderBy(u => Guid.NewGuid())
                            .FirstAsync();
                        */

                        IList<ApplicationUser> authors =
                            await userManager.GetUsersInRoleAsync(settingsService.GetAuthorRoleName());
                        ICollection<ArticleTag> articlesTags = new List<ArticleTag>();
                        articlesTags.Add(new ArticleTag
                            {
                                Article = a,
                                ArticleId = a.Id,
                                Tag = await dbContext.Tags.OrderBy(t => Guid.NewGuid()).FirstAsync()
                            }
                        );
                        a.ArticlesTags = articlesTags;
                        a.Slug = a.Title.Slugify();
                        a.User = authors.OrderBy(x => Guid.NewGuid()).First();
                        dbContext.Attach(a.User);
                    });
                /* .RuleFor(a => a.ArticlesTags, async (f, a) =>
                 {
                     ICollection<ArticleTag> list = new List<ArticleTag>();
                     list.Add(new ArticleTag
                         {
                             Article = a,
                             ArticleId = a.Id,
                             Tag = await dbContext.Tags.OrderBy(t => Guid.NewGuid()).FirstAsync()
                         }
                     );
                     return list;
                 });
                 */


                List<Article> articles = faker.Generate(articleToSeed);
                articles.ForEach(a =>
                {
                    dbContext.Articles.Add(a);
                    //       dbContext.Entry(a).State = EntityState.Added;
                });
                EntityEntry<Article> entry = dbContext.Articles.Add(articles[0]);
                dbContext.Articles.AddRange(articles);
                // dbContext.ChangeTracker.DetectChanges();
                // var res = dbContext.SaveChanges();
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task SeedAuthenticatedUsersAndRole(IServiceProvider services)
        {
            Faker faker = new Faker();
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IConfigurationService settingsService = services.GetService<IConfigurationService>();

                UserManager<ApplicationUser> userManager =
                    serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                RoleManager<IdentityRole> roleManager =
                    serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                string standardUserRoleName = settingsService.GetStandardUserRoleName();
//                if (await roleManager.FindByNameAsync(roleAuthor) == null)
                IdentityResult result = IdentityResult.Success;
                if (!(await roleManager.RoleExistsAsync(standardUserRoleName)))
                {
                    result = await roleManager.CreateAsync(new IdentityRole(standardUserRoleName));
                    if (!result.Succeeded)
                    {
                        throw new UnexpectedApplicationStateException();
                    }
                }

                if (result.Succeeded)
                {
                    ApplicationDbContext applicationDbContext =
                        serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var usersCount = applicationDbContext.Users.Count();
                    var usersToSeed = 43;
                    usersToSeed -= usersCount;
                    IdentityRole r = await roleManager.FindByNameAsync(settingsService.GetAuthorRoleName());
                    for (int i = 0; i < usersToSeed; i++)
                    {
                        ApplicationUser user = new ApplicationUser
                        {
                            FirstName = faker.Name.FirstName(), LastName = faker.Name.LastName(),
                            UserName = faker.Internet.UserName(faker.Name.FirstName(), faker.Name.LastName()),
                            Email = faker.Internet.Email()
                        };
                        result = await userManager.CreateAsync(user, "password");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, standardUserRoleName);
                        }
                    }
                }
            }
        }

        private static async Task SeedAuthorUsersAndRole(IServiceProvider services)
        {
            // To be able to use GetService we need Microsoft.Extensions.DependencyInjection
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                Faker faker = new Faker();
                IConfigurationService settingsService =
                    serviceScope.ServiceProvider.GetService<IConfigurationService>();
                UserManager<ApplicationUser> userManager =
                    serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                RoleManager<IdentityRole> roleManager =
                    serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                string roleAuthor = settingsService.GetAuthorRoleName();
//                if (await roleManager.FindByNameAsync(roleAuthor) == null)
                IdentityResult result = IdentityResult.Success;
                if (!(await roleManager.RoleExistsAsync(roleAuthor)))
                {
                    result = await roleManager.CreateAsync(new IdentityRole(roleAuthor));
                    if (!result.Succeeded)
                    {
                        throw new UnexpectedApplicationStateException();
                    }
                }

                if (result.Succeeded)
                {
                    ApplicationDbContext applicationDbContext =
                        serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var usersCount = applicationDbContext.Users.Count();
                    var usersToSeed = 5;
                    usersToSeed -= usersCount;
                    IdentityRole r = await roleManager.FindByNameAsync(settingsService.GetAuthorRoleName());
                    for (int i = 0; i < usersToSeed; i++)
                    {
                        ApplicationUser user = new ApplicationUser
                        {
                            FirstName = faker.Name.FirstName(), LastName = faker.Name.LastName(),
                            UserName = faker.Internet.UserName(faker.Name.FirstName(), faker.Name.LastName()),
                            Email = faker.Internet.Email()
                        };
                        result = await userManager.CreateAsync(user, "password");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, roleAuthor);
                        }
                    }
                }
            }
        }

        private static async Task SeedAdminUserAndRole(IServiceProvider services)
        {
            using (var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                IConfigurationService configurationService = services.GetService<IConfigurationService>();
                UserManager<ApplicationUser> userManager =
                    serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                RoleManager<IdentityRole> roleManager =
                    serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                string adminUserName = configurationService.GetAdminUserName();
                string adminEmail = configurationService.GetAdminEmail();
                string adminPassword = configurationService.GetAdminPassword();
                string adminRoleName = configurationService.GetAdminRoleName();
                {
                    IdentityResult adminRoleCreated = IdentityResult.Success;
                    if (await roleManager.FindByNameAsync(adminRoleName) == null)
                    {
                        adminRoleCreated = await roleManager.CreateAsync(new IdentityRole(adminRoleName));
                    }

                    if (await userManager.FindByNameAsync(adminUserName) == null && adminRoleCreated.Succeeded)
                    {
                        ApplicationUser user = new ApplicationUser
                        {
                            UserName = adminUserName,
                            Email = adminEmail
                        };

                        IdentityResult result = await userManager.CreateAsync(user, adminPassword);

                        if (result.Succeeded)
                        {
                            result = await userManager.AddToRoleAsync(user, adminRoleName);
                            if (!result.Succeeded)
                                throw new ThreadStateException();
                        }
                    }
                }
            }
        }
    }
}
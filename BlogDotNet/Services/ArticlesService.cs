using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogDotNet.Data;
using BlogDotNet.Dtos.Requests.Article;
using BlogDotNet.Dtos.Responses.Tag;
using BlogDotNet.Entities;
using BlogDotNet.Errors;
using BlogDotNet.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;

namespace BlogDotNet.Services
{
    public class ArticlesService
    {
        private readonly ApplicationDbContext _context;
        private readonly UsersService _usersService;

        public ArticlesService(ApplicationDbContext context,
            UsersService usersService)
        {
            _context = context;
            _usersService = usersService;
        }

        ICollection<ArticleTag> ProcessTags(Article TAG)
        {
            return TAG.ArticlesTags;
        }

        public static ICollection<ArticleTag> ProjectImplementers(Article checkTemplate) => checkTemplate.ArticlesTags;

        public async Task<Tuple<int, List<Article>>> GetArticles(int page = 1, int pageSize = 10)
        {
            var skip = (page - 1) * pageSize;
            var queryable = _context.Articles.Include(a => a.User)
                .Include(a => a.ArticlesTags)
                .ThenInclude(at => at.Tag)
                .Include(a => a.ArticleCategories)
                .ThenInclude(ac => ac.Category)
                // This would be a better approach, but it does not work in 2.1.1 because ThenInclude is skipt ...
                // https://github.com/aspnet/EntityFrameworkCore/issues/12678
                /* .Select(a => new Article
                 {
                     Id = a.Id,
                     Title = a.Title,
                     Description = a.Description,
                     Slug = a.Slug,
                     User = a.User,
                     ArticlesTags = a.ArticlesTags,
                     ArticleCategories = a.ArticleCategories,
                     CommentsCount = 0,
                     PublishedOn = a.PublishedOn,
                     CreatedAt = a.CreatedAt,
                     UpdatedAt = a.UpdatedAt,
                 }).Where(a => a.PublishedOn < DateTime.Now || true)*/
                .Select(a => new
                {
                    Article = a,
                    CommentsCount = a.Comments.Count()
                });

            ;


            //&& a.VisibilityType == VisibilityType.PUBLIC || a.VisibilityType == VisibilityType.UNLISTED)


            var count = queryable.Count();
            // queryable = queryable.Skip(skip).OrderBy(a => a.PublishedOn);


            if (pageSize >= 1)
                queryable = queryable.Take(pageSize);


            var articles = await queryable.ToListAsync();

            foreach (var article in articles)
            {
                article.Article.CommentsCount = article.CommentsCount;
            }

            return await Task.FromResult(Tuple.Create(count, articles.Select(a => a.Article).ToList()));
        }

        public async Task<Tuple<int, List<Article>>> GetArticlesByCategory(string category, int page = 1,
            int pageSize = 5)
        {
            // TODO: Make Categor ya Separate Class

            IQueryable<Article> queryable = _context.Articles.Include(a => a.ArticleCategories)
                .Where(a => a.ArticleCategories.Any(ar =>
                    ar.Category.Name.Contains(category, StringComparison.OrdinalIgnoreCase)));
            var count = queryable.Count();

            List<Article> articles = await queryable.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Tuple.Create(count, articles);
        }


        public async Task<Article> UpdateArticle(string slug, CreateOrEditArticleDto dto)
        {
            var article = await _context.Articles.Include(a => a.ArticleCategories).Include(a => a.ArticlesTags)
                .Where(x => x.Slug == slug).FirstOrDefaultAsync();

            if (article == null)
            {
                throw new ResourceNotFoundException();
            }

            article.Title = dto.Title.Trim();
            article.Slug = dto.Title.Trim().Slugify();
            article.Body = dto.Body;
            article.Description = dto.Description;


            if (dto.Categories != null && dto.Categories.Count() > 0)
            {
                foreach (var category in dto.Categories)
                {
                    Category cat = _context.Categories.Where(c => c.Name.Equals(category.Name)).SingleOrDefault();
                    if (cat == null)
                    {
                        cat = new Category
                        {
                            Name = category.Name,
                        };

                        _context.Categories.Add(cat);
                        await _context.SaveChangesAsync();
                    }

                    // If the article was not yet associated to the category then do it now
                    if (!article.ArticleCategories.Any(ac => ac.Article == article && ac.Category == cat))
                    {
                        article.ArticleCategories.Add(new ArticleCategory
                        {
                            Article = article,
                            Category = cat
                        });
                    }
                }
            }

            // if (requestForm != null && requestForm.ContainsKey("categories"))
            {
                // var categories = Request.Form["categories"].ToArray(); //.Split(",", StringSplitOptions.RemoveEmptyEntries)
                // .Select(c => c.Trim()).ToLowerInvariant().ToList();
                // article.Categories = categories;    
            }


            
            if (_context.ChangeTracker.Entries().First(x => x.Entity == article).State == EntityState.Modified)
            {
                article.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return article;
        }


        public virtual async Task<Article> GetArticleBySlug(string slug)
        {
            Article article =
                await _context.Articles
                    .Include(a => a.User)
                    .Include(a => a.Comments)
                    .ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(
                        p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
            bool isAdmin = await _usersService.IsAdmin();

            if (article != null && article.PublishedOn <= DateTime.UtcNow
                //             &&
                // article.VisibilityType == VisibilityType.PUBLIC
                || await _usersService.IsAdmin())
            {
                // _context.Entry(article).Collection(a => article.Comments).Load();
                _context.Entry(article).Reference(a => a.User).Load();
                return await Task.FromResult(article);
            }

            return await Task.FromResult(article);
        }

        public async Task<Article> GetArticleById(string id, bool onlyIfPublished = false)
        {
            var article = await _context.Articles.Include(a => a.User)
                .Include(a => a.Comments)
                .ThenInclude(c => c.User).FirstOrDefaultAsync(a => a.Id == id);

            if (article != null)
            {
                // if
                if (!onlyIfPublished || // skip published check or 
                    ((onlyIfPublished && article.PublishedOn < DateTime.UtcNow // is published and
                         //  && article.VisibilityType == VisibilityType.PUBLIC // is public
                     )
                     || await _usersService.IsAdmin() // or current user is admin
                    ))
                {
                    return await Task.FromResult(article);
                }
            }

            return await Task.FromResult<Article>(null);
        }

        public async Task<Task<List<ArticleCategory>>> GetCategories()
        {
            var categories = _context.Articles
                .Where(article => article.PublishedOn < DateTime.UtcNow)
                .SelectMany(article => article.ArticleCategories)
                .Distinct().ToListAsync();

            return await Task.FromResult(categories);
        }

        public async Task<Comment> AddComment(Comment comment, Article article)
        {
            comment.User = await _usersService.GetCurrentUserAsync();
            comment.Article = article;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return await Task.FromResult(comment);
        }

        public async Task<Comment> AddComment(ApplicationUser user, Comment comment, string articleId)
        {
            Article article = await GetArticleById(articleId);
            if (article == null)
                throw new ResourceNotFoundException();

            return await AddComment(comment, article);
        }

        
        public async Task<Comment> AddComment(string content, string slug)
        {
            var article = await _context.Articles.Include(a => a.Comments)
                .FirstOrDefaultAsync(a => a.Slug == slug);
            if (article == null)
                throw new ResourceNotFoundException();

            var now = DateTime.UtcNow;
            var user = await _usersService.GetCurrentUserAsync();
            var comment = new Comment()
            {
                User = user,
                Content = content,
                CreatedAt = now,
                UpdatedAt = now
            };
            int result = await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment> EditComment(Comment tempComment)
        {
            Comment comment = await _context.Comments.Where(c => c.Id == tempComment.Id).FirstOrDefaultAsync();
            if (comment == null)
                throw new ResourceNotFoundException();

            comment.Content = comment.Content;
            await _context.SaveChangesAsync();

            return await Task.FromResult(comment);
        }


        public async Task<int> GetAllArticlesCount()
        {
            return await _context.Articles.CountAsync();
        }

        public async Task<string> CreateArticle(string title, string content)
        {
            var user = await _usersService.GetCurrentUserAsync();
            EntityEntry<Article> id = _context.Articles.Add(new Article
            {
                Title = title,
                Body = content,
                User = user
            });
            return id.Entity.Slug;
        }

        public async Task<Tuple<int, List<Article>>> GetBySearch(string term, int page, int pageSize)
        {
            var queryable = _context.Articles
                //Approach 1  .AsNoTracking()
                //.FromSql("SELECT * from [dbo].[BlogPost] WHERE Contains((Content, Description, Title), {0})",
                //    query.SearchText)

                // Approach 2
                .Where(a => //a.Body.Contains(term) ||
                    // a.Description.Contains(term) ||
                    a.Title.Contains(term, StringComparison.OrdinalIgnoreCase));
            // End
            var count = queryable.Count();

            var articles = await queryable.OrderByDescending(a => a.PublishedOn)
                .ThenByDescending(a => a.UpdatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Include(a => a.User)
                .Include(a => a.Comments)
                .Include(a => a.ArticleCategories)
                .ThenInclude(ac => ac.Category).ToListAsync();


            return Tuple.Create(count, articles);
        }

        public async Task<List<Article>> GetAllArticles(int page, int pageSize)
        {
           
            // Try to clean Include() what happens?
            IIncludableQueryable<Article, ICollection<Comment>> queryableArticles = _context.Articles
                .Where(a => a.PublishedOn < DateTime.Now
                    //&& a.VisibilityType == VisibilityType.PUBLIC)
                ).Include(a => a.User)
                .Include(a => a.Comments);

            var totalCount = await queryableArticles.CountAsync();
            List<Article> articles = await queryableArticles.Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return articles;
        }


        public void GetArticles()
        {
            var articles = _context.Articles.Include(a => a.User)
                /*.Include(a => ArticleLikes)*/.Include(a => a.ArticlesTags)
                .AsNoTracking();
        }

        public async Task<Article> Create(string title, string description, string body, ApplicationUser user,
            ICollection<TagOnlyNameDto> tagOnlyNameDtos)
        {
            List<ArticleTag> tags = new List<ArticleTag>();
            List<ArticleCategory> categories = new List<ArticleCategory>();

            if (user == null)
                user = await _usersService.GetCurrentUserAsync();


            if (tagOnlyNameDtos == null)
                tagOnlyNameDtos = new List<TagOnlyNameDto>();


            var article = new Article()
            {
                User = user,
                Title = title,
                Body = body,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Slug = title.Slugify()
            };
            foreach (var tagName in tagOnlyNameDtos)
            {
                Tag tag = await _context.Tags.Where(t => t.Name == tagName.Name).FirstOrDefaultAsync();
                if (tag == null)
                {
                    // IS there a find or create?
                    tag = new Tag
                    {
                        Name = tagName.Name,
                    };
                    tags.Add(new ArticleTag
                    {
                        Tag = tag,
                        Article = article
                    });
                    //await _context.Tags.AddAsync(tag);
                }
            }

            article.ArticlesTags = tags;
            article.ArticleCategories = categories;

            EntityEntry<Article> articleEntity = await _context.Articles.AddAsync(article);

            await _context.SaveChangesAsync();
            return articleEntity.Entity;
        }


        public async Task<Article> Update(Article article)
        {
            var articleUpdated = await _context.Articles.Where(a => a.Slug == article.Slug).FirstOrDefaultAsync();
            if (article == null)
            {
                throw new ResourceNotFoundException();
            }

            articleUpdated.Title = article.Title;
            articleUpdated.Body = article.Body;
            articleUpdated.Slug = article.Slug.Slugify();


            if (_context.ChangeTracker.Entries()
                    .First(x => x.Entity == article).State == EntityState.Modified)
            {
                articleUpdated.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return await Task.FromResult(articleUpdated);
        }

        public async Task<List<Article>> GetByTagId(string id, int page, int pageSize)
        {
            // This is from eal-world this is wha AticleTags instead of Tags
            ArticleTag tag = await _context.ArticlesTags.Where(a => a.TagId == id).FirstOrDefaultAsync();
            if (tag != null)
            {
                IQueryable<Article> queryable = _context.Articles.Include(article => article.User)
                    .Include(article => article.ArticlesTags)
                    .Where(article => article.ArticlesTags.Select(articlesTags => articlesTags.TagId)
                        .Contains(tag.TagId));

                List<Article> articles = await queryable.OrderByDescending(a => a.CreatedAt)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                return articles;
            }

            return null;
        }

        public async Task<Article> Delete(string slug)
        {
            var article = await _context.Articles.FirstOrDefaultAsync(a => a.Slug == slug);
            if (article == null)
                throw new ResourceNotFoundException();

            EntityEntry<Article> entityEntry = _context.Articles.Remove(article);

            await _context.SaveChangesAsync();
            return entityEntry.Entity;
        }

        public async Task<Article> GetBySlug(string slug)
        {
            var article = await _context.Articles.Include(a => a.User)
                .Include(a => a.Comments).Include(a => a.ArticlesTags)
                .Include(a => a.ArticleCategories)
                .FirstOrDefaultAsync(a => a.Slug == slug);
            if (article == null)
            {
                throw new ResourceNotFoundException();
            }

            return article;
        }

        public async Task<Tuple<int, List<Article>>> GetByAuthorName(string username, int page, int pageSize)
        {
            ApplicationUser user = await _usersService.GetByUserNameAsync(username);
            if (user == null)
                return null;
            IOrderedQueryable<Article> queryable = _context.Articles.Include(a => a.User)
                //.Where(a => a.User.UserName == username)
                .Where(a => a.User == user)
                .OrderByDescending(a => a.CreatedAt);

            var itemsCount = queryable.Count();
            var articles = await queryable.Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Tuple.Create(itemsCount, articles);
        }

        public async Task<List<Article>> GetByAuthor(string name)
        {
            var articles = await _context.Articles.Where(a => a.User.UserName == name).ToListAsync();
            return articles;
        }

        async Task<List<Article>> GetArticlesLikedBy(string username, int page, int pageSize)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
                throw new ResourceNotFoundException();
            var queryable = _context.Articles.Include(a => a.User)
                .Include(a => a.Likes)
                .Where(a => a.Likes.Any(like => like.UserId == user.Id));
            var articles = await queryable.Skip(page * pageSize).Take(pageSize).ToListAsync();
            return articles;
        }


        public async Task<Comment> AddComment(Comment comment)
        {
            EntityEntry<Comment> ee = await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return ee.Entity;
        }

        public async Task<Comment> GetCommentAsync(string id)
        {
            Comment comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            EntityEntry<Comment> ee = _context.Comments.Remove(comment);
            return ee.Entity;
        }

        void GetImageUrls(string html)
        {
            /*
            string[] urls;
            var regexImgSrc = new Regex("<img.+?src=[\"'](.+?)[\"'].*?>",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
            foreach (Match match in regexImgSrc.Matches(html))
            {
                String RelativePath = AbsolutePath.Replace(match.Value, String.Empty);
                ;
                Path.Combine(env.WebRootPath
                urls += relativePath;
            }
 */
        }

        public async Task<Article> GetOnlyArticleBySlug(string articleSlug)
        {
            return await _context.Articles.Where(a => a.Slug.Equals(articleSlug)).FirstOrDefaultAsync();
        }
    }
}
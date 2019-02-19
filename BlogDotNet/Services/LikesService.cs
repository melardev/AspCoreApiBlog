using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogDotNet.Data;
using BlogDotNet.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace BlogDotNet.Services
{
    public class LikesService
    {
        private readonly ApplicationDbContext _context;
        private readonly UsersService _usersService;

        public LikesService(ApplicationDbContext context,
            UsersService usersService)
        {
            _context = context;
            _usersService = usersService;
        }

        public async Task<Tuple<int, List<Article>>> GetLikedArticles(ApplicationUser user, int page, int pageSize)
        {
            List<string> result = await _context.Likes.Where(l => l.User == user).Select(l => l.Id).ToListAsync();
            IQueryable<Article> queryable = _context.Articles.Where(a => result.Contains(a.Id));
            var count = queryable.Count();

            queryable = queryable.Skip((page - 1) * pageSize).OrderBy(a => a.PublishedOn);

            if (pageSize >= 1)
                queryable = queryable.Take(pageSize);


            var articles = await queryable.ToListAsync();

            return await Task.FromResult(Tuple.Create(count, articles));
        }

        public async Task<Tuple<int, List<Like>>> GetUserLikes(ApplicationUser user, int page, int pageSize,
            bool includeArticle = true, bool includeUser = false)
        {
            IQueryable<Like> queryable = _context.Likes.Where(l => l.User == user);

            if (includeUser)
                queryable = queryable.Include(l => l.User);

            if (includeArticle)
                queryable = queryable.Include(l => l.Article);

            var count = queryable.Count();

            IOrderedQueryable<Like> orderedQueryable = queryable.Skip((page - 1) * pageSize).OrderBy(a => a.CreatedAt);

            if (pageSize >= 1)
                orderedQueryable = (IOrderedQueryable<Like>) orderedQueryable.Take(pageSize);


            var likes = await queryable.ToListAsync();

            return await Task.FromResult(Tuple.Create(count, likes));
        }

        public async Task<bool> HasUserLiked(ApplicationUser user, Article article)
        {
            return await HasUserLiked(user.Id, article.Id);
        }

        public async Task<bool> HasUserLiked(string userId, Article article)
        {
            return await HasUserLiked(userId, article.Id);
        }

        public async Task<bool> HasUserLiked(string userId, string articleId)
        {
            return await _context.Likes.CountAsync(l => l.Article.Id == articleId && l.User.Id == userId) > 0;
        }

        public async Task<bool> HasUserLikedArticleBySlug(string userId, string articleSlug)
        {
            return await _context.Likes.Include(l => l.User).Include(l => l.Article).CountAsync(l => l.Article.Slug == articleSlug && l.User.Id == userId) > 0;
        }

        public async Task<bool> CreateLikeByArticleSlug(string userId, string slug)
        {
            var article = await _context.Articles.Where(a => a.Slug == slug).SingleAsync();
            await _context.Likes.AddAsync(new Like
            {
                Article = article,
                UserId = userId,
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteLikeByArticleSlug(string userId, string slug)
        {
            Like like = await _context.Likes.Where(l => l.Article.Slug == slug && l.UserId == userId).SingleOrDefaultAsync();
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Like> GetLikeByArticleSlugNoInclude(string userId, string slug)
        {
            Like like = await _context.Likes.Where(l => l.Article.Slug == slug && l.UserId == userId).SingleOrDefaultAsync();
            return like;
        }
    }
}
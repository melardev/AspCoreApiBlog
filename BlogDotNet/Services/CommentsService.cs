using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogDotNet.Data;
using BlogDotNet.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogDotNet.Services
{
    public class CommentsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ArticlesService _articleService;

        public CommentsService(ApplicationDbContext context, ArticlesService articlesService)
        {
            _context = context;
            _articleService = articlesService;
        }

        public async Task<Comment> CreateComment(ApplicationUser user, string articleSlug, string text, string userId)
        {
            var article = await _articleService.GetOnlyArticleBySlug(articleSlug); // await _context.Articles.Where(a => a.Slug == articleSlug).FirstOrDefaultAsync();

            var comment = new Comment()
            {
                Article = article,
                ArticleId = article.Id,
                Content = text,
                UserId = userId,
                User = user,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            
            await _context.Comments.AddAsync(comment);
            article.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment> GetCommentByIdAsync(string id)
        {
            return await _context.Comments.FindAsync(id);
        }


        public async Task<int> DeleteCommentAsync(string id)
        {
            Comment comment = await _context.Comments.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (comment == null)
            {
                return -1;
            }

            _context.Comments.Remove(comment);
            return await _context.SaveChangesAsync();
        }

        public async Task<Tuple<int, List<Comment>>> GetFromArticle(string slug)
        {
            IQueryable<Comment> queryable = _context.Comments.Where(c => c.Article.Slug.Equals(slug))
                .Include(c => c.Article)
                .Include(c => c.User)
                .Select(c => new Comment
                {
                    Content = c.Content,
                    IsReply = c.RepliedComment != null,
                    RepliedCommentId = c.RepliedCommentId,
                    ArticleId = c.ArticleId,
                    Article = new Article
                    {
                        Id = c.ArticleId,
                        Title = c.Article.Title,
                        Slug = c.Article.Slug
                    },
                    User = new ApplicationUser
                    {
                        Id = c.UserId,
                        UserName = c.User.UserName
                    },
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                });

            var count = await queryable.CountAsync();
            var comments = await queryable.ToListAsync();

            return Tuple.Create(count, comments);
        }
    }
}
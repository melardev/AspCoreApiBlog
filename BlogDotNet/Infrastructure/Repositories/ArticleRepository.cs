using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogDotNet.Data;
using BlogDotNet.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogDotNet.Infrastructure.Repositories
{
    public class ArticleRepository
    {
        private readonly ApplicationDbContext _db;

        public ArticleRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Article>> GetAll()
        {
            return await _db.Articles.ToListAsync();
        }

        public async Task<IEnumerable<Article>> GetByCategory(string category)
        {
            return await Task.FromResult(_db.Articles.Where(a =>
                    a.ArticleCategories.Any(ac => ac.Category.Name.Contains(category)))
                .Where(a => a.ArticlesTags.Any(t => t.Tag.Name.ToLower() == category.ToLower())));
        }

        public async Task<Article> GetById()
        {
            return null;
        }

        public async Task<IEnumerable<Article>> GetByAuthor(string name)
        {
            var articles = _db.Articles.Where(a => a.User.UserName == name);
            return await articles.ToListAsync();
        }

        public bool DeleteArticle(string id)
        {
            Article article = _db.Articles.Where(a => a.Id == id).FirstOrDefault();
            if (article != null)
            {
                _db.Articles.Remove(article);
                return true;
            }

            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlogDotNet.Dtos.Requests.Article;
using BlogDotNet.Dtos.Responses.Article;
using BlogDotNet.Entities;
using BlogDotNet.Models;
using BlogDotNet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogDotNet.Controllers
{
    // [Route("/Articles")]
    [Route("/api/Articles")]
    public class ArticlesController : Controller
    {
        private readonly IConfigurationService _settingsService;
        private readonly ArticlesService _articlesService;
        private IConfigurationService _configurationService;
        private IAuthorizationService _authorizationService;

        private readonly UsersService _usersService;
        

        public ArticlesController(IConfigurationService settingsService,
            IAuthorizationService authorizationService, ArticlesService articlesService,
            IConfigurationService configurationService, UsersService usersService)
        {
            _articlesService = articlesService;
            _configurationService = configurationService;
            _usersService = usersService;
            _authorizationService = authorizationService;
            _settingsService = settingsService;
        }


        [HttpGet("")]
        [ActionName(nameof(Index))]
        public async Task<IActionResult> Index(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var articles = await _articlesService.GetArticles(pageSize, pageSize);
            var basePath = Request.Path;

            return StatusCodeAndDtoWrapper.BuildSuccess(ArticleListDtoResponse.Build(articles.Item2, basePath,
                currentPage: page, pageSize: pageSize, totalItemCount: articles.Item1));
        }


        //[Authorize(Policy = "CreateArticlePolicy")]
        [HttpPost]
        [Authorize]
        // TODO: it should be only authors
        public async Task<ArticleDetailsDto> CreateArticle([FromBody] CreateOrEditArticleDto dto)
        {
            Article article = await _articlesService.Create(dto.Title, dto.Description, dto.Body,
                await _usersService.GetCurrentUserAsync(), dto.Tags);

            return ArticleDetailsDto.Build(article);
        }

        [HttpPut("{slug}")]
        // [Authorize(Policy = "EditArticlePolicy")]
        [Authorize]
        public async Task<IActionResult> UpdateArticle(string slug, [FromBody] CreateOrEditArticleDto dto)
        {
            if (!ModelState.IsValid)
                return StatusCodeAndDtoWrapper.BuilBadRequest(ModelState);


            Article article = await _articlesService.UpdateArticle(slug, dto);

            return StatusCodeAndDtoWrapper.BuildSuccess(ArticleDetailsDto.Build(article), "Updated successfully");
        }


        [Route("by_author/{name}")]
        public async Task<IActionResult> GetByAuthor(string name, int page = 1, int pageSize = 5)
        {
            Tuple<int, List<Article>> articles = await _articlesService.GetByAuthorName(name, page, pageSize);
            if (articles == null)
                return StatusCodeAndDtoWrapper.BuildGenericNotFound();

            return new StatusCodeAndDtoWrapper(ArticleListDtoResponse.Build(articles.Item2, "by_author/", page,
                pageSize, articles.Item1));
        }

        [HttpGet("search/{term}")]
//        [ActionName(nameof(GetBySearchTerm))]
        public async Task<IActionResult> GetBySearchTerm(string term, int page = 1, int pageSize = 5)
        {
            Tuple<int, List<Article>> result;
            if (!string.IsNullOrEmpty(term))
            {
                result = await _articlesService.GetBySearch(term, page, pageSize);
            }
            else
                result = await _articlesService.GetArticles(page, pageSize);

            return StatusCodeAndDtoWrapper.BuildSuccess(ArticleListDtoResponse.Build(result.Item2, "search/", page,
                pageSize, result.Item1));
        }


        [Route("/by_category/{category}")]
        public async Task<IActionResult> GetByCategory([FromRoute] string category, int page = 1, int pageSize = 5)
        {
            Tuple<int, List<Article>> articles = await _articlesService.GetArticlesByCategory(category, pageSize, page);

            return StatusCodeAndDtoWrapper.BuildSuccess(ArticleListDtoResponse.Build(articles.Item2,
                "/by_category/{category}", page, pageSize, articles.Item1));
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetArticleBySlug(string slug)
        {
            var article = await _articlesService.GetArticleBySlug(slug);
            if (article == null)
                return new StatusCodeAndDtoWrapper(404, new ErrorDtoResponse("Not Found"));

            //return NotFound();

            return new StatusCodeAndDtoWrapper(ArticleDetailsDto.Build(article));
        }
        
        [HttpGet("by_id/{id}")]
        public async Task<IActionResult> GetArticleById(string id)
        {
            var article = await _articlesService.GetArticleById(id);
            if (article == null)
                return new StatusCodeAndDtoWrapper(404, new ErrorDtoResponse("Not Found"));

            //return NotFound();

            return new StatusCodeAndDtoWrapper(ArticleDetailsDto.Build(article));
        }

        [HttpDelete]
        //[Authorize(Roles="Author")]
        public async Task<IActionResult> DeleteArticle(string id)
        {
            // Authorization

            var article = await _articlesService.GetArticleById(id);
            if (article == null)
                return NotFound();

            await _articlesService.Delete(article.Slug);
            return RedirectToAction(nameof(Index));
        }
    }
}
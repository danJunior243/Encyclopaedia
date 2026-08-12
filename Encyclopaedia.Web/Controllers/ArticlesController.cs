using Encyclopaedia.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Encyclopaedia.Web.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly EncyclopaediaDbContext _context;

        public ArticlesController(EncyclopaediaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? domaine, string? tri)
        {
            //Ici on affiche tous les articles avec leur traduction 
            var query = _context.ArticleTranslations
                .Include(t => t.Article)
                    .ThenInclude(a => a.Category)
                        .ThenInclude(c => c.Domain)
                            .ThenInclude(d => d.Translations)
                .Include(t => t.Article)
                    .ThenInclude(a => a.Category)
                        .ThenInclude(c => c.Translations)
                .Where(t => t.Article.Statut == Encyclopaedia.Core.Enums.ArticleStatus.Published)
                .AsQueryable();

            // Filtre par domaine
            if (!string.IsNullOrEmpty(domaine))
                query = query.Where(t => t.Article.Category.Domain.Slug == domaine);

            // Tri
            query = tri switch
            {
                "populaires" => query.OrderByDescending(t => t.Article.ViewCount),
                _ => query.OrderByDescending(t => t.Article.PublishAt)
            };

            var articles = await query.ToListAsync();

            ViewBag.Domaine = domaine;
            ViewBag.Tri = tri;
            ViewBag.TotalArticles = articles.Count;

            return View(articles);
        }
    }
}
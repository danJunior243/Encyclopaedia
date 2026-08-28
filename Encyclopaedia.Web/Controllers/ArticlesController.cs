using Encyclopaedia.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Encyclopaedia.Web.Helpers;


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
            // Récupérer la langue courante
            var currentLang = LanguageHelper.GetCurrentLanguage(Request);
            var language = await _context.Languages
                .FirstOrDefaultAsync(l => l.Code == currentLang)
                ?? await _context.Languages.FirstOrDefaultAsync(l => l.IsDefault);
            var langId = language?.LanguageId ?? 1;

            var query = _context.ArticleTranslations
                .Include(t => t.Article)
                    .ThenInclude(a => a.Category)
                        .ThenInclude(c => c.Domain)
                            .ThenInclude(d => d.Translations)
                .Include(t => t.Article)
                    .ThenInclude(a => a.Category)
                        .ThenInclude(c => c.Translations)
                .Include(t => t.Language)
                // Filtrer par langue courante
                .Where(t => t.LanguageId == langId)
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
            ViewBag.CurrentLang = currentLang;

            return View(articles);
        }
    }
}
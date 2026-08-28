using Encyclopaedia.Data;
using Encyclopaedia.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Encyclopaedia.Web.Controllers
{

    // Ce controller nous permet de gerer un seul article de notre encyclopedie

    public class ArticleController : Controller
    {
        private readonly EncyclopaediaDbContext _context;
        public ArticleController(EncyclopaediaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string slug, string lang = "fr")
        {
            // Récupérer la langue depuis le cookie ou l'URL
            var currentLang = LanguageHelper.GetCurrentLanguage(Request);

            var translation = await _context.ArticleTranslations
                .Include(t => t.Article)
                    .ThenInclude(a => a.Category)
                        .ThenInclude(c => c.Domain)
                            .ThenInclude(d => d.Translations)
                .Include(t => t.Article)
                    .ThenInclude(a => a.Category)
                        .ThenInclude(c => c.Translations)
                .Include(t => t.Language)
                .FirstOrDefaultAsync(t => t.Slug == slug && t.Language.Code == currentLang)
                ?? await _context.ArticleTranslations
                .Include(t => t.Article)
                    .ThenInclude(a => a.Category)
                        .ThenInclude(c => c.Domain)
                            .ThenInclude(d => d.Translations)
                .Include(t => t.Article)
                    .ThenInclude(a => a.Category)
                        .ThenInclude(c => c.Translations)
                .Include(t => t.Language)
                .FirstOrDefaultAsync(t => t.Slug == slug);

            if (translation == null)
                return NotFound();

            translation.Article.ViewCount++;
            await _context.SaveChangesAsync();

            var similarArticles = await _context.ArticleTranslations
                .Include(t => t.Article)
                .Where(t => t.Article.CategoryId == translation.Article.CategoryId
                        && t.Slug != slug
                        && t.Article.Statut == Encyclopaedia.Core.Enums.ArticleStatus.Published
                        && t.Language.Code == currentLang)
                .Take(4)
                .ToListAsync();

            ViewBag.SimilarArticles = similarArticles;
            ViewBag.CurrentLang = currentLang;

            return View(translation);
        }
    }
}

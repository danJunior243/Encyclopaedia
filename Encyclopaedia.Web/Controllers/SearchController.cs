using Encyclopaedia.Data;
using Encyclopaedia.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Encyclopaedia.Web.Controllers
{
    public class SearchController : Controller
    {

        private readonly EncyclopaediaDbContext _context;

        public SearchController(EncyclopaediaDbContext context)
        {
            _context = context;
        }


        // ── Autocomplete ──
        /// <summary>
        ///  Cette fonction est là pour proposer des suggestions lorsque l'on fait une recherche
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public async Task<IActionResult> Suggest(string q)
        {
            if (string.IsNullOrEmpty(q) || q.Length < 2)
                return Json(new List<string>());

            var suggestions = await _context.ArticleTranslations
                .Where(t => t.Title.Contains(q) &&
                            t.Article.Statut == Encyclopaedia.Core.Enums.ArticleStatus.Published)
                .Select(t => new { t.Title, t.Slug })
                .Take(5)
                .ToListAsync();

            return Json(suggestions);
        }

        public async Task<IActionResult> Index(string q)
        {// Récupérer la langue courante
            var currentLang = LanguageHelper.GetCurrentLanguage(Request);

            var language = await _context.Languages
                .FirstOrDefaultAsync(l => l.Code == currentLang)
                ?? await _context.Languages.FirstOrDefaultAsync(l => l.IsDefault);

            var langId = language?.LanguageId ?? 1;

            if (string.IsNullOrEmpty(q))
                return View(new List<Encyclopaedia.Core.Entities.ArticleTranslation>());

            var results = await _context.ArticleTranslations
                .Include(t => t.Article)
                    .ThenInclude(a => a.Category)
                        .ThenInclude(c => c.Domain)
                            .ThenInclude(d => d.Translations)
                .Include(t => t.Language)
                .Where(t => t.LanguageId == langId)
                .Where(t => t.Title.Contains(q) ||
                            t.Summary.Contains(q) ||
                            t.Content.Contains(q))
                .Where(t => t.Article.Statut == Encyclopaedia.Core.Enums.ArticleStatus.Published)
                .ToListAsync();

            ViewData["Query"] = q;
            return View(results);

        }
    }
}

using Encyclopaedia.Data;
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

        public async Task<IActionResult> Index(string q)
        {
            // si la query est vide, on retourne une vue avec une liste vide d'articles
            if (string.IsNullOrEmpty(q))
                return View(new List<Encyclopaedia.Core.Entities.ArticleTranslation>());


            var results = await _context.ArticleTranslations
                .Include(t => t.Article)
                    .ThenInclude(a => a.Category)
                        .ThenInclude(c => c.Domain)
                            .ThenInclude(d => d.Translations)
                .Include(t => t.Language)
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

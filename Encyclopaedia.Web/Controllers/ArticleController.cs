using Encyclopaedia.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Encyclopaedia.Web.Controllers
{


    public class ArticleController : Controller
    {
        private readonly EncyclopaediaDbContext _context;
        public ArticleController(EncyclopaediaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string slug, string lang = "fr")
        {
            // On charge la traduction de l'article, sa catégorie, son domaine et les traductions du domaine et de la catégorie
            var translation = await _context.ArticleTranslations
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

            // Incrémenter le nombre de vues
            translation.Article.ViewCount++;
            await _context.SaveChangesAsync();

            // Articles similaires — même catégorie
            var similarArticles = await _context.ArticleTranslations
                .Include(t => t.Article)
                .Where(t => t.Article.CategoryId == translation.Article.CategoryId
                        && t.Slug != slug
                        && t.Article.Statut == Encyclopaedia.Core.Enums.ArticleStatus.Published)
                .Take(4)
                .ToListAsync();

            ViewBag.SimilarArticles = similarArticles;

            return View(translation);
        }
    }
}

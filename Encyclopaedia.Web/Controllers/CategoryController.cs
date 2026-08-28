using Encyclopaedia.Core.Entities;
using Encyclopaedia.Data;
using Encyclopaedia.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Encyclopaedia.Web.Controllers
{
    public class CategoryController : Controller
    {

        // On creer et on  initialise le contexte de la base des données
        private readonly EncyclopaediaDbContext _context;

        // Le constructeur de la classe CategoryController prend un paramètre de type EncyclopaediaDbContext, qui est injecté par le framework ASP.NET Core.
        public CategoryController(EncyclopaediaDbContext context)
        {
            _context = context;
        }

        // L'action Index est une méthode asynchrone qui prend un paramètre slug de type string.
        // Elle utilise ce slug pour rechercher une catégorie spécifique dans la base de données.
        // On utilise Include pour charger les traductions de la catégorie, le domaine associé et
        // les articles liés à cette catégorie, ainsi que leurs traductions.
        public async Task<IActionResult> Index(string slug)
        {
            // Récupérer la langue courante
            var currentLang = LanguageHelper.GetCurrentLanguage(Request);
            var language = await _context.Languages
                .FirstOrDefaultAsync(l => l.Code == currentLang)
                ?? await _context.Languages.FirstOrDefaultAsync(l => l.IsDefault);
            var langId = language?.LanguageId ?? 1;

            var categorie = await _context.Categories
                .Include(c => c.Translations.Where(t => t.LanguageId == langId))
                .Include(c => c.Domain)
                    .ThenInclude(d => d.Translations.Where(t => t.LanguageId == langId))
                .Include(c => c.Articles)
                    .ThenInclude(a => a.Translations.Where(t => t.LanguageId == langId))
                .FirstOrDefaultAsync(c => c.Slug == slug);

            if (categorie == null)
                return NotFound();

            ViewBag.CurrentLang = currentLang;
            return View("~/Views/CategoryPublic/Index.cshtml", categorie);
        }
    }
}

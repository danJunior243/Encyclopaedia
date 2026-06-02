using Encyclopaedia.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Encyclopaedia.Web.Controllers
{
    public class ArticleController : Controller
    {
        // Injection de dépendance du contexte de base de données pour accéder aux données des articles.
        private readonly EncyclopaediaDbContext _context;

        // Initialisation du contrôleur avec le contexte de base de données injecté via le constructeur.
        public ArticleController(EncyclopaediaDbContext context)
        {
            _context = context;
        }

        // On utilise article tranlsation car ceci contient le slug et la langue, ce qui nous permet de récupérer l'article dans la langue souhaitée.
        // Si on passait par article alors on aurait pas le contenu de l'article dans la langue souhaitée, et on devrait faire une requete supplémentaire pour récupérer la traduction.

        public async Task<IActionResult> Index(string slug, string lang = "fr")
        {
            var translation = await _context.ArticleTranslations
                .Include(t => t.Article)
                    .ThenInclude(a => a.Category)
                        .ThenInclude(c => c.Domain)
                .Include(t => t.Language)
                .FirstOrDefaultAsync(t => t.Slug == slug);

            if (translation == null)
                return NotFound();

            // Incrémenter le nombre de vues
            translation.Article.ViewCount++;
            await _context.SaveChangesAsync();

            return View(translation);
        }
    }
}

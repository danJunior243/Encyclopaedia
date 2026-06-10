using Encyclopaedia.Core.Entities;
using Encyclopaedia.Data;
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
            var categorie = await _context.Categories
                .Include(c => c.Translations)
                .Include(c => c.Domain)
                    .ThenInclude(d => d.Translations)
                .Include(c => c.Articles)
                           .ThenInclude(a => a.Translations)
                .FirstOrDefaultAsync(c => c.Slug == slug);

            if (categorie == null)
                return NotFound();

            return View("~/Views/CategoryPublic/Index.cshtml", categorie);
        }
    }
}

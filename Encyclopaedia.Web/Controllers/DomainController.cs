using Encyclopaedia.Data;
using Encyclopaedia.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Encyclopaedia.Web.Controllers
{
    public class DomainController : Controller
    {
        private readonly EncyclopaediaDbContext _context;

        public DomainController(EncyclopaediaDbContext context)
        {
            _context = context;
        }

        

             public async Task<IActionResult> Index(string slug)
            {
                // Récupérer la langue courante
                var currentLang = LanguageHelper.GetCurrentLanguage(Request);
                var language = await _context.Languages
                    .FirstOrDefaultAsync(l => l.Code == currentLang)
                    ?? await _context.Languages.FirstOrDefaultAsync(l => l.IsDefault);
                var langId = language?.LanguageId ?? 1;

                var domain = await _context.Domains
                    .Include(d => d.Translations.Where(t => t.LanguageId == langId))
                    .Include(d => d.Categories)
                        .ThenInclude(c => c.Translations.Where(t => t.LanguageId == langId))
                    .Include(d => d.Categories)
                        .ThenInclude(c => c.Articles)
                            .ThenInclude(a => a.Translations.Where(t => t.LanguageId == langId))
                    .FirstOrDefaultAsync(d => d.Slug == slug);

                if (domain == null)
                    return NotFound();

                ViewBag.CurrentLang = currentLang;
                return View("~/Views/DomainPublic/Index.cshtml", domain);
             }
    }
} 
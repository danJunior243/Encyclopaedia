using Encyclopaedia.Core.Entities;
using Encyclopaedia.Data;
using Encyclopaedia.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Encyclopaedia.Web.Controllers.Admin
{
    public class AdminDomainController : Controller
    {

        // On creer et on  initialise le contexte de la base des données 

        private readonly EncyclopaediaDbContext _context;

        public AdminDomainController(EncyclopaediaDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var domains = await _context.Domains
                .Include(d => d.Translations)
                .OrderBy(d => d.SortOrder)
                .ToListAsync();

            return View("~/Views/Domain/Index.cshtml", domains);
        }

        // ── GET /Domain/Create ──
        public IActionResult Create()
        {
            return View("~/Views/AdminDomain/Create.cshtml", new DomainViewModel());
        }

        // ── POST /Domain/Create ──
        [HttpPost]
        public async Task<IActionResult> Create(DomainViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/AdminDomain/Create.cshtml", model);

            var domain = new Domain
            {
                Slug = model.Slug.ToLower().Replace(" ", "-"),
                SortOrder = model.SortOrder
            };

            _context.Domains.Add(domain);
            await _context.SaveChangesAsync();

            // Ajouter les traductions
            if (!string.IsNullOrEmpty(model.NameFr))
                _context.DomainTranslations.Add(new DomainTranslation { DomainId = domain.DomainId, LanguageId = 1, Name = model.NameFr });

            if (!string.IsNullOrEmpty(model.NameEn))
                _context.DomainTranslations.Add(new DomainTranslation { DomainId = domain.DomainId, LanguageId = 2, Name = model.NameEn });

            if (!string.IsNullOrEmpty(model.NameAr))
                _context.DomainTranslations.Add(new DomainTranslation { DomainId = domain.DomainId, LanguageId = 3, Name = model.NameAr });

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



    }
}

using Encyclopaedia.Data;
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
            var domain = await _context.Domains
                .Include(d => d.Translations)
                .Include(d => d.Categories)
                    .ThenInclude(c => c.Translations)
                .Include(d => d.Categories)
                    .ThenInclude(c => c.Articles)
                        .ThenInclude(a => a.Translations)
                .FirstOrDefaultAsync(d => d.Slug == slug);

            if (domain == null)
                return NotFound();

            return View("~/Views/DomainPublic/Index.cshtml",domain);
        }
    }
} 
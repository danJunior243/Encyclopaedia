using Encyclopaedia.Core.Enums;
using Encyclopaedia.Data;
using Encyclopaedia.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Encyclopaedia.Web.Controllers.Admin
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly EncyclopaediaDbContext _context;

        public DashboardController(EncyclopaediaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                // On compte le nom total d'articles dans la base de données et on le stocke dans une variable du modèle de vue
                TotalArticles = await _context.Articles.CountAsync(),

                // On regroupe les articles par statut, on compte le nombre d'articles dans chaque groupe,
                // et on stocke le résultat dans un dictionnaire du modèle de vue
                ArticlesByStatus = await _context.Articles
                    .GroupBy(a => a.Statut)
                    .ToDictionaryAsync(g => g.Key, g => g.Count()),
                // On regroupe les articles par domaine, on compte le nombre d'articles dans chaque groupe,
                // et on stocke le résultat dans un dictionnaire du modèle de vue
                ArticlesByDomain = await _context.Articles
                    .GroupBy(a => a.Category.Domain.Slug)
                    .ToDictionaryAsync(g => g.Key, g => g.Count()),
                // On récupère les 5 articles les plus récemment mis à jour, triés par date de mise à jour décroissante,

                RecentlyUpdated = await _context.Articles
                    .OrderByDescending(a => a.LastUpdatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}
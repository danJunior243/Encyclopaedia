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
                TotalArticles = await _context.Articles.CountAsync(),

                ArticlesByStatus = await _context.Articles
            .GroupBy(a => a.Statut)
            .ToDictionaryAsync(g => g.Key, g => g.Count()),

                ArticlesByDomain = await _context.Articles
            .GroupBy(a => a.Category.Domain.Slug)
            .ToDictionaryAsync(g => g.Key, g => g.Count()),

                RecentlyUpdated = await _context.Articles
            .OrderByDescending(a => a.LastUpdatedAt)
            .Take(5)
            .ToListAsync()
            };

            // Données pour les graphiques
            ViewBag.DomainLabels = viewModel.ArticlesByDomain.Keys.ToList();
            ViewBag.DomainData = viewModel.ArticlesByDomain.Values.ToList();

            ViewBag.StatusLabels = viewModel.ArticlesByStatus.Keys.Select(s => s.ToString()).ToList();
            ViewBag.StatusData = viewModel.ArticlesByStatus.Values.ToList();

            return View(viewModel);
        }
    }
}
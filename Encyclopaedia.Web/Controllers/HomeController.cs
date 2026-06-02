using Encyclopaedia.Core.Enums;
using Encyclopaedia.Data;
using Encyclopaedia.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Encyclopaedia.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EncyclopaediaDbContext _context;

        public HomeController(ILogger<HomeController> logger, EncyclopaediaDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Cette action retourne la vue principale de l'application, généralement la page d'accueil.
        public IActionResult Index()
        {
            // ici on recupere les 5 articles les plus récents et
            // les 5 articles les plus populaires (en fonction du nombre de vues) pour les afficher sur la page d'accueil.
            var viewModel = new HomeViewModel
            {
          RecentArticles = _context.Articles
            .Include(a => a.Category)
            .ThenInclude(c => c.Domain)
             .Include(a => a.Translations)
            .Where(a => a.Statut == ArticleStatus.Published)
            .OrderByDescending(a => a.PublishAt)
            .Take(5)
            .ToList(),

           PopularArticles = _context.Articles
            .Include(a => a.Category)
            .ThenInclude(c => c.Domain)
            .Include(a => a.Translations)
            .Where(a => a.Statut == ArticleStatus.Published)
            .OrderByDescending(a => a.ViewCount)
            .Take(5)
            .ToList()
            };

            //retourne la vue associée à cette action, qui est généralement située dans Views/Home/Index.cshtml.
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

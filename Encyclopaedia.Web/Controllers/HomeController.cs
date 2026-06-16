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
        public async Task <IActionResult> Index()
        {
            // ici on recupere les 5 articles les plus récents et
            // les 5 articles les plus populaires (en fonction du nombre de vues) pour les afficher sur la page d'accueil.
            var viewModel = new HomeViewModel
            {
              RecentArticles = await _context.Articles
                .Include(a => a.Category)
                    .ThenInclude(c => c.Domain)
                         .ThenInclude(d => d.Translations)
                .Include(a => a.Translations)
                .Where(a => a.Statut == ArticleStatus.Published)
                .OrderByDescending(a => a.PublishAt)
                .Take(5)
                .ToListAsync(),

               PopularArticles = await _context.Articles
                .Include(a => a.Category)
                   .ThenInclude(c => c.Domain)
                       .ThenInclude(d => d.Translations)
                .Include(a => a.Translations)
                .Where(a => a.Statut == ArticleStatus.Published)
                .OrderByDescending(a => a.ViewCount)
                .Take(5)
                .ToListAsync()
           
            };

            ViewBag.TotalArticles = await _context.Articles
               .Where(a => a.Statut == ArticleStatus.Published)
               .CountAsync();

            // Article aléatoire pour "Le saviez-vous ?"
            var publishedArticles = await _context.ArticleTranslations
                .Include(t => t.Article)
                .Where(t => t.Article.Statut == ArticleStatus.Published)
                .ToListAsync();

            if (publishedArticles.Any())
            {
                var random = new Random();
                var randomArticle = publishedArticles[random.Next(publishedArticles.Count)];
                ViewBag.RandomArticle = randomArticle;
                ViewBag.RandomSummary = randomArticle.Summary;
                ViewBag.RandomSlug = randomArticle.Slug;
            }
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

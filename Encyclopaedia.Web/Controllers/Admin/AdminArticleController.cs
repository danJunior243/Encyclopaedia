
using Encyclopaedia.Core.Enums;
using Encyclopaedia.Data;
using Encyclopaedia.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Slugify;

namespace Encyclopaedia.Web.Controllers.Admin
{
    [Authorize]
    public class AdminArticleController : Controller
    {
        private readonly EncyclopaediaDbContext _context;

        public AdminArticleController(EncyclopaediaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? filtreStatut)
        {
            var query = _context.Articles
                .Include(a => a.Category)
                .ThenInclude(c => c.Domain)
                .Include(a => a.Translations)
                .AsQueryable();

            // Appliquer le filtre si présent
            if (!string.IsNullOrEmpty(filtreStatut) &&
                Enum.TryParse<ArticleStatus>(filtreStatut, out var statut))
            {
                query = query.Where(a => a.Statut == statut);
            }

            var articles = await query
                .OrderByDescending(a => a.LastUpdatedAt)
                .ToListAsync();

            var viewModel = new ArticleAdminListViewModel
            {
                TotalArticles = await _context.Articles.CountAsync(),
                FiltreStatut = filtreStatut,
                Articles = articles.Select(a => new ArticleListViewModel
                {
                    Id = a.ArticleId,
                    Title = a.Translations.FirstOrDefault()?.Title ?? "Sans titre",
                    Domain = a.Category.Domain.Slug,
                    Category = a.Category.Slug,
                    Status = a.Statut,
                    LastUpdatedAt = a.LastUpdatedAt,
                    ViewCount = a.ViewCount
                }).ToList()
            };

            return View( viewModel);
        }

        // ── GET /AdminArticle/Create ──
        public async Task<IActionResult> Create()
        {
            var viewModel = new ArticleCreateViewModel
            {
                Categories = await _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Slug
                    }).ToListAsync(),

                Languages = await _context.Languages
                    .Where(l => l.IsActive)
                    .Select(l => new SelectListItem
                    {
                        Value = l.LanguageId.ToString(),
                        Text = l.Name
                    }).ToListAsync()
            };

            return View("~/Views/AdminArticle/Create.cshtml", viewModel);
        }

        // ── POST /AdminArticle/Create ──
        [HttpPost]
        public async Task<IActionResult> Create(ArticleCreateViewModel model)
        {

            // Si le formulaire n'est pas valide, on retourne la vue avec les données saisies
            if (!ModelState.IsValid)
                return View("~/Views/AdminArticle/Create.cshtml", model);
            //On crée l'article avec les données de base

            var article = new Encyclopaedia.Core.Entities.Article
            {
                CategoryId = model.CategoryId,
                AuthorId = 1, // temporaire
                Statut = Encyclopaedia.Core.Enums.ArticleStatus.Draft,
                FeaturedImage = model.FeaturedImage,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            // On crée la traduction de l'article avec les données du formulaire
             var slugHelper = new Slugify.SlugHelper();

            var translation = new Encyclopaedia.Core.Entities.ArticleTranslation
            {
                ArticleId = article.ArticleId,
                LanguageId = model.LanguageId,
                Title = model.Title,
                Summary = model.Summary,
                Content = model.Content,
               
                Slug = slugHelper.GenerateSlug(model.Title)
            };
            // On ajoute la traduction à la base de données

            _context.ArticleTranslations.Add(translation);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        // ── GET /AdminArticle/Edit/5 ──
        public async Task<IActionResult> Edit(int id)
        {
            var article = await _context.Articles
                .Include(a => a.Translations)
                .FirstOrDefaultAsync(a => a.ArticleId == id);

            if (article == null)
                return NotFound();

            var translation = article.Translations.FirstOrDefault();

            var model = new ArticleCreateViewModel
            {
                Title = translation?.Title ?? "",
                Summary = translation?.Summary ?? "",
                Content = translation?.Content ?? "",
                CategoryId = article.CategoryId,
                LanguageId = translation?.LanguageId ?? 1,
                FeaturedImage = article.FeaturedImage,

                Categories = await _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Slug
                    }).ToListAsync(),

                Languages = await _context.Languages
                    .Where(l => l.IsActive)
                    .Select(l => new SelectListItem
                    {
                        Value = l.LanguageId.ToString(),
                        Text = l.Name
                    }).ToListAsync()
            };

            ViewBag.ArticleId = id;
            return View("~/Views/AdminArticle/Edit.cshtml", model);
        }

        // ── POST /AdminArticle/Edit/5 ──
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ArticleCreateViewModel model)
        {

            // ici si le formulaire n'est pas valide on retourne la vue
            if (!ModelState.IsValid)
            {
                ViewBag.ArticleId = id;
                return View("~/Views/AdminArticle/Edit.cshtml", model);
            }

            // Ici on recupère l'article en cherchant si l'id correspond
            var article = await _context.Articles
                .Include(a => a.Translations)
                .FirstOrDefaultAsync(a => a.ArticleId == id);

            // Si l'article n'existe pas alors on retourne not found cad rien
            if (article == null)
                return NotFound();
            // On modifie l'article

            article.CategoryId = model.CategoryId;
            article.FeaturedImage = model.FeaturedImage;
            article.LastUpdatedAt = DateTime.UtcNow;
            //on modifie la traduction aussi en verificant si le id de la langue correspond à l'objet

            var translation = article.Translations.FirstOrDefault(t => t.LanguageId == model.LanguageId);
            if (translation != null)
            {
                translation.Title = model.Title;
                translation.Summary = model.Summary;
                translation.Content = model.Content;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // ── Supprimer un article ──
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _context.Articles
                .Include(a => a.Translations)
                .FirstOrDefaultAsync(a => a.ArticleId == id);

            if (article == null)
                return NotFound();
            // il faut supprimer les traductions avant de supprimer l'article pour éviter les problèmes de clé étrangère
            //  Et on utilise RemoveRange pour supprimer toutes les traductions de l'article en une seule opération 

            _context.ArticleTranslations.RemoveRange(article.Translations);
            _context.Articles.Remove(article);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // ── Publier un article ──
        public async Task<IActionResult> Publish(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
                return NotFound();

            article.Statut = Encyclopaedia.Core.Enums.ArticleStatus.Published;
            article.PublishAt = DateTime.UtcNow;
            article.LastUpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



    }
}
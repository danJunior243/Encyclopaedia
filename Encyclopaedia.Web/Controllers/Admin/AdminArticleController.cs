
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


        // ── Générer avec IA ──
        [HttpPost]
        public async Task<IActionResult> GenerateWithAI([FromBody] GenerateRequest request)
        {
            try
            {
                var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
                if (string.IsNullOrEmpty(apiKey))
                    return Json(new { success = false, error = "Clé API manquante" });

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);
                client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                    var prompt = $@"Tu es un rédacteur encyclopédique expert. Rédige un article encyclopédique complet et précis sur le sujet suivant : {request.Subject}

                                Réponds UNIQUEMENT en JSON sans backticks avec ce format exact :
                                {{
                                  ""summary"": ""Un résumé de 2-3 phrases maximum"",
                                  ""content"": ""Le contenu HTML complet de l'article avec des paragraphes <p>, des titres <h2>, des listes <ul> si nécessaire. Minimum 400 mots."",
                                  ""imageQuery"": ""Un mot-clé en anglais pour trouver une image sur Unsplash (ex: biology, history, ocean)""
                                }}";

                var body = new
                {
                    model = "claude-haiku-4-5-20251001",
                    max_tokens = 2000,
                    messages = new[]
                    {
                new { role = "user", content = prompt }
            }
                };

                var response = await client.PostAsync(
                    "https://api.anthropic.com/v1/messages",
                    new StringContent(System.Text.Json.JsonSerializer.Serialize(body),
                        System.Text.Encoding.UTF8, "application/json")
                );

                var responseText = await response.Content.ReadAsStringAsync();
                var responseObj = System.Text.Json.JsonDocument.Parse(responseText);
                var content = responseObj.RootElement
                    .GetProperty("content")[0]
                    .GetProperty("text")
                    .GetString();

                // Nettoyer la réponse de Claude
                var cleanContent = content!
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                // Parser le JSON
                var articleData = System.Text.Json.JsonDocument.Parse(cleanContent);
                var summary = articleData.RootElement.GetProperty("summary").GetString();
                var articleContent = articleData.RootElement.GetProperty("content").GetString();
                var imageQuery = articleData.RootElement.GetProperty("imageQuery").GetString();

                // Récupérer une image depuis Unsplash
                var unsplashKey = Environment.GetEnvironmentVariable("UNSPLASH_ACCESS_KEY");
                var imageUrl = "https://images.unsplash.com/photo-1481627834876-b7833e8f5570?w=1200&q=80";

                if (!string.IsNullOrEmpty(unsplashKey) && !string.IsNullOrEmpty(imageQuery))
                {
                    try
                    {
                        var unsplashClient = new HttpClient();
                        var unsplashResponse = await unsplashClient.GetAsync(
                            $"https://api.unsplash.com/photos/random?query={Uri.EscapeDataString(imageQuery)}&client_id={unsplashKey}&orientation=landscape"
                        );
                        var unsplashText = await unsplashResponse.Content.ReadAsStringAsync();
                        var unsplashData = System.Text.Json.JsonDocument.Parse(unsplashText);
                        imageUrl = unsplashData.RootElement
                            .GetProperty("urls")
                            .GetProperty("regular")
                            .GetString() ?? imageUrl;
                    }
                    catch { }
                }

                return Json(new { success = true, summary, content = articleContent, imageUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public class GenerateRequest
        {
            public string Subject { get; set; } = string.Empty;
        }


    }
}
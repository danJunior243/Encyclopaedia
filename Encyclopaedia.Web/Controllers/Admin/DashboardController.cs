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

        public class TranslateAllRequest
        {
            public string TargetLang { get; set; } = string.Empty;
        }


        // ── Traduire tous les articles en masse ──
        [HttpPost]
        public async Task<IActionResult> TranslateAll([FromBody] TranslateAllRequest request)
        {
            try
            {
                var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
                if (string.IsNullOrEmpty(apiKey))
                    return Json(new { success = false, error = "Clé API manquante" });

                var targetLanguage = await _context.Languages
                    .FirstOrDefaultAsync(l => l.Code == request.TargetLang);

                if (targetLanguage == null)
                    return Json(new { success = false, error = "Langue cible introuvable" });

                // Récupérer tous les articles publiés sans traduction dans la langue cible
                var articlesWithoutTranslation = await _context.Articles
                    .Include(a => a.Translations)
                    .Where(a => a.Statut == Encyclopaedia.Core.Enums.ArticleStatus.Published)
                    .Where(a => !a.Translations.Any(t => t.LanguageId == targetLanguage.LanguageId))
                    .ToListAsync();

                var translated = 0;
                var errors = 0;

                foreach (var article in articlesWithoutTranslation)
                {
                    // Récupérer la traduction française
                    var frTranslation = article.Translations.FirstOrDefault(t => t.LanguageId == 1)
                        ?? article.Translations.FirstOrDefault();
                    if (frTranslation == null) continue;

                    try
                    {
                        var client = new HttpClient();
                        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
                        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                        // Détecter la langue source
                        var sourceLang = frTranslation.LanguageId == 1 ? "French" : "English";
                        var langName = request.TargetLang == "en" ? "English" : request.TargetLang == "fr" ? "French" : "Arabic";

                        var prompt = $@"Translate the following encyclopedia article from {sourceLang} to {langName}.
                            Return ONLY a JSON object without backticks in this exact format:
                            {{
                              ""title"": ""translated title"",
                              ""summary"": ""translated summary"",
                              ""content"": ""translated HTML content""
                            }}

                            {sourceLang} title: {frTranslation.Title}
                            {sourceLang} summary: {frTranslation.Summary}
                            {sourceLang} content: {frTranslation.Content}";
                        var body = new
                        {
                            model = "claude-haiku-4-5-20251001",
                            max_tokens = 3000,
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

                        var cleanContent = content!
                            .Replace("```json", "")
                            .Replace("```", "")
                            .Trim();

                        var translationData = System.Text.Json.JsonDocument.Parse(cleanContent);
                        var translatedTitle = translationData.RootElement.GetProperty("title").GetString();
                        var translatedSummary = translationData.RootElement.GetProperty("summary").GetString();
                        var translatedContent = translationData.RootElement.GetProperty("content").GetString();

                        var slugHelper = new Slugify.SlugHelper();
                        var slug = slugHelper.GenerateSlug(translatedTitle ?? frTranslation.Title);

                        var newTranslation = new Encyclopaedia.Core.Entities.ArticleTranslation
                        {
                            ArticleId = article.ArticleId,
                            LanguageId = targetLanguage.LanguageId,
                            Title = translatedTitle ?? frTranslation.Title,
                            Summary = translatedSummary ?? frTranslation.Summary,
                            Content = translatedContent ?? frTranslation.Content,
                            Slug = slug
                        };

                        _context.ArticleTranslations.Add(newTranslation);
                        await _context.SaveChangesAsync();
                        translated++;

                        // Pause pour éviter de dépasser les limites de l'API
                        await Task.Delay(500);
                    }
                    catch
                    {
                        errors++;
                    }
                }

                return Json(new
                {
                    success = true,
                    message = $"{translated} articles traduits, {errors} erreurs",
                    translated,
                    errors
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

    }
}
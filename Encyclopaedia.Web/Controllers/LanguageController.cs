using Encyclopaedia.Data;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Encyclopaedia.Web.Controllers
{
    public class LanguageController : Controller
    {

        private readonly EncyclopaediaDbContext _context;

        public LanguageController(EncyclopaediaDbContext context)
        {
            _context = context;
        }
        // ── Changer la langue de l'utilisateur ──
        public async Task< IActionResult> Set(string lang, string returnUrl = "/")
        {
            // Sauvegarder la langue dans un cookie
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(lang)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            // Si on est sur une page article — trouver la traduction
            if (returnUrl.StartsWith("/article/"))
            {
                var currentSlug = returnUrl.Replace("/article/", "");

                // Trouver l'article via le slug actuel
                var currentTranslation = await _context.ArticleTranslations
                    .FirstOrDefaultAsync(t => t.Slug == currentSlug);

                if (currentTranslation != null)
                {
                    // Chercher la traduction dans la nouvelle langue
                    var targetLanguage = await _context.Languages
                        .FirstOrDefaultAsync(l => l.Code == lang);

                    if (targetLanguage != null)
                    {
                        var targetTranslation = await _context.ArticleTranslations
                            .FirstOrDefaultAsync(t => t.ArticleId == currentTranslation.ArticleId
                                && t.LanguageId == targetLanguage.LanguageId);

                        if (targetTranslation != null)
                            return LocalRedirect($"/article/{targetTranslation.Slug}");
                    }
                }
            }

            // Rediriger vers la page précédente ou l'accueil
            if (string.IsNullOrEmpty(returnUrl) || !returnUrl.StartsWith("/"))
                returnUrl = "/";

            return LocalRedirect(returnUrl);
        }
    }
}

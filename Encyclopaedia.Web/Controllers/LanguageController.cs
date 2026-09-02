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

            if (returnUrl.StartsWith("/article/"))
            {
                var currentSlug = returnUrl.Replace("/article/", "");
                var currentTranslation = await _context.ArticleTranslations
                    .FirstOrDefaultAsync(t => t.Slug == currentSlug);

                if (currentTranslation == null)
                    return LocalRedirect($"/?debug=slug-not-found-{currentSlug}");

                var targetLanguage = await _context.Languages
                    .FirstOrDefaultAsync(l => l.Code == lang);

                if (targetLanguage == null)
                    return LocalRedirect($"/?debug=lang-not-found-{lang}");

                var targetTranslation = await _context.ArticleTranslations
                    .FirstOrDefaultAsync(t => t.ArticleId == currentTranslation.ArticleId
                        && t.LanguageId == targetLanguage.LanguageId);

                if (targetTranslation == null)
                    return LocalRedirect($"/?debug=no-translation-for-article-{currentTranslation.ArticleId}-lang-{targetLanguage.LanguageId}");

                return LocalRedirect($"/article/{targetTranslation.Slug}");
            }

            if (string.IsNullOrEmpty(returnUrl) || !returnUrl.StartsWith("/"))
                returnUrl = "/";

            return LocalRedirect(returnUrl);
        }
    }
}

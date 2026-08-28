using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Encyclopaedia.Web.Controllers
{
    public class LanguageController : Controller
    {
        // ── Changer la langue de l'utilisateur ──
        public IActionResult Set(string lang, string returnUrl = "/")
        {
            // Sauvegarder la langue dans un cookie
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(lang)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            // Rediriger vers la page précédente
            return LocalRedirect(returnUrl);
        }
    }
}

using System.Net;
using System.Text.RegularExpressions;

namespace Encyclopaedia.Web.Helpers
{
    public class LanguageHelper
    {
        /// <summary>
        /// Récupère la langue courante depuis le cookie de culture ASP.NET
        /// Retourne "fr" par défaut si aucun cookie n'est trouvé
        /// </summary>
        public static string GetCurrentLanguage(HttpRequest request)
        {
            var cultureCookie = request.Cookies[".AspNetCore.Culture"];
            if (cultureCookie == null) return "fr";

            var parts = cultureCookie.Split("|");
            foreach (var part in parts)
            {
                if (part.StartsWith("c="))
                    return part.Substring(2);
            }

            return "fr";
        }


    }
}

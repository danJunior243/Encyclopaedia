using Microsoft.AspNetCore.Mvc;

namespace Encyclopaedia.Web.Controllers.Admin
{
    public class ArticleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

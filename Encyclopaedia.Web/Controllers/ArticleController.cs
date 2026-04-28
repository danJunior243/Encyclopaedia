using Microsoft.AspNetCore.Mvc;

namespace Encyclopaedia.Web.Controllers
{
    public class ArticleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

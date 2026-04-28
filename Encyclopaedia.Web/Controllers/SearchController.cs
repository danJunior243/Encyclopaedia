using Microsoft.AspNetCore.Mvc;

namespace Encyclopaedia.Web.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

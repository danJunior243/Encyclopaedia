using Microsoft.AspNetCore.Mvc;

namespace Encyclopaedia.Web.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

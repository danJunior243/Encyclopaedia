using Microsoft.AspNetCore.Mvc;

namespace Encyclopaedia.Web.Controllers.Admin
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

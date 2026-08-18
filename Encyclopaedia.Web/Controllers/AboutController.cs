using Microsoft.AspNetCore.Mvc;

namespace Encyclopaedia.Web.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

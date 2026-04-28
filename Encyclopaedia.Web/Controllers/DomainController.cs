using Microsoft.AspNetCore.Mvc;

namespace Encyclopaedia.Web.Controllers
{
    public class DomainController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

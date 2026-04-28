using Microsoft.AspNetCore.Mvc;

namespace Encyclopaedia.Web.Controllers.Admin
{
    public class DomainController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

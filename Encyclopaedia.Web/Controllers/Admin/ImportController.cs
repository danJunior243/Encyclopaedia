using Microsoft.AspNetCore.Mvc;

namespace Encyclopaedia.Web.Controllers.Admin
{
    public class ImportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

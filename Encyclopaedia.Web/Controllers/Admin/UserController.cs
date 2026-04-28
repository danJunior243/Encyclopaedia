using Microsoft.AspNetCore.Mvc;

namespace Encyclopaedia.Web.Controllers.Admin
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

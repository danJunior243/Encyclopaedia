using Encyclopaedia.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Encyclopaedia.Web.Models;

namespace Encyclopaedia.Web.Controllers
{
    public class AccountController : Controller
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // ── GET /Account/Login ──
        public IActionResult Login()
        {
            return View();

        }

        // ── POST /Account/Login ──
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // On tente de connecter l'utilisateur avec les informations fournies.
            // On recupere l'utilisateur par son email pour vérifier s'il existe avant de tenter la connexion.

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, false);

            // Si la connexion est réussie, on redirige vers la page d'accueil.
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Email ou mot de passe incorrect.");
            return View(model);
        }

        // ── Logout ──
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }




    }
}

using Encyclopaedia.Core.Entities;
using Encyclopaedia.Data;
using Encyclopaedia.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Encyclopaedia.Web.Controllers.Admin
{
    public class AdminCategoryController : Controller
    {

        private readonly EncyclopaediaDbContext _context;

        public AdminCategoryController(EncyclopaediaDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult>  Index()
        {

            var categories = await _context.Categories
                .Include(c => c.Domain)
                .Include(c => c.Translations)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            return View("~/Views/AdminCategory/Index.cshtml", categories);



        }

        public async Task<IActionResult> Create()
        {

            //On charge la liste des domaines et des catégories parents pour les afficher dans les dropdowns

            var model = new CategoryViewModel
            {
                Domains = await _context.Domains
                    .Select(d => new SelectListItem
                    {
                        Value = d.DomainId.ToString(),
                        Text = d.Slug
                    }).ToListAsync(),

                ParentCategories = await _context.Categories
                    .Include(c => c.Translations)
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Slug
                    }).ToListAsync()
            };
            return View("~/Views/AdminCategory/Create.cshtml", model);
        }
        [HttpPost]

        public async Task<IActionResult> Create(CategoryViewModel model)
        {

            if (!ModelState.IsValid)
                return View("~/Views/AdminCategory/Create.cshtml", model);

            var category = new Category
            {
                DomainId = model.DomainId,
                ParentId = model.ParentId,
                Slug = model.Slug.ToLower().Replace(" ", "-"),
                SortOrder = model.SortOrder
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Traductions
            if (!string.IsNullOrEmpty(model.NameFr))
                _context.CategoryTranslations.Add(new CategoryTranslation { CategoryId = category.CategoryId, LanguageId = 1, Name = model.NameFr });

            if (!string.IsNullOrEmpty(model.NameEn))
                _context.CategoryTranslations.Add(new CategoryTranslation { CategoryId = category.CategoryId, LanguageId = 2, Name = model.NameEn });

            if (!string.IsNullOrEmpty(model.NameAr))
                _context.CategoryTranslations.Add(new CategoryTranslation { CategoryId = category.CategoryId, LanguageId = 3, Name = model.NameAr });

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");




        }
    }
}

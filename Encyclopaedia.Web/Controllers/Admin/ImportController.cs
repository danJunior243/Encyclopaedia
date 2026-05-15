using CsvHelper;
using CsvHelper.Configuration;
using Encyclopaedia.Core.Entities;
using Encyclopaedia.Data;
using Encyclopaedia.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;
using System.Globalization;

namespace Encyclopaedia.Web.Controllers.Admin
{
    [Authorize]

    // Le controller d'importation de données depuis un fichier CSV. Actuellement limité à l'import de catégories, mais peut être étendu à d'autres types (articles, domaines) en fonction des besoins.
    // Cela va nous eviter de devoir ecrire des lignes à la main et nous gagnerons plus de temps
    public class ImportController : Controller
    {
        private readonly EncyclopaediaDbContext _context;

        public ImportController(EncyclopaediaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View("~/Views/Import/Index.cshtml", new ImportViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Import(ImportViewModel model)
        {
            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("", "Veuillez sélectionner un fichier.");
                return View("~/Views/Import/Index.cshtml", model);
            }

            using var reader = new StreamReader(model.File.OpenReadStream());
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            });

            if (model.Type == "categories")
            {
                var records = csv.GetRecords<CategoryCsvModel>().ToList();
                foreach (var record in records)
                {
                    try
                    {
                        var category = new Category
                        {
                            DomainId = record.DomainId,
                            ParentId = record.ParentId == 0 ? null : record.ParentId,
                            Slug = record.Slug.ToLower().Replace(" ", "-"),
                            SortOrder = record.SortOrder
                        };
                        // On ajoute la catégorie à la base de données pour générer son ID, nécessaire pour les traductions

                        _context.Categories.Add(category);
                        await _context.SaveChangesAsync();

                        if (!string.IsNullOrEmpty(record.NameFr))
                            _context.CategoryTranslations.Add(new CategoryTranslation { CategoryId = category.CategoryId, LanguageId = 1, Name = record.NameFr });
                        if (!string.IsNullOrEmpty(record.NameEn))
                            _context.CategoryTranslations.Add(new CategoryTranslation { CategoryId = category.CategoryId, LanguageId = 2, Name = record.NameEn });
                        if (!string.IsNullOrEmpty(record.NameAr))
                            _context.CategoryTranslations.Add(new CategoryTranslation { CategoryId = category.CategoryId, LanguageId = 3, Name = record.NameAr });

                        await _context.SaveChangesAsync();
                        model.ImportedCount++;
                    }
                    catch (Exception ex)
                    {
                        model.Errors.Add($"Erreur sur {record.Slug} : {ex.Message}");
                    }
                }
            }

            model.Success = model.ImportedCount > 0;
            return View("~/Views/Import/Index.cshtml", model);
        }
    }

    // ── Modèles CSV ──
    public class CategoryCsvModel
    {
        public string Slug { get; set; } = string.Empty;
        public int DomainId { get; set; }
        public int ParentId { get; set; }
        public int SortOrder { get; set; }
        public string NameFr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
    }
}
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Encyclopaedia.Web.Models
{
    public class CategoryViewModel
    {

        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Le domaine est obligatoire")]
        public int DomainId { get; set; }

        public int? ParentId { get; set; }

        [Required(ErrorMessage = "Le slug est obligatoire")]
        public string Slug { get; set; } = string.Empty;

        public int SortOrder { get; set; } = 0;

        // Traductions
        [Required(ErrorMessage = "Le nom en français est obligatoire")]
        public string NameFr { get; set; } = string.Empty;

        public string NameEn { get; set; } = string.Empty;

        public string NameAr { get; set; } = string.Empty;

        // Listes pour les dropdowns
        public List<SelectListItem> Domains { get; set; } = new();
        public List<SelectListItem> ParentCategories { get; set; } = new();
    }
}

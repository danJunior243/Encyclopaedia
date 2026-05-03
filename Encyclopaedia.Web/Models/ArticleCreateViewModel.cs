using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Encyclopaedia.Web.Models
{
    public class ArticleCreateViewModel
    {
        [Required(ErrorMessage = "Le titre est obligatoire")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le résumé est obligatoire")]
        public string Summary { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le contenu est obligatoire")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "La catégorie est obligatoire")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "La langue est obligatoire")]
        public int LanguageId { get; set; }


        // Listes pour les dropdowns
        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> Languages { get; set; } = new();

    }
}

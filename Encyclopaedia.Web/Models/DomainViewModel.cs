using Encyclopaedia.Core.Enums;
using System.ComponentModel.DataAnnotations;
namespace Encyclopaedia.Web.Models
{
    public class DomainViewModel
    {
        public int DomainId { get; set; }
        [Required(ErrorMessage = "Le slug est obligatoire")]
        public string Slug { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
        // Traductions
        [Required(ErrorMessage = "Le nom en français est obligatoire")]
        public string NameFr { get; set; } = string.Empty;

        public string NameEn { get; set; } = string.Empty;

        public string NameAr { get; set; } = string.Empty;
    }
}

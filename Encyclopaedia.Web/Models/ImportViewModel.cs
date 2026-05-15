using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Encyclopaedia.Web.Models
{
    public class ImportViewModel
    {
        [Required(ErrorMessage = "Veuillez sélectionner un fichier")]
        public IFormFile? File { get; set; }

        public string Type { get; set; } = string.Empty; // "categories" ou "domains"

        public int ImportedCount { get; set; } = 0;
        public List<string> Errors { get; set; } = new();
        public bool Success { get; set; } = false;
    }
}
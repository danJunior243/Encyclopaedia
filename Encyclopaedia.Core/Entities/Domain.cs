using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encyclopaedia.Core.Entities
{
    public class Domain
    {
        public int DomainId { get; set; }
        // On utilise slug au lieu de Name pour les domaines, car le slug est une version URL-friendly du nom du domaine,
        // ce qui facilite la création d'URL propres et lisibles pour les pages associées à chaque domaine.
        public string Slug { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
        public ICollection<DomainTranslation> Translations { get; set; } = new List<DomainTranslation>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encyclopaedia.Core.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        public int DomainId { get; set; }
        public int? ParentId { get; set; }
        public string Slug { get; set; } = string.Empty;
        public Domain Domain { get; set; }=null!;
        public int SortOrder { get; set; } = 0;

        public ICollection<CategoryTranslation> Translations { get; set; } = new List<CategoryTranslation>();
        public ICollection<Article> Articles { get; set; } = new List<Article>();

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encyclopaedia.Core.Entities
{
    public class CategoryTranslation
    {
       
        public int CategoryId { get; set; }
        public int LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encyclopaedia.Core.Entities
{
    public class TagTranslation
    {
        public int TagTranslationId { get; set; }
        public int TagId { get; set; }
        public int LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
       
    }
}

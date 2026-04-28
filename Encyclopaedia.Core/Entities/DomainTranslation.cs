using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encyclopaedia.Core.Entities
{
    public class DomainTranslation
    {
        public int DomainTranslationId { get; set; }
        public int DomainId { get; set; }
        public int LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

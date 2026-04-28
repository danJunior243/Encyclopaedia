using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encyclopaedia.Core.Entities
{
    public class Language
    {
        public int LanguageId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}

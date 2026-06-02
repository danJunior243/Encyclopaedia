using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encyclopaedia.Core.Entities
{
   public class ArticleTranslation
    {

        public int ArticleTranslationId { get; set; }
        public int ArticleId { get; set; }
        public int LanguageId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public Article Article { get; set; } = null!;
        public Language Language { get; set; } = null!;
    }
}

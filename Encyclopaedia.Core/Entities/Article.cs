using Encyclopaedia.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encyclopaedia.Core.Entities
{
   public  class Article
   {
        public int ArticleId { get; set; }
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }
        public ArticleStatus Statut { get; set; } = ArticleStatus.Draft;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime?PublishAt { get; set; }
        public Category Category { get; set; } = null!;
        public ICollection<ArticleTranslation> Translations { get; set; } = new List<ArticleTranslation>();

        public long ViewCount { get; set; } = 0;


    }
}

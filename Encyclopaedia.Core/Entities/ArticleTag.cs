using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encyclopaedia.Core.Entities
{
    public class ArticleTag
    {
        // Cette classe représente la relation entre un article et une étiquette (tag).
       
        public int ArticleId { get; set; }
        public int TagId { get; set; }
    }
}

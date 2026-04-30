using Encyclopaedia.Core.Entities;
using Encyclopaedia.Core.Enums;

namespace Encyclopaedia.Web.Models
{
    public class DashboardViewModel
    {
       public long TotalArticles { get; set; }
        public Dictionary<ArticleStatus, int> ArticlesByStatus { get; set; } = new();
        public Dictionary<string, int> ArticlesByDomain { get; set; } = new();

        public List<Article> RecentlyUpdated { get; set; } = new();
    }
}

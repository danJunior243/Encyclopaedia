using Encyclopaedia.Core.Entities;

namespace Encyclopaedia.Web.Models
{
    public class HomeViewModel
    {
        public List<Article> RecentArticles { get; set; } = new List<Article>();
        public List<Article> PopularArticles { get; set; } = new List<Article>();
    }
}

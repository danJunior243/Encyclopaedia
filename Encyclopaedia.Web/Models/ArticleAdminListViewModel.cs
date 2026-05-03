using Encyclopaedia.Core.Entities;
using Encyclopaedia.Core.Enums;

namespace Encyclopaedia.Web.Models
{
    public class ArticleAdminListViewModel
    {
        public List<ArticleListViewModel> Articles { get; set; } = new List<ArticleListViewModel>();
        public long TotalArticles { get; set; }=0;
        public string? FiltreStatut { get; set; } = string.Empty;


    }
}

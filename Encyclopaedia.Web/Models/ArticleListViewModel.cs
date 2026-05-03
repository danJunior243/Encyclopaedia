using Encyclopaedia.Core.Entities;
using Encyclopaedia.Core.Enums;

namespace Encyclopaedia.Web.Models
{
    public class ArticleListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string Category { get; set; } = null!;
        public ArticleStatus Status { get; set; }=ArticleStatus.Draft;
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
        public long ViewCount { get; set; } = 0;
    }
}

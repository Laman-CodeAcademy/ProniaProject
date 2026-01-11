using Pronia.Models.Common;

namespace Pronia.Models
{
    public class BasketItem : BaseEntity
    {
         public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public int Count { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}

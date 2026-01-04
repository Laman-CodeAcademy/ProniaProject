using Pronia.Models.Common;

namespace Pronia.Models
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<ProductTag> ProductTags { get; set; } = [];
    }
}


public class ProductTag : BaseEntity
{
    public Tag Tag { get; set; }
    public int TagId { get; set; }
    public Product Product { get; set; }
    public int ProductId { get; set; }
}
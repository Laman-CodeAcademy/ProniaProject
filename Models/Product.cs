using Pronia.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Product : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        [Required]
        public string MainImageUrl { get; set; }
        [Required]
        public string? HoverImageUrl { get; set; }
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        public ICollection<ProductTag> ProductTags { get; set; } = [];
        public ICollection<ProductImage> ProductImages { get; set; } = [];


    }
}

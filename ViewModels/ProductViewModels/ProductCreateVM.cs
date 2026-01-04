using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels.ProductViewModels
{
    public class ProductCreateVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<int> TagIds{ get; set; } 
        [Required]
        public IFormFile MainImage { get; set; }
        [Required]
        public IFormFile HoverImage { get; set; }
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        public List<IFormFile> Images { get; set; } = [];
    }
}

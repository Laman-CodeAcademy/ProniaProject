using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels.ProductViewModels
{
    public class ProductUpdateVM
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
        public List<int>? TagIds { get; set; }

        public IFormFile? MainImage { get; set; }
        public IFormFile? HoverImage { get; set; }
        public string? MainImageUrl { get; set; }
        public string? HoverImageUrl { get; set; }
        public List<IFormFile>? Images { get; set; } = [];
        public List<string>? ImageUrls { get; set; } = [];
        public List<int>? ImageIds { get; set; } = [];
        public int Rating { get; set; }
    }
}

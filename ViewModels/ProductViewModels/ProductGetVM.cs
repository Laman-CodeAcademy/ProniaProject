namespace Pronia.ViewModels.ProductViewModels
{
    public class ProductGetVM
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public string SKU { get; set; }

        public string CategoryName { get; set; }

        public string? MainImageUrl { get; set; }

        public string? HoverImageUrl { get; set; }

        public int Rating { get; set; }

        public List<string> TagNames { get; set; }
        public List<string> ImageUrls { get; set; }


    }
}
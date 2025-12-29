using Microsoft.AspNetCore.Mvc;
using Pronia.Contexts;
using Pronia.ViewModels.ProductViewModels;

namespace Pronia.Controllers
{
    public class ShopController : Controller
    {
        AppDbContext context;
        public ShopController(AppDbContext _context)
        {
            context = _context;
        }
        public IActionResult Index()
        {
            var products = context.Products
                .Select(p => new ProductGetVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    SKU = p.SKU,
                    CategoryName = p.Category.Name,
                    Rating = p.Rating,
                    MainImageUrl = p.MainImageUrl,
                    HoverImageUrl = p.HoverImageUrl
                })
                .ToList();
            return View(products);
        }
    }
}
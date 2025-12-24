using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        private void SendCategoriesWithViewBag()
        {
            ViewBag.Categories = _context.Categories.ToList();
        }

        public IActionResult Index()
        {
            var products = _context.Products
                .Include(p => p.Category)
                .ToList();

            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            SendCategoriesWithViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                SendCategoriesWithViewBag();
                return View(product);
            }

            bool existsCategory = _context.Categories
                .Any(c => c.Id == product.CategoryId);

            if (!existsCategory)
            {
                ModelState.AddModelError("CategoryId", "Category is not valid");
                SendCategoriesWithViewBag();
                return View(product);
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            SendCategoriesWithViewBag();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Product product)
        {
            if (!ModelState.IsValid)
            {
                SendCategoriesWithViewBag();
                return View(product);
            }

            bool existsCategory = _context.Categories
                .Any(c => c.Id == product.CategoryId);

            if (!existsCategory)
            {
                ModelState.AddModelError("CategoryId", "Category is not valid");
                SendCategoriesWithViewBag();
                return View(product);
            }

            var dbProduct = _context.Products.Find(product.Id);
            if (dbProduct == null)
                return NotFound();

            dbProduct.Name = product.Name;
            dbProduct.Description = product.Description;
            dbProduct.Price = product.Price;
            dbProduct.SKU = product.SKU;
            dbProduct.CategoryId = product.CategoryId;
            dbProduct.MainImageUrl = product.MainImageUrl;
            dbProduct.HoverImageUrl = product.HoverImageUrl;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }






    }
}
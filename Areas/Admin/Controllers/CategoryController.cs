using Microsoft.AspNetCore.Mvc;
using Pronia.Contexts;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers;
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            bool existsCategory = _context.Categories
                .Any(c => c.Name.ToLower() == category.Name.ToLower());

            if (existsCategory)
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View(category);
            }

            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            bool existsCategory = _context.Categories
                .Any(c => c.Name.ToLower() == category.Name.ToLower()
                       && c.Id != category.Id);

            if (existsCategory)
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View(category);
            }

            var dbCategory = _context.Categories.Find(category.Id);
            if (dbCategory == null)
                return NotFound();

            dbCategory.Name = category.Name;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
}
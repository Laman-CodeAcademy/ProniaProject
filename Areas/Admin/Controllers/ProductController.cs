using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using Pronia.Contexts;
using Pronia.Helpers;
using Pronia.Models;
using Pronia.ViewModels.ProductViewModels;

namespace Pronia.Areas.Admin.Controllers;
    [Area("Admin")]
    public class ProductController(AppDbContext _context, IWebHostEnvironment _environment) : Controller
    {

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
        public IActionResult Create(ProductCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                SendCategoriesWithViewBag();
                return View(vm);
            }

            //MainImage
            if (!vm.MainImage.CheckType("image"))
            {
                ModelState.AddModelError("MainImage", "u can only upload image file");
                return View(vm);
            }
            if (!vm.MainImage.CheckSize(2))
            {
                ModelState.AddModelError("MainImage", "u can only upload images less than 2mb");
                return View(vm);
            }

            //HoverImage
            if (!vm.HoverImage.CheckType("image"))
            {
                ModelState.AddModelError("HoverImage", "u can only upload image file");
                return View(vm);
            }
            if (!vm.HoverImage.CheckSize(2))
            {
                ModelState.AddModelError("HoverImage", "u can only upload images less than 2mb");
                return View(vm);
            }

        bool existsCategory = _context.Categories
                .Any(c => c.Id == vm.CategoryId);

            if (!existsCategory)
            {
                ModelState.AddModelError("CategoryId", "Category is not valid");
                SendCategoriesWithViewBag();
                return View(vm);
            }

        string folderPath = Path.Combine(_environment.WebRootPath, "assets","images","website-images");

        string mainImageUniqueName = vm.MainImage.SaveFile(folderPath);
        string hoverImageUniqueName = vm.HoverImage.SaveFile(folderPath);


        Product product = new Product()
        {
            Name = vm.Name,
            Description = vm.Description,
            SKU = vm.SKU,
            CategoryId = vm.CategoryId,
            Price = vm.Price,
            MainImageUrl = mainImageUniqueName,
            HoverImageUrl = hoverImageUniqueName,


        };

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

        ProductUpdateVM vm = new ProductUpdateVM()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            SKU = product.SKU,
            CategoryId = product.CategoryId,
            Price = product.Price,

        };

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ProductUpdateVM vm)
        {
            if (!ModelState.IsValid)
            {
                SendCategoriesWithViewBag();
                return View(vm);
            }

        var dbProduct = _context.Products.Find(vm.Id);
        if (dbProduct == null)
            return NotFound();

        bool existsCategory = _context.Categories
                .Any(c => c.Id == vm.CategoryId);

            if (!existsCategory)
            {
                ModelState.AddModelError("CategoryId", "Category is not valid");
                SendCategoriesWithViewBag();
                return View(vm);
            }

        //MainImage
        if (!vm.MainImage?.CheckType("image")?? false)
        {
            ModelState.AddModelError("MainImage", "u can only upload image file");
            return View(vm);
        }
        if (!vm.MainImage?.CheckSize(2) ?? false)
        {
            ModelState.AddModelError("MainImage", "u can only upload images less than 2mb");
            return View(vm);
        }

        //HoverImage
        if (!vm.HoverImage?.CheckType("image") ?? false)
        {
            ModelState.AddModelError("HoverImage", "u can only upload image file");
            return View(vm);
        }
        if (!vm.HoverImage?.CheckSize(2) ?? false)
        {
            ModelState.AddModelError("HoverImage", "u can only upload images less than 2mb");
            return View(vm); 
        }


        dbProduct.Name = vm.Name;
            dbProduct.Description = vm.Description;
            dbProduct.Price = vm.Price;
            dbProduct.SKU = vm.SKU;
            dbProduct.CategoryId = vm.CategoryId;

        string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

        if (vm.MainImage is { })
        {
            string newMainImage = vm.MainImage.SaveFile(folderPath);
            ExtensionMethods.DeleteFile(folderPath, dbProduct.MainImageUrl);
            dbProduct.MainImageUrl = newMainImage;
        }
        if (vm.HoverImage is { })
        {
            string newHoverImage = vm.HoverImage.SaveFile(folderPath);
            ExtensionMethods.DeleteFile(folderPath, dbProduct.HoverImageUrl);
            dbProduct.HoverImageUrl = newHoverImage;
        }

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

        string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

        ExtensionMethods.DeleteFile(folderPath, product.MainImageUrl);
        ExtensionMethods.DeleteFile(folderPath, product.HoverImageUrl);


        return RedirectToAction(nameof(Index));
        }






}
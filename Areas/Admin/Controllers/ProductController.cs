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

        private void SendItemsWithViewBag()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Tags = _context.Tags.ToList();
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
            SendItemsWithViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                SendItemsWithViewBag();
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

            foreach(var image in vm.Images)
        {
            if (!image.CheckType("image"))
            {
                ModelState.AddModelError("Image", "u can only upload image file");
                return View(vm);
            }
            if (!vm.MainImage.CheckSize(2))
            {
                ModelState.AddModelError("Image", "u can only upload images less than 2mb");
                return View(vm);
            }
        }

        bool existsCategory = _context.Categories
                .Any(c => c.Id == vm.CategoryId);

            if (!existsCategory)
            {
                ModelState.AddModelError("CategoryId", "Category is not valid");
                SendItemsWithViewBag();
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
            Rating = vm.Rating,
            MainImageUrl = mainImageUniqueName,
            HoverImageUrl = hoverImageUniqueName,
        };

        foreach(var image in vm.Images)
        {
            string imageUniqueName = image.SaveFile(folderPath);
            ProductImage productImage = new ProductImage()
            {
                ImageUrl = imageUniqueName,
                Product = product,
            };
            product.ProductImages.Add(productImage);
        }

        foreach (var tagId in vm.TagIds)
        {
            var IsExistTag = _context.Tags.Any(x => x.Id == tagId);

            if(IsExistTag is false)
            {
                SendItemsWithViewBag();
                ModelState.AddModelError("", "tag not found");
                return View(vm);
            }

            ProductTag productTag = new ProductTag()
            {
                TagId = tagId,
                Product = product,
            };
            product.ProductTags.Add(productTag); 
        }

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var product = _context.Products.Include(x=>x.ProductTags).Include(x => x.ProductImages).FirstOrDefault(x=>x.Id==id);
            if (product == null)
                return NotFound();

            SendItemsWithViewBag();

        ProductUpdateVM vm = new ProductUpdateVM()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            SKU = product.SKU,
            CategoryId = product.CategoryId,
            Price = product.Price,
            Rating = product.Rating,
            TagIds = product.ProductTags.Select(x => x.TagId).ToList(),
            ImageUrls = product.ProductImages.Select(x => x.ImageUrl).ToList(),
            ImageIds = product.ProductImages.Select(x => x.Id).ToList(),
        };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ProductUpdateVM vm)
        {
            if (!ModelState.IsValid)
            {
                SendItemsWithViewBag();
                return View(vm);
            }

        var dbProduct = _context.Products.Include(x=>x.ProductTags).Include(x => x.ProductImages).FirstOrDefault(x=>x.Id==vm.Id);
        if (dbProduct == null)
            return NotFound();

        bool existsCategory = _context.Categories
                .Any(c => c.Id == vm.CategoryId);

            if (!existsCategory)
            {
                ModelState.AddModelError("CategoryId", "Category is not valid");
                SendItemsWithViewBag();
                return View(vm);
            }

        foreach (var tagId in vm.TagIds)
        {
            var isExistTag = _context.Tags.Any(x => x.Id == tagId);

            if (isExistTag is false)
            {
                SendItemsWithViewBag();
                ModelState.AddModelError("", "tag not found");
                return View(vm);
            }
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
            dbProduct.Rating = vm.Rating;

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

        dbProduct.ProductTags = [];

        foreach(var tagId in vm.TagIds)
        {
            ProductTag productTag = new ProductTag()
            {
                TagId = tagId,
                ProductId=dbProduct.Id,
            };
            dbProduct.ProductTags.Add(productTag);
        }

        foreach (var productImage in dbProduct.ProductImages.ToList())
        {
            var isExistImage = vm.ImageIds.Any(x => x == productImage.Id);

            if (isExistImage is false)
            {
               dbProduct.ProductImages.Remove(productImage);
                ExtensionMethods.DeleteFile(folderPath, productImage.ImageUrl);
            }
        }

        _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var product = _context.Products.Include(x=>x.ProductImages).FirstOrDefault(x=>x.Id==id);
            if (product == null)
                return NotFound();
            _context.Products.Remove(product);
            _context.SaveChanges();

        string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

        ExtensionMethods.DeleteFile(folderPath, product.MainImageUrl);
        ExtensionMethods.DeleteFile(folderPath, product.HoverImageUrl);

        foreach(var image in product.ProductImages)
        {
            ExtensionMethods.DeleteFile(folderPath, image.ImageUrl);
        }

        return RedirectToAction(nameof(Index));
        }


        public IActionResult Detail(int id)
    {
        var product = _context.Products.Select(x => new ProductGetVM()
        {
            Id = x.Id,
            Name = x.Name,
            CategoryName=x.Category.Name,
            Description = x.Description,
            SKU = x.SKU,
            Price = x.Price,
            Rating = x.Rating,
            MainImageUrl = x.MainImageUrl,
            HoverImageUrl = x.HoverImageUrl,
            TagNames=x.ProductTags.Select(x =>x.Tag.Name).ToList(),
            ImageUrls=x.ProductImages.Select(x => x.ImageUrl).ToList(),
        }).FirstOrDefault(x=>x.Id == id);
        if (product == null)
            return NotFound();
       
        return View(product);
       
    }



}
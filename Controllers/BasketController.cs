using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pronia.Abstraction;
using Pronia.Context;
using Pronia.Views.Account;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pronia.Controllers;
[Authorize]

public class BasketController(AppDbContext _context, IBasketService _basketService) : Controller
{
    public async Task<IActionResult> IndexAsync()
    {
        var basketItems = await _basketService.GetBasketsAsync();
        return View(basketItems);
    }
    public async Task<IActionResult> AddToBasket(int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(p => p.Id == productId);
        if (!isExistProduct)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var IsExistUser = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!IsExistUser)
        {
            return BadRequest();
        }

        var isExistInBasket = await _context.Baskets
            .FirstOrDefaultAsync(b => b.ProductId == productId && b.AppUserId == userId);

        if (isExistInBasket != null)
        {
            isExistInBasket.Count++;
            await _context.SaveChangesAsync();
        }
        else
        {
            Basket basket = new()
            {
                ProductId = productId,
                Count = 1,
                AppUserId = userId!
            };

            await _context.Baskets.AddAsync(basket);
        }


        await _context.SaveChangesAsync();

        TempData["Success"] = "Product added to basket successfully.";
        return RedirectToAction("Index", "Shop");
    }

    [Authorize]
    public async Task<IActionResult> RemoveFromBasket(int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(p => p.Id == productId);
        if (!isExistProduct)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var basketItem = await _context.Baskets
            .FirstOrDefaultAsync(b => b.ProductId == productId && b.AppUserId == userId);

        if (basketItem == null)
        {
            return NotFound();
        }

        _context.Baskets.Remove(basketItem);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Product removed from basket successfully.";

        var returnUrl = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Shop");
    }


    [HttpPost]
    public async Task<IActionResult> DecreaseBasketCount(int productId)
    {
       var isExistProduct =  _context.Products.Any(p => p.Id == productId);
        if (!isExistProduct)
        {
            return NotFound();
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
        var basketItem =  _context.Baskets
            .FirstOrDefault(b => b.ProductId == productId && b.AppUserId == userId);
        if (basketItem == null)
        {
            return NotFound();
        }
        if (basketItem.Count > 1)
        {
            basketItem.Count--;
        }
        else
        {
            _context.Baskets.Remove(basketItem);
        }

        _context.SaveChanges();
         var basketItems= await _basketService.GetBasketsAsync();
        return PartialView("_BasketPartialView", basketItems);
    }
    [HttpPost]
    public async Task<IActionResult> IncreaseBasketCount(int productId)
    {
        var isExistProduct = await _context.Products.AnyAsync(p => p.Id == productId);
        if (!isExistProduct)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var basketItem = await _context.Baskets
            .FirstOrDefaultAsync(b => b.ProductId == productId && b.AppUserId == userId);

        if (basketItem != null)
        {
            basketItem.Count++;
        }
        else
        {
            Basket basket = new()
            {
                ProductId = productId,
                Count = 1,
                AppUserId = userId
            };

            await _context.Baskets.AddAsync(basket);
        }

        await _context.SaveChangesAsync();

        var basketItems = await _basketService.GetBasketsAsync();
        return PartialView("_BasketPartialView", basketItems);
    }

}
using Pronia.Abstraction;
using Pronia.Context;
using Pronia.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pronia.Services
{
    public class BasketService(IHttpContextAccessor _accessor,AppDbContext _context) : IBasketService
    {
        public async Task<List<Basket>> GetBasketsAsync()
        {
            var usedId= _accessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isExistUser = await _context.Users.AnyAsync(u => u.Id == usedId);
            if (!isExistUser)
            {
                return [];
            }

            var basketItems = await _context.Baskets
                .Include(b => b.Product)
                .Where(b => b.AppUserId == usedId)
                .ToListAsync();

            return basketItems;
        }
    }
}
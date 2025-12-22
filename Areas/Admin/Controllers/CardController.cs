using Microsoft.AspNetCore.Mvc;
using Pronia.Contexts;

namespace Pronia.Areas.Admin.Controllers;
    [Area("Admin")]
    public class CardController(AppDbContext _context) : Controller
    {

        public IActionResult Index()
        {
            var cards = _context.Cards.ToList();
            return View(cards);
        }

    [HttpGet]
    public IActionResult Create()
    { 
        return View();
    }

    [HttpPost]
        public IActionResult Create(Card card)
        {
        _context.Cards.Add(card);
        _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }

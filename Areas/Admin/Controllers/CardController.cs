using Microsoft.AspNetCore.Mvc;
using Pronia.Contexts;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers;
[Area("Admin")]
//[ValidateAntiForgeryToken]
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

    [HttpPost("card/{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var card = _context.Cards.FirstOrDefault(c=>c.Id== id);
        if (card == null) return NotFound("card not found"); 
        _context.Remove(card);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        var card = _context.Cards.Find(id);
        if (card == null) return NotFound();
        return View(card);
    }

    [HttpPost]
    public IActionResult Update(Card card)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var existingCard = _context.Cards.Find(card.Id);
        if (existingCard == null)
            return NotFound();

        existingCard.Title = card.Title;
        existingCard.Description = card.Description;
        existingCard.Image = card.Image;


        _context.Cards.Update(existingCard);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

}

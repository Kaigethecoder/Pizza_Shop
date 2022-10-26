using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pizza_Shop.Data;
using Pizza_Shop.Models;

namespace Pizza_Shop.Controllers
{
    public class ToppingsController : Controller
    {
        private readonly Pizza_ShopContext _context;

        public ToppingsController(Pizza_ShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
              return View(await _context.Topping.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Topping == null)
            {
                return NotFound();
            }

            var topping = await _context.Topping
                .FirstOrDefaultAsync(m => m.ToppingID == id);
            if (topping == null)
            {
                return NotFound();
            }

            return View(topping);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ToppingName")] Topping topping)
        {
            if (ValidateDuplicate(topping.ToppingName))
            {
                ModelState.AddModelError("", "We have a topping like that already!");
            }

            if (ModelState.IsValid)
            {
                _context.Add(topping);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(topping);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Topping == null)
            {
                return NotFound();
            }

            var topping = await _context.Topping.FindAsync(id);
            if (topping == null)
            {
                return NotFound();
            }
            return View(topping);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ToppingID,ToppingName")] Topping topping)
        {
            if (id != topping.ToppingID)
            {
                return NotFound();
            }

            var lastName = await _context.Topping.FindAsync(id);
            if (!ValidateEdit(topping.ToppingName, lastName.ToppingName))
            {
                ModelState.AddModelError("", "In order to prevent confusion and ruining pizza maserterpieces, make sure the name is similar!");
            }

            var updatedTopping = await _context.Topping.FindAsync(id);
            updatedTopping.ToppingName = topping.ToppingName;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(updatedTopping);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToppingExists(topping.ToppingID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(topping);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Topping == null)
            {
                return NotFound();
            }

            var topping = await _context.Topping
                .FirstOrDefaultAsync(m => m.ToppingID == id);
            if (topping == null)
            {
                return NotFound();
            }

            return View(topping);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Topping == null)
            {
                return Problem("Entity set 'Pizza_ShopContext.Topping'  is null.");
            }
            var topping = await _context.Topping.FindAsync(id);
            if (topping != null)
            {
                _context.Topping.Remove(topping);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ValidateEdit(string newName, string oldName)
        {
            if (newName.Contains(oldName, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            else if (oldName.Contains(newName, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ValidateDuplicate(string toppingName)
        {
            foreach (var currTopping in _context.Topping.ToList())
            {
                if (currTopping.ToppingName.Equals(toppingName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
                else if (currTopping.ToppingName.Contains(toppingName, StringComparison.CurrentCultureIgnoreCase))
                {

                    return true;
                }
                else if (toppingName.Contains(currTopping.ToppingName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ToppingExists(int id)
        {
          return _context.Topping.Any(e => e.ToppingID == id);
        }
    }
}

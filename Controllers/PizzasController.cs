using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizza_Shop.Data;
using Pizza_Shop.Models;

namespace Pizza_Shop.Controllers
{
    public class PizzasController : Controller
    {
        private readonly Pizza_ShopContext _context;

        public PizzasController(Pizza_ShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Pizza.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pizza == null)
            {
                return NotFound();
            }

            var pizza = await _context.Pizza
                .Include(s=> s.PizzaToppings)
                .ThenInclude(e=> e.Topping)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.PizzaID == id);
            if (pizza == null)
            {
                return NotFound();
            }

            return View(pizza);
        }

        public async Task<IActionResult> Create()
        {
            var pizzaForm = new PizzaForm();
            pizzaForm.AvailableToppings = await _context.Topping.ToListAsync();
            return View(pizzaForm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PizzaForm pizza)
        {
            pizza.AvailableToppings = await _context.Topping.ToListAsync();
            if (pizza.ToppingsSelected == null)
            {
                ModelState.AddModelError("", "You must select at least ONE topping.");
            }

            bool duplicate = true;
            if (pizza.ToppingsSelected != null)
            {
                duplicate = DuplicateToppings(pizza.ToppingsSelected) || DuplicateName(pizza.PizzaName);
                if (duplicate)
                {
                    ModelState.AddModelError("", "Looks like we already have a pizza like that.");
                }
            }
           
            if (!duplicate)
            {
                var newPizza = new Pizza();
                newPizza.PizzaName = pizza.PizzaName;
                _context.Add(newPizza);
                await _context.SaveChangesAsync();
                foreach (var topping in pizza.ToppingsSelected)
                {
                    var newPizzaTopping = new PizzaTopping();
                    newPizzaTopping.PizzaID = newPizza.PizzaID;
                    foreach (var currTopping in await _context.Topping.ToListAsync())
                    {
                        if (topping == currTopping.ToppingID)
                        {
                            newPizzaTopping.Topping = currTopping;
                            _context.Add(newPizzaTopping);
                            await _context.SaveChangesAsync();
                            break;
                        }
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pizza);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pizza == null)
            {
                return NotFound();
            }

            var pizza = await _context.Pizza.FindAsync(id);
            if (pizza == null)
            {
                return NotFound();
            }

            var pizzaForm = new PizzaForm();
            pizzaForm.AvailableToppings = await _context.Topping.ToListAsync();
            pizzaForm.PizzaName = pizza.PizzaName;
            pizzaForm.ID = (int)id;
            return View(pizzaForm);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostEdit(PizzaForm pizza)
        {
            pizza.AvailableToppings = await _context.Topping.ToListAsync();
            if (pizza.ToppingsSelected == null)
            {
                ModelState.AddModelError("", "You must select at least ONE topping.");
                return RedirectToAction(nameof(Edit));
            }

            bool duplicateName = true;
            bool duplicateToppings = true;
            if (pizza.ToppingsSelected != null)
            {
                duplicateName = DuplicateName(pizza.PizzaName);
                
                if (!duplicateName)
                {
                    ModelState.AddModelError("", "Try to keep the name similar to avoid confusion with other pizzas.");
                }
            }

            var updatedPizza = await _context.Pizza.FindAsync(pizza.ID);
            if (duplicateName)
            {
                updatedPizza.PizzaName = pizza.PizzaName;
                foreach (var pt in await _context.PizzaTopping.ToListAsync())
                {
                    if (pt.PizzaID == updatedPizza.PizzaID)
                    {
                        _context.Remove(pt);
                        await _context.SaveChangesAsync();
                    }
                }

                duplicateToppings = DuplicateToppings(pizza.ToppingsSelected);
                if (duplicateToppings)
                {
                    _context.Remove(updatedPizza);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Create));
                }

                foreach (var topping in pizza.ToppingsSelected)
                {
                    var newPizzaTopping = new PizzaTopping();
                    newPizzaTopping.PizzaID = updatedPizza.PizzaID;
                    foreach (var currTopping in await _context.Topping.ToListAsync())
                    {
                        if (topping == currTopping.ToppingID)
                        {
                            newPizzaTopping.Topping = currTopping;
                            updatedPizza.PizzaToppings.Add(newPizzaTopping);
                            _context.Add(newPizzaTopping);
                            await _context.SaveChangesAsync();
                            break;
                        }
                    }
                }
                _context.Update(updatedPizza);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pizza);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pizza == null)
            {
                return NotFound();
            }

            var pizza = await _context.Pizza
                .FirstOrDefaultAsync(m => m.PizzaID == id);
            if (pizza == null)
            {
                return NotFound();
            }

            return View(pizza);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pizza == null)
            {
                return Problem("Entity set 'Pizza_ShopContext.Pizza'  is null.");
            }
            var pizza = await _context.Pizza.FindAsync(id);
            if (pizza != null)
            {
                _context.Pizza.Remove(pizza);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool DuplicateToppings(List<int> toppingsSelected)
        {
            bool duplicate = false;
            foreach (var p in _context.Pizza.ToList())
            {
                var pToppings = new List<int>();
                foreach (var pt in _context.PizzaTopping.ToList())
                {
                    if (p.PizzaID == pt.PizzaID)
                    {
                        pToppings.Add(pt.ToppingID);
                    }
                }
                duplicate = Enumerable.SequenceEqual(toppingsSelected.OrderBy(i => i), pToppings.OrderBy(j => j));
                if (duplicate == true)
                    break;
            }
            return duplicate;
        }
        private bool DuplicateName(string name)
        {
            foreach (var currPizza in _context.Pizza.ToList())
            {
                if (currPizza.PizzaName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
                else if (currPizza.PizzaName.Contains(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
                else if (name.Contains(currPizza.PizzaName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        private bool PizzaExists(int id)
        {
            return _context.Pizza.Any(e => e.PizzaID == id);
        }
    }
}

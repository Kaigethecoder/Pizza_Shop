using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pizza_Shop.Models;

namespace Pizza_Shop.Data
{
    public class Pizza_ShopContext : DbContext
    {
        public Pizza_ShopContext (DbContextOptions<Pizza_ShopContext> options)
            : base(options)
        {
        }

        public DbSet<Pizza_Shop.Models.Topping> Topping { get; set; } = default!;

        public DbSet<Pizza_Shop.Models.Pizza> Pizza { get; set; }
        public DbSet<Pizza_Shop.Models.PizzaTopping> PizzaTopping { get; set; }
    }
}

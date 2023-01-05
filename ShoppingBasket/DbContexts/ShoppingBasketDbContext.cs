using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Entities;

namespace ShoppingBasket.DbContexts
{
    public class ShoppingBasketDbContext : DbContext
    {
        public ShoppingBasketDbContext(DbContextOptions<ShoppingBasketDbContext> options) : base(options)
        {
        }

        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketLine> BasketLines { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}

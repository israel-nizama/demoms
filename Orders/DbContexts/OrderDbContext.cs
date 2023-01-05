using Microsoft.EntityFrameworkCore;
using Orders.Entities;

namespace Orders.DbContexts
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using ShoppingBasket.DbContexts;
using ShoppingBasket.Entities;

namespace ShoppingBasket.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly ShoppingBasketDbContext shoppingBasketDbContext;

        public BasketRepository(ShoppingBasketDbContext shoppingBasketDbContext)
        {
            this.shoppingBasketDbContext = shoppingBasketDbContext;
        }

        public async Task<Basket> GetBasketById(Guid basketId)
        {
            return await shoppingBasketDbContext.Baskets.Include(sb => sb.BasketLines)
                .Where(b => b.BasketId == basketId).FirstOrDefaultAsync();
        }

        public void AddBasket(Basket basket)
        {
            shoppingBasketDbContext.Baskets.Add(basket);
        }

        public async Task<bool> SaveChanges()
        {
            return (await shoppingBasketDbContext.SaveChangesAsync() > 0);
        }
    }
}

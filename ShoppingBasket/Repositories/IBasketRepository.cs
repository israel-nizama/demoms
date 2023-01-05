using ShoppingBasket.Entities;

namespace ShoppingBasket.Repositories
{
    public interface IBasketRepository
    {
        Task<Basket> GetBasketById(Guid basketId);
        void AddBasket(Basket basket);

        Task<bool> SaveChanges();
    }
}

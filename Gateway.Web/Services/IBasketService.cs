using Gateway.Web.Models;

namespace Gateway.Web.Services
{
    public interface IBasketService
    {
        Task<BasketCheckoutMessage> Checkout(BasketCheckout basketCheckout);
    }
}

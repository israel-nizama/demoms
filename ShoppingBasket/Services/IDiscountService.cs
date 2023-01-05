using ShoppingBasket.Models;

namespace ShoppingBasket.Services
{
    public interface IDiscountService
    {
        Task<Coupon> GetCoupon(Guid couponId);
    }
}

using Discounts.Entities;

namespace Discounts.Repositories
{
    public interface ICouponRepository
    {
        Task<Coupon> GetCouponByCode(string couponCode);
        Task<Coupon> GetCouponById(Guid couponId);
    }
}

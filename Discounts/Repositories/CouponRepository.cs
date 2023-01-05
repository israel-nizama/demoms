using Discounts.DBContexts;
using Discounts.Entities;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly DiscountDbContext _discountDbContext;

        public CouponRepository(DiscountDbContext discountDbContext)
        {
            _discountDbContext = discountDbContext;
        }

        public async Task<Coupon> GetCouponByCode(string couponCode)
        {
            var coupon = await _discountDbContext.Coupons.Where(x => x.Code == couponCode).FirstOrDefaultAsync();

            return coupon;
        }

        public async Task<Coupon> GetCouponById(Guid couponId)
        {
            return await _discountDbContext.Coupons.Where(x => x.CouponId == couponId).FirstOrDefaultAsync();
        }
    }
}

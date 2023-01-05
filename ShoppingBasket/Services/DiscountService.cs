using ShoppingBasket.Extensions;
using ShoppingBasket.Models;

namespace ShoppingBasket.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly HttpClient client;

        public DiscountService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<Coupon> GetCoupon(Guid couponId)
        {
            var response = await client.GetAsync($"/api/discount/{couponId}");
            return await response.ReadContentAs<Coupon>();
        }
    }
}

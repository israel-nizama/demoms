using Gateway.Web.Extensions;
using Gateway.Web.Models;

namespace Gateway.Web.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly HttpClient client;

        public DiscountService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<Coupon> GetDiscountForCode(string code)
        {
            var response = await client.GetAsync($"/api/discount/code/{code}");
            return await response.ReadContentAs<Coupon>();
        }
    }
}

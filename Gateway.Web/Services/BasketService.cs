using Gateway.Web.Extensions;
using Gateway.Web.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gateway.Web.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient client;

        public BasketService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<BasketCheckoutMessage> Checkout(BasketCheckout basketCheckout)
        {
            var dataAsString = JsonSerializer.Serialize(basketCheckout);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync($"/api/baskets/checkout", content);
            return await response.ReadContentAs<BasketCheckoutMessage>();
        }
    }
}

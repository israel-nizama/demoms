using Gateway.Web.Models;
using Gateway.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Web.Controllers
{
    [ApiController]
    [Route("api/gateway/basket")]
    public class BasketController : Controller
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> GetCouponForCode([FromBody] BasketCheckout basketCheckout)
        {
            var basketCheckoutMessage = await _basketService.Checkout(basketCheckout);
            return Ok(basketCheckoutMessage);
        }
    }
}

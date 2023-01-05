using Gateway.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Gateway.Web.Controllers
{
    [ApiController]
    [Route("api/gateway/discount")]
    public class DiscountController : Controller
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetCouponForCode(string code)
        {
            var coupon = await _discountService.GetDiscountForCode(code);
            return Ok(coupon);
        }
    }
}

using AutoMapper;
using Discounts.Models;
using Discounts.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Discounts.Controllers
{
    [ApiController]
    [Route("api/discount")]
    public class DiscountController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;

        public DiscountController(ICouponRepository couponRepository, IMapper mapper)
        {
            _couponRepository = couponRepository;
            _mapper = mapper;
        }

        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetDiscountForCode(string code)
        {
            var coupon = await _couponRepository.GetCouponByCode(code);

            if (coupon == null)
                return NotFound();

            return Ok(_mapper.Map<CouponDto>(coupon));
        }

        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("{couponId}")]
        public async Task<IActionResult> GetDiscountForCode(Guid couponId)
        {

            var coupon = await _couponRepository.GetCouponById(couponId);

            if (coupon == null)
                return NotFound();

            return Ok(_mapper.Map<CouponDto>(coupon));
        }
    }
}

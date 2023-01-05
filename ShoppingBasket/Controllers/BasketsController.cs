using AutoMapper;
using MessagingBus;
using Microsoft.AspNetCore.Mvc;
using ShoppingBasket.Messages;
using ShoppingBasket.Models;
using ShoppingBasket.Repositories;
using ShoppingBasket.Services;
using System.Diagnostics;
using System.Net;

namespace ShoppingBasket.Controllers
{
    [Route("api/baskets")]
    [ApiController]
    public class BasketsController : Controller
    {
        private readonly IBasketRepository basketRepository;
        private readonly IMapper mapper;
        private readonly IMessageBus messageBus;
        private readonly ILogger<BasketsController> logger;
        private readonly IDiscountService discountService;

        public BasketsController(IBasketRepository basketRepository, IMapper mapper, IMessageBus messageBus, ILogger<BasketsController> logger, IDiscountService discountService)
        {
            this.basketRepository = basketRepository;
            this.mapper = mapper;
            this.messageBus = messageBus;
            this.logger = logger;
            this.discountService = discountService;
        }

        [HttpPost("checkout")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CheckoutBasketAsync([FromBody] BasketCheckout basketCheckout)
        {
            using var scope = logger.BeginScope("Checking out basket {BasketId}", basketCheckout.BasketId);

            try
            {
                //based on basket checkout, fetch the basket lines from repo
                var basket = await basketRepository.GetBasketById(basketCheckout.BasketId);

                if (basket == null)
                {
                    logger.LogWarning("Basket was not found");
                    return BadRequest();
                }

                logger.LogDebug("Loaded basket");

                BasketCheckoutMessage basketCheckoutMessage = mapper.Map<BasketCheckoutMessage>(basketCheckout);
                basketCheckoutMessage.BasketLines = new List<BasketLineMessage>();
                int total = 0;

                foreach (var b in basket.BasketLines)
                {
                    var basketLineMessage = new BasketLineMessage
                    {
                        BasketLineId = b.BasketLineId,
                        Price = b.Price,
                        TicketAmount = b.TicketAmount
                    };

                    total += b.Price * b.TicketAmount;

                    basketCheckoutMessage.BasketLines.Add(basketLineMessage);
                }

                //call microsevice discount to get coupon info
                Coupon coupon = null;

                if (basket.CouponId.HasValue)
                    coupon = await discountService.GetCoupon(basket.CouponId.Value);

                if (coupon != null)
                {
                    logger.LogDebug("Applying discount {DiscountAmount} from {CouponId}", coupon.Amount, basket.CouponId.Value);
                    basketCheckoutMessage.BasketTotal = total - coupon.Amount;
                }
                else
                {
                    logger.LogDebug("No discount to apply");
                    basketCheckoutMessage.BasketTotal = total;
                }

                try
                {
                    //throw the basket checkout message to the service bus
                    await messageBus.PublishMessage(basketCheckoutMessage, "checkoutmessage",
                        Activity.Current.TraceId.ToString());

                    logger.LogDebug("Published checkout message");
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Unable to publish checkout message");
                    throw;
                }

                //TODO await basketRepository.ClearBasket(basketCheckout.BasketId);
                return Accepted(basketCheckoutMessage);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An exception occurred when checking out the basket");

                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
            }
        }
    }
}

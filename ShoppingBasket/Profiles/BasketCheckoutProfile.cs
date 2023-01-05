using AutoMapper;
using ShoppingBasket.Messages;
using ShoppingBasket.Models;

namespace ShoppingBasket.Profiles
{
    public class BasketCheckoutProfile : Profile
    {
        public BasketCheckoutProfile()
        {
            CreateMap<BasketCheckout, BasketCheckoutMessage>().ReverseMap();
        }
    }
}

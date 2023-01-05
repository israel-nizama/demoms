using AutoMapper;

namespace ShoppingBasket.Profiles
{
    public class BasketProfile : Profile
    {
        public BasketProfile()
        {
            CreateMap<Models.BasketForCreation, Entities.Basket>();
            CreateMap<Entities.Basket, Models.Basket>().ReverseMap();
        }
    }
}

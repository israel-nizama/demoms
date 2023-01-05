using AutoMapper;
using Discounts.Entities;
using Discounts.Models;

namespace Discounts.Profiles
{
    public class CouponProfile : Profile
    {
        public CouponProfile()
        {
            CreateMap<Coupon, CouponDto>().ReverseMap();
        }
    }
}

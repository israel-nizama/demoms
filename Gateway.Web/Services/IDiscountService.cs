using Gateway.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Web.Services
{
    public interface IDiscountService
    {
        Task<Coupon> GetDiscountForCode(string code);
    }
}

using System.ComponentModel.DataAnnotations;

namespace ShoppingBasket.Models
{
    public class BasketForCreation
    {
        [Required]
        public Guid UserId { get; set; }
    }
}

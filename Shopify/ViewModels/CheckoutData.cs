using System.ComponentModel.DataAnnotations;

namespace Shopify.Controllers
{
    public class CheckoutData
    {

        public int cartId { get; set; }
        public int governrateId { get; set; }
        [Required]
        [StringLength(150,MinimumLength =10)]
        public string striptoken { get; set; }
    }
}
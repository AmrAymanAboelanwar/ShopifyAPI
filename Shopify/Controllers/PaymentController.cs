using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopify.Helper;
using Shopify.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles ="Customer")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // add commentkokookk


        // checkout 
        [HttpPost]
        public  ActionResult Checkout(CheckoutData checkout)
        {
            if (ModelState.IsValid && checkout.cartId > 0 && checkout.governrateId > 0)
            {
                Response response = _paymentService.MakeCheckout(checkout, User.Identity);
                if (response.Status == "Success")
                    return NoContent();
                else if (response.Status == "Error")
                    return StatusCode(StatusCodes.Status500InternalServerError);
                else
                    NotFound();
            }
            return BadRequest();
        }





    }
}

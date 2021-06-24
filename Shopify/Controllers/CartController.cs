using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopify.Models;
using Shopify.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        // Add cart
        [HttpPost]
        [Authorize(Roles ="Customer")]
        public async Task<ActionResult<Cart>> AddCart()
        {
            Cart cart =  await _cartService.AddCart(new Cart(),User.Identity);
            return Ok(cart);
        }



        // get cart 
        [HttpGet]
        [Authorize(Roles = "Customer")]
        public ActionResult<Cart> GetAvaliableCart()
        {
            Cart cart =  _cartService.GetAvaliableCart(User.Identity);
            return Ok(cart);
        }



        // get cart by Id
        [HttpGet("{id}")]
        [Authorize(Roles = "Customer")]
        public ActionResult<Cart> GetCart(int id)
        {
            Cart cart = _cartService.GetCart(id,User.Identity);
            if(cart!=null)
                return Ok(cart);
            return NotFound();
        }



    }
}

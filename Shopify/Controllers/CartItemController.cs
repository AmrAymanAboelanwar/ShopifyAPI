using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopify.Helper;
using Shopify.Models;
using Shopify.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Controllers
{
    [Authorize(Roles ="Customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {

        private readonly CartItemService _cartItemService;

        public CartItemController(CartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }


        // get cartItems

        [HttpGet("{id}")] // cartId
        public ActionResult<List<CartItem>> GetCartItems(int id)
        {
           var result =  _cartItemService.GetCartItems(id);
            if (result.Status == "Success")
                return Ok(result.data);
            return NotFound();
        }



        // add cart Item to cart

        [HttpPost] 
        public ActionResult<List<CartItem>> AddCartItems([FromBody] CartItem cartItem)
        {
            if (ModelState.IsValid && cartItem.Quantity>0)
            {
                var result = _cartItemService.AddCartItemAsync(cartItem, User.Identity);
                if (result.Result.Status == "Success")
                    return Ok(result.Result); 
                else
                    return BadRequest(result.Result);
            }
            return BadRequest(ModelState);
        }


        // edit CartItem 
        [HttpPut] 
        public async Task<ActionResult> EditCartItemsAsync( CartItem cartItem)
        {
            if (ModelState.IsValid && cartItem.Quantity > 0)
            {
                var result = _cartItemService.EditCartItem(cartItem , User.Identity);
                if (result == null)
                {
                    return NotFound();
                }
              else if (result.Status == "Success"){
                    return NoContent();
                }
                else
                {
                    return BadRequest(result);
                }
            }
            return BadRequest();
        }



        // delete cartItem
       
        [HttpDelete("{id}")] 
        public  ActionResult DeleteCartItem(int id)
        {
            var result =  _cartItemService.DeleteCartItemAsync(id,User.Identity);
            if (result.Status == "Success")
                return NoContent();
              return NotFound(result.Message);
           
        }

    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shopify.Models;
using Microsoft.AspNetCore.Identity;
using Shopify.Repository;
using Microsoft.AspNetCore.Authorization;

namespace Shopify.Controllers
{  
    [Route("api/[controller]")]
    [ApiController]
    public class SellerController : ControllerBase
    {
        private ShopifyContext _shopifyContext;
        private readonly UserManager<ApplicationUser> _applicationUser;
        private readonly SellerService _sellerRepo;

        public SellerController(ShopifyContext shopifyContext ,UserManager<ApplicationUser> applicationUser,SellerService sellerRepo )
        {
            _shopifyContext = shopifyContext;
            _applicationUser = applicationUser;
            _sellerRepo = sellerRepo;
        }

        //get active sellers data
        [Authorize(Roles = "Admin")]
        [HttpGet("Active")]
        public ActionResult<List<Seller>> GetAllActive()
        {
            return _sellerRepo.GetAllActiveSellers();
        }


        //get waiting sellers data
        [Authorize(Roles = "Admin")]
        [HttpGet("waiting")]
        public ActionResult<List<Seller>> GetAllWaitingSeller()
        {
            return _sellerRepo.GetAllWaitingSellers();
        }





        //get seller by id
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public  ActionResult GetSellerById(string id)
        {
            Seller seller = _sellerRepo.GetSellerByID(id);
            if (seller == null)
            {
                return NotFound();
            }
            return Ok(seller);
        }




        //get seller info
        [Authorize(Roles = "Seller")]
        [HttpGet]
        public ActionResult GetSellerInfo()
        {
            Seller seller = _sellerRepo.GetSellerInfo(User.Identity);
            if (seller == null)
            {
                return NotFound();
            }
            return Ok(seller);
        }





        //edit seller
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApplicationUser>> EditSellerAsync(string id, [FromBody] ApplicationUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            else
            {
                user.Id = id;
                var result = await _sellerRepo.PutSeller( id,  user);
                if (result != null)
                    return NoContent();
                return NotFound();
            }

        }
        // delete seller
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeller(string id)
        {
            ApplicationUser app = new ApplicationUser();
            var seller = await _shopifyContext.Sellers.FindAsync(id);
            var sellerInUser = await _shopifyContext.Users.FindAsync(seller.SellerId);
            if (seller == null)
            {
                return NotFound();
            }
            if(sellerInUser != null)
            {
                sellerInUser.AdminLocked = true;
                seller.Isdeleted = true;
                await _shopifyContext.SaveChangesAsync();
            }        
            return NoContent();
        }




        // apply seller
        [Authorize(Roles = "Admin")]
        [HttpPost("apply/{id}")]
         public ActionResult ApplySeller(string id)
         {
           var result = _sellerRepo.ApplySeller(id);
            if (result == true)
                return NoContent();
            return NotFound();
         }





        // block seller
        [Authorize(Roles = "Admin")]
        [HttpPost("block/{id}")]
        public ActionResult BlockSeller(string id)
        {
            var result = _sellerRepo.BlockSeller(id);
            if (result == true)
                return NoContent();
            return NotFound();
        }



        // get orders
        [HttpGet("orders")]
        [Authorize(Roles = "Seller")]
        public ActionResult SellerOrder()
        {
           _sellerRepo.SellerOrders(User.Identity);

            return NoContent();

        }


    }
}

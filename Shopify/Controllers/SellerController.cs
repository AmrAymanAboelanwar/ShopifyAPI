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
{   [Authorize(Roles ="Admin")]
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
        [HttpGet("Active")]
        public ActionResult<List<Seller>> GetAllActive()
        {
            return _sellerRepo.GetAllActiveSellers();
        }


        //get waiting sellers data
        [HttpGet("waiting")]
        public ActionResult<List<Seller>> GetAllWaitingSeller()
        {
            return _sellerRepo.GetAllWaitingSellers();
        }





        //get seller by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Seller>> GetSellerById(string id)
        {
            var seller = await _shopifyContext.Sellers.Include( "ApplicationUser" ).FirstOrDefaultAsync(s=>s.SellerId==id);
            if (seller == null)
            {
                return NotFound();
            }
            return seller;
        }
        //edit seller
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
         [HttpGet("apply/{id}")]
         public ActionResult ApplySeller(string id)
         {
           var result = _sellerRepo.ApplySeller(id);
            if (result == true)
                return NoContent();
            return NotFound();
         }





        // block seller
        [HttpGet("block/{id}")]
        public ActionResult BlockSeller(string id)
        {
            var result = _sellerRepo.BlockSeller(id);
            if (result == true)
                return NoContent();
            return NotFound();
        }



        //// block seller
        //[HttpGet("aa")]
        //public ActionResult BlockSellera(string id)
        //{
        //    var result = _sellerRepo.SellerOrders();
            
        //        return NoContent();
          
        //}


    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shopify.Models;
using Shopify.Helper;
using Shopify.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Shopify.Controllers
{ 
    [Authorize(Roles ="Seller")]
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private PromotionService _promotionRepo;

        public PromotionController(PromotionService promotionRepo)
        {
            _promotionRepo = promotionRepo;
        }



        // get all seller's promotions
        [HttpGet]
        public ActionResult<List<Promotions>> GetAll()
        {
            return _promotionRepo.GetAllPromotions(User.Identity);
        }




        //get promotion by id
        [HttpGet("{id}")]
        public ActionResult<Promotions> GetPromotion(int id)
        {
            var result = _promotionRepo.GetPromotion(id , User.Identity);
            if (result == null)
                return NotFound();
            return Ok(result);
        }




        // add promotion
        [HttpPost]
        public async Task<ActionResult<Promotions>> AddPromotionAsync([FromForm] Promotions promotion ,IFormFile file)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                Promotions result =await  _promotionRepo.addPromotionAsync(promotion,file, User.Identity);

                return Ok(result);
            }

        }


        // add pormotion to product 
        [HttpPost("{PormotionId}")]
        public ActionResult AddPromotionToProduct(int PormotionId, [FromBody]int ProductId)
        {
            var result = _promotionRepo.AddPromotionToProduct(PormotionId, ProductId, User.Identity);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }



        //edit Promotions
        [HttpPut("{id}")]
        public async Task<ActionResult> EditPoromothionAsync(int id, [FromForm] Promotions promotion , IFormFile file)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                promotion.PromotionsId = id;
                var result = await _promotionRepo.EditPromotionAsync(promotion , file, User.Identity);
                if (result)
                    return NoContent();
                return NotFound();
            }

        }



        // delete promotion
        [HttpDelete("{id}")]
        public ActionResult deletePromotion(int id)
        {

            var result = _promotionRepo.Deletepromotion(id , User.Identity);
            if (result)
                return NoContent();
            return NotFound();
        }

    }
}

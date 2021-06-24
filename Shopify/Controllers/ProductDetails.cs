using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopify.Models;
using Shopify.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDetails : ControllerBase
    {

        ProductService _productRepo;
        ShopifyContext _shopifyContext;

        public ProductDetails(ProductService productRepo, ShopifyContext shopifyContext)
        {
            _productRepo = productRepo;
            _shopifyContext = shopifyContext;
        }




        //// add product details
        //[Authorize(Roles = "Seller")]
        //[HttpPost("{InventoryId}/{productId}")]
        //public  ActionResult AddProduct( [FromBody] ProductDetail[] productDetails, int productId , int InventoryId)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result =  _productRepo.AddProdctDetails(productId,InventoryId, productDetails, User.Identity);
        //        if (result.Status == "Success")

        //            return Ok(result);
        //        return StatusCode(StatusCodes.Status500InternalServerError, "data not valid");
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }

        //}


        
        

    }
}

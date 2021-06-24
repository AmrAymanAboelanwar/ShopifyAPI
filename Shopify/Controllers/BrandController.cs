using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopify.Models;
using Shopify.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {



        BrandService _brandRepo;

        public BrandController(BrandService brandRepo)
        {
            _brandRepo = brandRepo;
        }


        // get brands for sub category
        [AllowAnonymous]
        [HttpGet("subCat/{id}")]
        public ActionResult<List<Brand>> GetAllBrandForSubCat(int id)
        {
           var result=  _brandRepo.getBrandsForSubCategory(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }



        // get brand by id
        [HttpGet("{id}")]
        public ActionResult<Brand> GetBrand(int id)
        {
            var result = _brandRepo.GetBrand(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }



        // add brand
        [HttpPost]
        public ActionResult<Brand> AddBrand([FromBody] Brand brand)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            else
            {
                Brand result =  _brandRepo.AddBrand(brand);

                return Ok(result);
            }

        }


        //edit brand
        [HttpPut("{id}")]
        public ActionResult EditBrand(int id, [FromBody] Brand brand)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            else
            {
                brand.BrandId = id;
                var result = _brandRepo.EditBrandAsync(brand);
                if (result != null)
                    return NoContent();
                return NotFound();
            }

        }



        // delete category
        [HttpDelete("{id}")]
        public ActionResult deleteBrand(int id)
        {
            var result = _brandRepo.DeleteBrand(id);
            if (result.Status == "Success")
                return NoContent();
            else if (result.Status == "Error")
                return new StatusCodeResult(StatusCodes.Status405MethodNotAllowed);
            else
                return NotFound();
        }







    }
}

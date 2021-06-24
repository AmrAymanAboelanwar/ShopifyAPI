using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopify.Helper;
using Shopify.Models;
using Shopify.Repository;
using Shopify.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Controllers
{
    [Authorize(Roles = "Admin  , Employee")]
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        SubCategoryService _subCategoryRepo;

        public SubCategoryController(SubCategoryService subCategoryRepo)
        {
            _subCategoryRepo = subCategoryRepo;
        }


        // get sub category by id
        [AllowAnonymous]
        [HttpGet("{id}")]
        public ActionResult<SubCategory> GetSubCategory(int id)
        {
            var result = _subCategoryRepo.GetSubCategory(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }



        // get sub catergories by category id
        [AllowAnonymous]
        [HttpGet("Category/{id}")]
        public ActionResult<SubCategory> GetSubCategoryByCategoryId(int id)
        {
            var result = _subCategoryRepo.GetSubCategoryByCategoryId(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }


        // add sub category
        [HttpPost("{id}")]

        public async Task<ActionResult<SubCategory>> AddSubCategoryAsync(int id, [FromForm] SubCategory subCategory, IFormFile file) // not swagger [from form]
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            else
            {
                subCategory.CategoryId = id;
                SubCategory result = await _subCategoryRepo.AddSubCategoryAsync(subCategory, file);
                if (result == null)

                    return NotFound(new Response { Status = "Error", Message = "This Categoty Not Found" });

                return Ok(result);
            }

        }





        //edit sub category
        [HttpPut("{id}")]
        public async Task<ActionResult> EditSubCategoryAsync(int id, [FromForm] SubCategory subCategory, IFormFile file)// not swagger [from form]
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            else
            {
                subCategory.SubCategoryId = id;
                var result = await _subCategoryRepo.EditSubCategoryAsync(subCategory, file);
                if (result != null)
                    return NoContent();
                return NotFound();
            }

        }



        // delete sub category
        [HttpDelete("{id}")]
        public ActionResult<Category> deleteSubCategory(int id)
        {

            var result = _subCategoryRepo.DeleteSubCategory(id);
            if (result.Status == "Success")
                return NoContent();
            else if (result.Status == "Error")
                return new StatusCodeResult(StatusCodes.Status405MethodNotAllowed);
            else
                return NotFound();
        }



        //// add brand to sub category
        //[HttpPost("add-to-Brand")]
        //public ActionResult<Response> AddBrandToSubCategory([FromBody] BrandSubCatViewModel model)
        //{
        //    if (model.BrandId==0&&model.SubCatId==0)
        //    {
        //        return BadRequest();
        //    }
        //    var result = _subCategoryRepo.AddBrandToSubCategory(model.BrandId,model.SubCatId);
        //    if (result.Status=="Error")
        //        return NotFound(result);
        //    return Ok(result);
        //}



        // get all colors for specific sub-category
        [AllowAnonymous]
        [HttpGet("colors/{id}")]
        public ActionResult<List<string>> GetColors(int id)
        {
           var result = _subCategoryRepo.GetAllColorsForSubCategory(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result); 

        }


        // get all size for specific sub-category
        [AllowAnonymous]
        [HttpGet("sizes/{id}")]
        public ActionResult<List<string>> GetSizes(int id)
        {
            var result = _subCategoryRepo.GetAllSizesForSubCategory(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);

        }





        // get all size for specific sub-category
        [AllowAnonymous]
        [HttpGet("range-money/{id}")]
        public ActionResult<List<string>> GetRangeMoney(int id)
        {
            var result = _subCategoryRepo.GetRangeModenyForSubCategory(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);

        }


    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopify.Helper;
using Shopify.Models;
using Shopify.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Shopify.Controllers
{
    [Authorize(Roles = "Customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        //get all reviews
        [HttpGet]
        public ActionResult<List<Review>> GetAll()
        {
            return Ok(_reviewService.GetReviews());
        }


        //get all reviews for spicific product
        [HttpGet("{id}")]
        public ActionResult<List<Review>> getspicificReview(int id)
        {
            Response reault = _reviewService.GetReviewForSpicificProduct(id);
             if(reault.Status=="Success")
               return Ok(reault.data);
            return NotFound();
        }



        // get all review for spicific customer
        [HttpGet]
        [Route("customer-review/{id}")]
        public ActionResult<List<Review>> getcustumersReview(int id)
        {

            Response reault = _reviewService.GetReviewForCustomer(id, User.Identity);
            if (reault.Status == "Success")
                return Ok(reault.data);
            return NotFound();

           
        }




        //Edit Review
        [HttpPut("{id}")]
        public  ActionResult EditReview(int id, [FromBody] Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                review.ProductId = id;
                var result =  _reviewService.EditReview( User.Identity, review);
                if (result)
                    return NoContent();
                return NotFound();
            }
        }



        //add review
        [HttpPost("{ProductId}")]
        public ActionResult AddReviewToProduct(int ProductId, [FromBody] Review review)
        {
            if (ModelState.IsValid)
            {
                review.ProductId = ProductId;
                var result = _reviewService.AddNewReview(User.Identity, review);
                if (result.Status=="Success")
                {
                    return Ok(result.data);
                }else if (result.Status == "Error")
                {
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
                else if (result.Status == "Error2")
                {
                    return new StatusCodeResult(StatusCodes.Status405MethodNotAllowed);
                    
                }
                else
                {
                  return  NotFound();
                }
               
            }
            return BadRequest(ModelState);
        }


        // delete review
        [HttpDelete("{id}")]
        public ActionResult deletereview(int id)
        {

            var result = _reviewService.DeleteReview(id, User.Identity);
            if (result)
                return NoContent();
            return NotFound();
        }




    }
}

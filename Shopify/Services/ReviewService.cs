using Microsoft.EntityFrameworkCore;
using Shopify.Helper;
using Shopify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Shopify.Services
{
    public class ReviewService
    {
        private ShopifyContext _db;

        public ReviewService(ShopifyContext db)
        {
            _db = db;
        }


        //get all reviews
        public List<Review> GetReviews()
        {
            return _db.Reviews.Where(r => r.Isdeleted == false).ToList();
        }


        //get all reviews for spicific product
        public Response GetReviewForSpicificProduct(int id)
        {
            var product = _db.Products.FirstOrDefault(a => a.ProductId == id && a.IsdeletedBySeller == false);
            if (product != null)
                return new Response { Status = "Success", data = _db.Reviews.Where(a => a.ProductId == product.ProductId && a.Isdeleted == false).ToList() };
            else
                return new Response { Status = "Error", Message = "Product Not Found" };
        }


        // get all review for spicific customer
        public Response GetReviewForCustomer(int id, IIdentity customer)
        {
            var customerId = HelperMethods.GetAuthnticatedUserId(customer);
            var product = _db.Products.FirstOrDefault(a => a.ProductId == id && a.IsdeletedBySeller == false);
            if (product != null)
                return new Response { Status = "Success", data = _db.Reviews.Where(a => a.ProductId == id && a.CustomerId == customerId && a.Isdeleted == false).ToList() };
            else
                return new Response { Status = "Error", Message = "Product Not Found" };
        }



        ////Edit Review
        //public bool EditReview(IIdentity customer, Review Newreview)
        //{
        //    var customerId = HelperMethods.GetAuthnticatedUserId(customer);
        //    Review review = _db.Reviews.FirstOrDefault(a => a.ProductId == Newreview.ProductId && a.CustomerId == customerId && a.Isdeleted == false);
        //    if (Newreview != null)
        //    {
        //        review.review = Newreview.review;
        //        review.comment = Newreview.comment;
        //        _db.SaveChanges();
        //        UpdateProductReview(review.ProductId);
        //        return true;
        //    }
        //    return false;

        //}


        //Edit Review
        public bool EditReview(IIdentity customer, Review Newreview)
        {

             var customerId = HelperMethods.GetAuthnticatedUserId(customer);
            Review review = _db.Reviews.SingleOrDefault(pr => pr.CustomerId == customerId && pr.ProductId == Newreview.ProductId && pr.Isdeleted == false);
             if (review != null)
              {
                     review.review = Newreview.review;
                     review.comment = Newreview.comment;
                     _db.SaveChanges();
                    UpdateProductReview(review.ProductId);
                return true;
             }
                return false;
            
          
        }






        //add review
        public Response AddNewReview(IIdentity customer, Review review)
        {
            
            var customerId = HelperMethods.GetAuthnticatedUserId(customer);
          
            var product = _db.Products.Include(r=>r.Reviews).FirstOrDefault(a => a.ProductId == review.ProductId && a.IsdeletedBySeller==false && a.Active==true);
            if (product != null)
            {
                if (IsCustomerOrderedIt(review.ProductId, customerId)){
                    if (product.Reviews.SingleOrDefault(pr => pr.CustomerId == customerId && pr.ProductId == review.ProductId && pr.Isdeleted == false) == null)
                    {
                        review.CustomerId = customerId;
                        _db.Reviews.Add(review);
                        _db.SaveChanges();
                        UpdateProductReview(review.ProductId);
                        return new Response { Status = "Success", data = review };
                    }
                    return new Response { Status = "Error", Message = "This review with this customer is exist already " };
                }
                return new Response { Status = "Error2", Message = "Not Allow" };
            }
            return new Response { Status = "Error3", Message = "Product Not Found" };
        }




        private bool IsCustomerOrderedIt(int productId, string customerId)
        {
            List<CartItem> cartItems = _db.Carts.Include(r => r.CartItems.Where(r=>r.ProductId==productId && r.Isdeleted==false)).Where(r =>r.CustomerID == customerId && r.Isdeleted == false && r.Payed == true /*&& r.Status.StatusName=="Arrived"*/).Select(r=>r.CartItems).FirstOrDefault();
            if (cartItems != null)
                return true;
            return false;
        }




        //delete review 
        public bool DeleteReview(int id, IIdentity customer)
        {
            var customerId = HelperMethods.GetAuthnticatedUserId(customer);
            Review review = _db.Reviews.SingleOrDefault(pr => pr.CustomerId == customerId && pr.ProductId == id && pr.Isdeleted == false);
            if (review != null)
            {
                 review.Isdeleted = true;
                _db.SaveChanges();
                 UpdateProductReview(review.ProductId);
                return true;
             
            }
            return false;
        }



        // set product review()
        private void UpdateProductReview(int productId){
           Product product = _db.Products.Include(r=>r.Reviews).SingleOrDefault(p => p.ProductId == productId && p.IsdeletedBySeller==false && p.Active==true);
           var reviews = product.Reviews.Where(r => r.Isdeleted == false).Select(s => s.review);
           product.Rate =(double)reviews.Sum() / reviews.Count(); // set new rate
           _db.SaveChanges();
        }

    }
}

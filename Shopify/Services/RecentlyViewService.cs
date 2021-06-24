using Microsoft.EntityFrameworkCore;
using Shopify.Helper;
using Shopify.Models;
using Shopify.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Shopify.Repository
{
    public class RecentlyViewService
    {

        ShopifyContext _db;
        ProductService _productService;
        public RecentlyViewService(ShopifyContext db , ProductService productService)
        {
            _db = db;
            _productService = productService;
        }

     

      

        // add  recently views to customer

        public Response AddRecentlyViewToCustomer(int productId, IIdentity AuthUser)
        {


            // check if product is already exist
            var product = _db.Products.FirstOrDefault(p => p.ProductId == productId &&p.Active == true);
            if (product != null)
            {

                string userId = HelperMethods.GetAuthnticatedUserId(AuthUser);
                if (userId != null)
                {
                    if (_db.Views.FirstOrDefault(rv => rv.CustomerId == userId && rv.ProductId == productId) != null)
                    {
                        return new Response { Status = "Found Error", Message = "The Relation is Existed already" };
                    }
                    _db.Views.Add(new View { CustomerId = userId, ProductId = productId });
                    _db.SaveChanges();

                    return new Response { Status = "Success", Message = "Product Added Successfully" };
                }
                else
                {
                    return new Response { Status = "Error", Message = "User Not Found" };
                }

            }
            return new Response { Status = "Error", Message = "Product Not Found" };


        }

           public Response GetRecentlyViewBy(int id, IIdentity identity)
           {
            string userId = HelperMethods.GetAuthnticatedUserId(identity);

            if (_productService.GetProductById(id) != null)
            {
                var product = _db.Views.Include(r => r.Product).ThenInclude(i => i.ProductImages).Where(v => v.CustomerId == userId && v.ProductId == id).Select(r => r.Product);
               if(product!=null)
                return new Response() { Status = "Success", data = product };
                return new Response() { Status = "Error", Message="Product Not Found In RecentlyView" };
            
            }
            else
            {
                return new Response() { Status = "Error", data = null ,Message="Product Not Found" };
            }
        }


        // get customer recently views

        public List<Product> GetRecentlyViewFoCustomer(IIdentity AuthUser)
        {
            string userId = HelperMethods.GetAuthnticatedUserId(AuthUser);
            var r = _db.Views.Include(r => r.Product).ThenInclude(i => i.ProductImages).Where(v => v.CustomerId == userId).Select(r => r.Product).ToList();
            return r;

        }


        // add to favourite


        public Response AddToFavourite(int productId, IIdentity AuthUser)
        {

            // check if product is already exist
            var product = _db.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                string userId = HelperMethods.GetAuthnticatedUserId(AuthUser);
                var ProductView = _db.Views.FirstOrDefault(v => v.CustomerId == userId && v.ProductId == productId);
                if (ProductView.IsFav)
                {
                    return new Response { Status = "Error", Message = "Product Is Already In Favourite" };
                }

                ProductView.IsFav = true;
                _db.SaveChanges();
                return new Response { Status = "Success", Message = "Product Add In Favourite" };

            }
            return new Response { Status = "Error", Message = "Product Not Found" };


        }





        // remove from favourite


        public Response RemoveFromFavourite(int productId, IIdentity AuthUser)
        {
            string userId = HelperMethods.GetAuthnticatedUserId(AuthUser);
            var ProductView = _db.Views.FirstOrDefault(v => v.CustomerId == userId && v.ProductId == productId);
            if (!ProductView.IsFav)
            {
                return new Response { Status = "Error", Message = "Product already remove from Favourite" };
            }

            ProductView.IsFav = false;
            _db.SaveChanges();
            return new Response { Status = "Success", Message = "Product Remove From Favourite" };



        }



        // get customer favoutites


        public List<Product> GetCustomerFavoutites(IIdentity AuthUser)
        {
            string userId = HelperMethods.GetAuthnticatedUserId(AuthUser);
            var favourites = _db.Views.Include(r => r.Product).ThenInclude(i => i.ProductImages).Where(v => v.CustomerId == userId && v.IsFav == true).Select(r => r.Product).ToList();
            return favourites;

        }


    }
}

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
    public class CartService
    {

        private readonly ShopifyContext _db;
        public CartService(ShopifyContext db)
        {
            _db = db;
        }


        //  create cart 
        public async Task<Cart> AddCart(Cart cart , IIdentity Customer)
        {
            cart.CustomerID = HelperMethods.GetAuthnticatedUserId(Customer); 
            await _db.AddAsync(cart);
            _db.SaveChanges();
            return cart;
        }


        // get avaliable cart
        public Cart GetAvaliableCart(IIdentity Customer)
        {
            var CustomerID = HelperMethods.GetAuthnticatedUserId(Customer);
            Cart cart = _db.Carts.Include(i => i.CartItems.Where(r => r.Isdeleted == false)).ThenInclude(r => r.Product).ThenInclude(r=>r.InventoryProducts).Include(i => i.CartItems.Where(r => r.Isdeleted == false)).ThenInclude(r => r.Product).ThenInclude(r => r.ProductImages).SingleOrDefault(c => c.CustomerID == CustomerID && c.Payed == false && c.Isdeleted == false);
            return cart;
        }


        // edit total cost for cart 
        public Cart GetAvaliableCart(Cart cart)
        {
            Cart Cart2 = _db.Carts.SingleOrDefault(c => c.CartId ==cart.CartId && c.Payed == false && c.Isdeleted == false);
            if(Cart2 != null)
            {
                Cart2.Cost = cart.Cost;
                _db.SaveChanges();
                return cart;

            }
            return null;
        }

        // get  cart
        public Cart GetCart(int id , IIdentity Customer)
        {
            var CustomerID = HelperMethods.GetAuthnticatedUserId(Customer);
            Cart cart = _db.Carts.Include(i=>i.CartItems).SingleOrDefault(c => c.CustomerID == CustomerID && c.CartId == id && c.Isdeleted == false);
            return cart;
        }
    }
}

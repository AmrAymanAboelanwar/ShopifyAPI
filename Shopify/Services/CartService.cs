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



        public bool ChangeCartStatus(int id, int statusId, IIdentity identity)
        {
            string employeeId= HelperMethods.GetAuthnticatedUserId(identity);
            Status status = _db.Statuses.FirstOrDefault(s => s.StatusId == statusId && s.StatusName!=StatusEnum.Approved.ToString()&& s.Isdeleted == false );
            Cart cart = _db.Carts.Include(r=>r.Status).FirstOrDefault(s => s.CartId == id && s.Isdeleted == false && s.Status.StatusName !=StatusEnum.Arrived.ToString());
            
            if (status != null && cart !=null )
            {
                if (statusId == (int)StatusEnum.Shipping)
                {
                    cart.ShippingDate = DateTime.Now;
                }else
                {
                    cart.DueDate = DateTime.Now;
                }
                cart.StatusId = statusId;
                cart.EmployeeId = employeeId;
                _db.SaveChanges();
                return true;
            }
            return false;
             
        }

        public List<Cart> GetCartsPayedForCustomer(IIdentity customer)
        {
           var customerId= HelperMethods.GetAuthnticatedUserId(customer);
           List<Cart> carts = _db.Carts.Include(r => r.Status).Where(r => r.CustomerID == customerId && r.Payed == true && r.Isdeleted==false).ToList();
            return carts;
        }

        public List<Cart> GetNotArrivedCarts()
        {
            return _db.Carts.Include(s=>s.Status).Where(r => r.Isdeleted == false && r.Payed == true && r.Status.StatusName !=StatusEnum.Approved.ToString()).ToList();
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

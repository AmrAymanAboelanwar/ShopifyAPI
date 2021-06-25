using Shopify.Controllers;
using Shopify.Helper;
using Shopify.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Shopify.Services
{
    public class PaymentService
    {
        ShopifyContext _db;
        public PaymentService(ShopifyContext db)
        {
            _db = db;
        }




        // checkout

        public Response MakeCheckout(CheckoutData  checkoutData,IIdentity customer)
        {
           string customerId = HelperMethods.GetAuthnticatedUserId(customer);
            Cart cart = _db.Carts.FirstOrDefault(c =>c.CustomerID == customerId && c.CartId == checkoutData.cartId && c.Isdeleted == false);
            if (cart != null)
            {

                if (cart.Payed == false)
                {

                    // add shipping value with in the total cost of cart and pay all 
                    double amount;
                    if (AddShippingValue(cart, checkoutData.governrateId,out amount))
                    {
                        _db.SaveChanges();
                        // makePaynebt
                        if (Checkout(checkoutData.striptoken, customerId, cart,amount))
                        {
                            cart.Payed = true;
                            cart.StatusId =2;              // approved
                            cart.OrderDate = DateTime.Now;
                            _db.SaveChanges();
                            // payed done
                            return new Response { Status = "Success", Message = "payed done successully" };
                        }
                        return new Response { Status = "Error", Message = "Error In Payment" };
                    }

                    // return governrate not found 
                    return new Response { Status = "Error2", Message = "governrate not found " };
                }

                // already payed
                return new Response { Status = "Error3", Message = "is already payed" };
            }
            else
            {
                // not found
                return new Response { Status = "Error4", Message = "cart not found " };
            }

        }



        // checkout

        private bool Checkout(string striptoken, string customerId, Cart cart,double amount)
        {
            ApplicationUser customer = _db.ApplicationUsers.FirstOrDefault(id => id.Id == customerId);
            var options = new ChargeCreateOptions
            {
                Amount = ((long?)amount),
                Currency = "EGP",
                Description = "Order Cost",
                Source = striptoken,
                ReceiptEmail = customer.Email,
                
            };
            var service = new ChargeService();
            Charge charge = service.Create(options);
            if (charge.Status == "succeeded")
            {
                Payment payment = new Payment() { amount= amount ,type="online" ,CreatedAt=DateTime.Now};
                _db.Payments.Add(payment);
                _db.SaveChanges();
                cart.PaymentId = payment.PaymentId;
                return true;
            }
            else
                return false;
            
        }







        private bool  AddShippingValue(Cart cart, int governrateId , out double amount)
        {
          Governorate governorate = _db.Governorates.FirstOrDefault(g => g.GovernorateId == governrateId && g.Isdeleted == false);
            amount = 0;
            if(governorate != null)
            {
                 amount  = cart.Cost + governorate.ShippingValue;
                 cart.GovernrateId = governrateId;
                return true;
            }
            return false;
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopify.Helper;
using Shopify.Models;
using Shopify.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Shopify.Repository
{
    public class CustomerServices
    {
        ShopifyContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerServices(ShopifyContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db ;
            _userManager = userManager;
        }

        public void AddCustomerId(string id)
        {
             _db.Customers.Add(new Customer() { CustomerId = id });
             _db.SaveChanges(); 
        }

        // get all Customer
        public List<ApplicationUser> GetAllCustomers()
        {
            List<ApplicationUser> customerData = new List<ApplicationUser>();
            var customerId = _db.Customers.ToList();

            foreach (var item in customerId)
            {
                var seller = _db.Users.Where(c => c.AdminLocked == false).FirstOrDefault(a => a.Id == item.CustomerId);

                customerData.Add(seller);
            }
            return customerData;

        }
        //edit Customer
        public async Task<ApplicationUser> editCustomer(UserData userData, IIdentity user)
        {
            var customer =  _db.ApplicationUsers.FirstOrDefault(c=>c.Id==HelperMethods.GetAuthnticatedUserId(user));
            customer.Fname = userData.Fname;
            customer.Lname = userData.Lname;
            string token = await _userManager.GenerateChangeEmailTokenAsync(customer , userData.Email);
            await _userManager.ChangeEmailAsync(customer , userData.Email , token);
         
            await _db.SaveChangesAsync();
            return customer;
        }


        // edit customer address 


        public  Customer EditCustomerAddress(string Address , IIdentity user)
        {
            Customer customer = _db.Customers.Include(r => r.ApplicationUser).FirstOrDefault(s => s.CustomerId == HelperMethods.GetAuthnticatedUserId(user));
            customer.ApplicationUser.Address = Address;
             _db.SaveChangesAsync();
            return customer;
        }



        // edit customer address 


        public Customer EditCustomerPassword(string Address, IIdentity user)
        {
            Customer customer = _db.Customers.Include(r => r.ApplicationUser).FirstOrDefault(s => s.CustomerId == HelperMethods.GetAuthnticatedUserId(user));
            customer.ApplicationUser.Address = Address;
            _db.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> EditCustomerPasswordAsync(ChangePasswordViewModel changePassword, IIdentity customer)
        {
           var user = await  _userManager.FindByIdAsync(HelperMethods.GetAuthnticatedUserId(customer));
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetPassResult = await _userManager.ResetPasswordAsync(user,token, changePassword.NewPassword);
            if (resetPassResult.Succeeded)
                return true;
            return false;
            

        }
    }
}

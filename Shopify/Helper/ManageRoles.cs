using Microsoft.AspNetCore.Identity;
using Shopify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Helper
{
    sealed public class ManageRoles
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;


        public ManageRoles(RoleManager<IdentityRole> roleManager , UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }



        // add to customer 

       public async Task<bool> AddToCustomerRole(ApplicationUser user)
        {
            try
            {
                if (!await roleManager.RoleExistsAsync("Customer"))
                    await roleManager.CreateAsync(new IdentityRole("Customer"));

                if (await roleManager.RoleExistsAsync("Customer"))
                {
                    await userManager.AddToRoleAsync(user, "Customer");

                }
                return true;
               
            }
            catch(Exception e)
            {
                return false;
            }
        }


        // add to  employee

        public async Task<bool> AddToEmployeeRole(ApplicationUser user)
        {
            try
            {
                if (!await roleManager.RoleExistsAsync("Employee"))
                    await roleManager.CreateAsync(new IdentityRole("Employee"));

                if (await roleManager.RoleExistsAsync("Employee"))
                {
                    await userManager.AddToRoleAsync(user, "Employee");

                }
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }




        // add to  seller

        public async Task<bool> AddToSellerRole(ApplicationUser user)
        {
            try
            {
                if (!await roleManager.RoleExistsAsync("Seller"))
                    await roleManager.CreateAsync(new IdentityRole("Seller"));

                if (await roleManager.RoleExistsAsync("Seller"))
                {
                    await userManager.AddToRoleAsync(user, "Seller");

                }
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }



        // add to  admin

        public async Task<bool> AddToSAdminRole(ApplicationUser user)
        {
            try
            {
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));

                if (await roleManager.RoleExistsAsync("Admin"))
                {
                    await userManager.AddToRoleAsync(user, "Admin");

                }
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}

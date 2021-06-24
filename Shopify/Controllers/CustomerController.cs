using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shopify.Repository.Interfaces;
using Shopify.Repository;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Security.Principal;
using Shopify.Helper;
using Shopify.ViewModels;

namespace Shopify.Controllers
{
   
    [Route("api/[controller]")]
    [Authorize(Roles ="Customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private ShopifyContext _shopifyContext;
        private readonly UserManager<ApplicationUser> _applicationUser;
        private readonly CustomerServices _customerRepo;

        public CustomerController(ShopifyContext shopifyContext, UserManager<ApplicationUser> applicationUser, CustomerServices customerRepo)
        {
            _shopifyContext = shopifyContext;
            _applicationUser = applicationUser;
            _customerRepo = customerRepo;
        }

        //get sellers data
        [HttpGet("All")]
        public ActionResult<List<ApplicationUser>> GetAll()
        {
            return _customerRepo.GetAllCustomers();
        }



        //get customer by id
        [HttpGet]
      
        public async Task<ActionResult<Customer>> GetCustomerById()
        {
            var CustomerId = HelperMethods.GetAuthnticatedUserId(User.Identity);
            var customer = await _shopifyContext.Customers.Include("ApplicationUser").FirstOrDefaultAsync(s => s.CustomerId == CustomerId);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }



        //edit customer
        [HttpPut]
        public async Task<ActionResult<ApplicationUser>> EditCustomerAsync([FromBody] UserData userData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            else
            {
                
                var result = await _customerRepo.editCustomer(userData, User.Identity);
                if (result != null)
                    return NoContent();
                return NotFound();
            }

        }


        //edit customer
        [HttpPut("change-address")]
        public ActionResult EditCustomerAddress([FromBody]  EditAddress address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            else
            {
                _customerRepo.EditCustomerAddress(address.Address, User.Identity);
                    return NoContent();
            }
        }


        //edit customer
        [HttpPut("password")]
        public async Task<ActionResult> EditCustomerPasswordAsync([FromBody] ChangePasswordViewModel changePassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            else
            {
              bool result  =  await  _customerRepo.EditCustomerPasswordAsync(changePassword, User.Identity);
                if (result)
                  return NoContent();
                return  new StatusCodeResult(StatusCodes.Status500InternalServerError);

            }
        }






        // delete customer
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            ApplicationUser app = new ApplicationUser();
            var customer = await _shopifyContext.Customers.FindAsync(id);
            var customerInUser = await _shopifyContext.Users.FindAsync(customer.CustomerId);
            if (customer == null)
            {
                return NotFound();
            }
            if (customerInUser != null)
            {
                customerInUser.AdminLocked = true;
                customer.Isdeleted = true;
                await _shopifyContext.SaveChangesAsync();
            }
            return NoContent();
        }




    }
}

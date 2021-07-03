using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shopify.Helper;
using Shopify.Models;
using Shopify.Repository;
using Shopify.Repository.Interfaces;
using Shopify.ViewModels;
using Shopify.ViewModels.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shopify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> userManager;
      
        private IAuthentication _authentication;

        public AuthenticationController(UserManager<ApplicationUser> userManager,   IAuthentication authentication)
        {
            this.userManager = userManager;
            _authentication = authentication;
        }
     

        // sign up customer 

        [HttpPost]
        [Route("register-customer")]

        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result= await _authentication.RegisterCustomerAsync(model);
                if (!result.IsAuthenticated)
                {
                    return BadRequest(result.Message);
                }
                return Ok(result);
            }
            return BadRequest(ModelState);
        }






        // sign up emplyee
        [HttpPost]
        [Route("register-emplyee")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Registeremplyee([FromBody] RegisterEmployeeViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.Salary > 0)
                {
                    var result = await _authentication.RegisterEmployeeAsync(model);
                    if(result.Status== "Success")
                     return Ok(result);
                }
                return Conflict();
            }
            return BadRequest(ModelState);
        }


        // sign up as seller 



        [HttpPost]
        [Route("register-seller")]
        
        public async Task<IActionResult> Registerseller([FromForm] RegisterSellerModel model  ,  IFormFile[] files)
        {

            if (ModelState.IsValid)
            {
                if (files.Length == 4)
                {
                    var result = await _authentication.RegisterSellerAsync(model , files);
                    if (!result.IsAuthenticated)
                    {
                        return BadRequest(result.Message);
                    }
                    return Ok(result.Message);
                }
                else
                {
                    return BadRequest();
                }
            }
            return BadRequest(ModelState);
        }



        [HttpPost]
        [Route("register-admin")]

        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authentication.RegisterAdminAsync(model);
                if (!result.IsAuthenticated)
                {
                    return BadRequest(result.Message);
                }
                return Ok(result);
            }
            return BadRequest(ModelState);
        }


        // login


        [HttpPost]
        [Route("login")]

        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authentication.LoginAsync(model);
                if (!result.IsAuthenticated)
                {
                    return BadRequest(result.Message);
                }
                return Ok(result);
            }
            return BadRequest(ModelState);
        }



        // login facebook 
        [HttpPost("login-facebook")]
        public async Task<IActionResult> LoginFacebook([FromBody] FacebookLoginModel Data)
        {
            if (ModelState.IsValid)
            {

                var result = await _authentication.LoginWithFacebookAsync(Data.accessToken);
                if (!result.IsAuthenticated)

                    return BadRequest(result.Message);

                return Ok(result);
            }
            return BadRequest(ModelState);
        }






        // login google 
        [HttpPost("login-google")]
        public async Task<IActionResult> LoginGoogle([FromForm] GoogleLoginModel Data)
        {
            Data.idToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImI2ZjhkNTVkYTUzNGVhOTFjYjJjYjAwZTFhZjRlOGUwY2RlY2E5M2QiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJhY2NvdW50cy5nb29nbGUuY29tIiwiYXpwIjoiNzcyMDM3OTE2OTczLWRzc2o1ZmltbDBrZjNwaGRrbXFxbTY2ZDNpaDlnNjE0LmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwiYXVkIjoiNzcyMDM3OTE2OTczLWRzc2o1ZmltbDBrZjNwaGRrbXFxbTY2ZDNpaDlnNjE0LmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwic3ViIjoiMTExNDI1OTMyMzIwOTg0ODkyODcyIiwiZW1haWwiOiJhbXIyNTExMTk5N0BnbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwiYXRfaGFzaCI6Ik40LV9MT1pPLUg1NDJCajk5SXZRMlEiLCJuYW1lIjoiYW1yIGF5bWFuIiwicGljdHVyZSI6Imh0dHBzOi8vbGgzLmdvb2dsZXVzZXJjb250ZW50LmNvbS9hL0FBVFhBSnpnSWRTS0NCNl9SUW9SSXo2Mk1kOUpmUldxRUZqY2xDQV9ac1paPXM5Ni1jIiwiZ2l2ZW5fbmFtZSI6ImFtciIsImZhbWlseV9uYW1lIjoiYXltYW4iLCJsb2NhbGUiOiJlbiIsImlhdCI6MTYyNTMxMDQ1MiwiZXhwIjoxNjI1MzE0MDUyLCJqdGkiOiIyNDZkYWIyYzYyOTBlNjZjMDU5YmJkMDczMDhkOWE4ZTUyZTk3YzI5In0.F0FWpExvxqCPxqua6K-e52r5MA5TmcxY02e0I1s1TnIBoAQyoNr5wFNCk7-aZQcYPTCh9XeqUeMHHM7ZMLHU5rgA4AYQ0ir77wusFrqIYf9ghAxz6eOMpxM1Z2x81nBV9J6Zz5p3cL0PC7et7jNpK6CQ-RYpvsXojr2zezamv7E7HAzjZta8UVM8igqW9ICcTuYJWCYdUAXVAbbQjamVVNnlfAXFKvinbavBnSsugYnm099ggbxlrLRIB963ThC2gB-R5ajkY_OGjUECE2e7lJsQG8HNy4xsCrsT_YtCIeMZUho1dk5rL7XkEPegHjJOhycEwMpi-mczMvoQlxcd1g";
            if (ModelState.IsValid)
            {

                var result = await _authentication.LoginWithGoogleAsync(Data);
                if (!result.IsAuthenticated)

                    return BadRequest(result.Message);

                return Ok(result);
            }
            return BadRequest(ModelState);
        }



        // forget password
        [HttpGet("forget-password/{email}")]

        public async Task<ActionResult> ForgetPassword(string email)
        {
            ForgetPasswordModel model = new ForgetPasswordModel{ Email=email};
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
             var result = await _authentication.ForgetPasswordAsync(model);
                if (result.Status == "Success")
                    return Ok();
                else if(result.Status=="Error")
                return BadRequest(new Response { Status=result.Status,Message=result.Message});
                return StatusCode(StatusCodes.Status500InternalServerError, result);

            }
        }



        // reset password
        [HttpPost("reset-password")]

        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var result = await _authentication.ResetPasswordAsync(model);
                if (result.Status == "Success")
                    return Ok();

                return BadRequest(new Response { Status = result.Status, Message = result.Message });

            }
        }


    }
}

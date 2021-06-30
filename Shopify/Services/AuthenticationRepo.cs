using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shopify.Helper;
using Shopify.Models;
using Shopify.Repository.Interfaces;
using Shopify.Services.Interfaces;
using Shopify.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Shopify.Repository
{

 

    public class AuthenticationRepo : IAuthentication
    {
        private readonly FacebookService _facebookService;
        private readonly UserManager<ApplicationUser> _userManager;
        private ManageRoles _manageRoles;
        private CustomerServices _customerRepo;
        private EmployeeService _employeeRepo;
        private SellerService _sellerRepo;
        private  JwtHelper _jwt;
        EmailHelper _emailHelper ;

        public AuthenticationRepo(UserManager<ApplicationUser> userManager, ManageRoles manageRoles, CustomerServices customerRepo, EmployeeService employeeRepo, SellerService sellerRepo, IOptions<JwtHelper> jwt , EmailHelper emailHelper , FacebookService facebookService)
        {
            _manageRoles = manageRoles;
            _userManager = userManager;
            _customerRepo=customerRepo;
            _sellerRepo = sellerRepo;
            _employeeRepo= employeeRepo;
            _jwt = jwt.Value;
             _emailHelper= emailHelper;
            _facebookService = facebookService;


        }

     

        public async Task<ResponseAuth> LoginAsync(LoginModel model)
        {
          var user =  await _userManager.FindByEmailAsync(model.Email);
            if(user==null||!await _userManager.CheckPasswordAsync(user, model.password))
            {
                return new ResponseAuth { Message = "email or password not valid" };
            }
            if (user.AdminLocked)
            {
                 return new ResponseAuth { Message = "email or password not valid" };
            }
           
            var token = await CreateJwtToken(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            return new ResponseAuth
            {
                Email = user.Email,
                UserName = user.Email,
                Role =userRoles[0],
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireDate = token.ValidTo,
                IsAuthenticated = true

            };

        }



        // login with facebook


        public async Task<ResponseAuth> LoginWithFacebookAsync(string accessToken)
        {
            var validTokenResult =await _facebookService.ValidateAccessTokenAsync(accessToken);
            if (validTokenResult==null)
                return new ResponseAuth() { Message = "Not Valid", IsAuthenticated = false };
            else
            {
               var userInfo  = await _facebookService.GetUserDataAsync(accessToken);
                 var user        =  await  _userManager.FindByEmailAsync(userInfo.email);
                if (user != null)
                {
                    var token = await CreateJwtToken(user);
                    return new ResponseAuth
                    {
                        Email = user.Email,
                        UserName = user.UserName,
                        Role = "Customer",
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        ExpireDate = token.ValidTo,
                        IsAuthenticated = true

                    };


                }
                else
                {
                    string username = userInfo.email.Split('@')[0];
                    ApplicationUser newUser = new ApplicationUser()
                    {
                       Fname = userInfo.first_name,
                        Lname = userInfo.last_name,
                        Email = userInfo.email,
                        UserName = username,
                        Address = "Mansoura",  // will be edit
                        SecurityStamp = Guid.NewGuid().ToString()
                    };
                    var result = await _userManager.CreateAsync(newUser);
                    _customerRepo.AddCustomerId(newUser.Id);

                    await _manageRoles.AddToCustomerRole(newUser);

                    var token = await CreateJwtToken(newUser);
                    return new ResponseAuth
                    {
                        Email = newUser.Email,
                        UserName = newUser.UserName,
                        Role = "Customer",
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        ExpireDate = token.ValidTo,
                        IsAuthenticated = true

                    };
                }
            }



         }




        // login with google 

        public async Task<ResponseAuth> LoginWithGoogleAsync(Payload payload)
        {
            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                string username = payload.Email.Split('@')[0];
                ApplicationUser newUser = new ApplicationUser()
                {

                    Fname = payload.Name,
                    Lname = payload.FamilyName,
                    Email = payload.Email,
                    UserName = username,
                    Address = "mansoura"  // will be edit
                };
                var result = await _userManager.CreateAsync(newUser);
                _customerRepo.AddCustomerId(newUser.Id);

                await _manageRoles.AddToCustomerRole(newUser);

                var token = await CreateJwtToken(newUser);
                return new ResponseAuth
                {
                    Email = newUser.Email,
                    UserName = newUser.UserName,
                    Role = "Customer",
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpireDate = token.ValidTo,
                    IsAuthenticated = true

                };
            }

            else
            {
                var token = await CreateJwtToken(user);
                return new ResponseAuth
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Role = "Customer",
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpireDate = token.ValidTo,
                    IsAuthenticated = true

                };
            }
        }





        public async Task<ResponseAuth> RegisterCustomerAsync(RegisterModel model)
        {
            string username = model.Email.Split('@')[0];

            if (await _userManager.FindByEmailAsync(model.Email)!=null)
                return new ResponseAuth{Message="Email is already Exist"};

            if (await _userManager.FindByNameAsync(username) != null)
                return new ResponseAuth { Message = "Username is already Exist" };

            ApplicationUser user = new ApplicationUser()
            {
                Fname = model.Fname,
                Lname = model.Lname,
                Address = model.Address,
                Email = model.Email,
                UserName =username,
                SecurityStamp = Guid.NewGuid().ToString()
            };

                var result = await _userManager.CreateAsync(user, model.Password);
              if (!result.Succeeded)
              {
                var errors = "";
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}";
                }
                return new ResponseAuth { Message =errors };

               }

               _customerRepo.AddCustomerId(user.Id);

                await _manageRoles.AddToCustomerRole(user);
                var token =await CreateJwtToken(user);
            return new ResponseAuth
            {
                Email = user.Email,
                UserName = username,
                Role = "Customer",
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireDate = token.ValidTo,
                IsAuthenticated = true

             };
               
           

        }

        public async Task<Response> RegisterEmployeeAsync(RegisterEmployeeViewModel model)
        {
            string username = model.Email.Split('@')[0];
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return new Response { Status="Error" ,Message = "Email is already Exist" };

            if (await _userManager.FindByNameAsync(username) != null)
                return new Response { Status = "Error",Message = "Username is already Exist" };

            ApplicationUser user = new ApplicationUser()
            {
                Fname = model.Fname,
                Lname = model.Lname,
                Address = model.Address,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName=username
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}";
                }
                return new Response { Message = errors };

            }


            await _manageRoles.AddToEmployeeRole(user);
            _employeeRepo.AddEmployeeId(user.Id, model.Salary, DateTime.Now);
            return new Response { Status = "Success", Message = "done" };
           
        }

        public async Task<ResponseAuth> RegisterSellerAsync(RegisterSellerModel model , IFormFile[] files)
        {
            string username = model.Email.Split('@')[0];
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return new ResponseAuth { Message = "Email is already Exist" };

            if (await _userManager.FindByNameAsync(username) != null)
                return new ResponseAuth { Message = "Username is already Exist" };

            ApplicationUser user = new ApplicationUser()
            {
                Fname = model.Fname,
                Lname = model.Lname,
                Address = model.Address,
                Email = model.Email,
                UserName = username,
                SecurityStamp = Guid.NewGuid().ToString(),
                AdminLocked = true,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}";
                }
                return new ResponseAuth { Message = errors };

            }

            var pathes = await FileHelper.SaveFilesSellerDocumentsAsync(user.Id, files);
            _sellerRepo.AddSellerId(user.Id ,model.StoreName , pathes);

            await _manageRoles.AddToSellerRole(user);
            return new ResponseAuth { Message = "Sign up has done successully pleaze wait until we review your documents " ,IsAuthenticated=true };
        }



        public async Task<ResponseAuth> RegisterAdminAsync(RegisterModel model)
        {
            string username = model.Email.Split('@')[0];
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return new ResponseAuth { Message = "Email is already Exist" };

            if (await _userManager.FindByNameAsync(username) != null)
                return new ResponseAuth { Message = "Username is already Exist" };

            ApplicationUser user = new ApplicationUser()
            {
                Fname = model.Fname,
                Lname = model.Lname,
                Address = model.Address,
                Email = model.Email,
                UserName =username ,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}";
                }
                return new ResponseAuth { Message = errors };

            }

            await _manageRoles.AddToSAdminRole(user);
            var token = await CreateJwtToken(user);
            return new ResponseAuth
            {
                Email =user.Email ,
                UserName = username,
                Role = "Admin",
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireDate = token.ValidTo,
                IsAuthenticated = true

            };
        }


        public async Task<Response> ForgetPasswordAsync(ForgetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
           
            if (user == null)
            {
                return new Response { Status = "Error", Message = "this email not valid" };
            }
            else
            {
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodeToken = Encoding.UTF8.GetBytes(token);
                var validToken = WebEncoders.Base64UrlEncode(encodeToken);

               
               var result = await _emailHelper.SendEmailAsync(model.Email, validToken);
                if(result)
                return  new Response {Status = "Success", Message = "this email not valid" };
                return new Response { Status = "Error2", Message = "please try again" };
            }
        }



        public async Task<Response> ResetPasswordAsync(ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new Response { Status = "Error", Message = "this email not valid" };
            }
            else
            {
                var decodeToken = WebEncoders.Base64UrlDecode(model.Token);
                var normalToken = Encoding.UTF8.GetString(decodeToken);


                var result = await _userManager.ResetPasswordAsync(user, normalToken, model.Password);
                if (result.Succeeded)
                    return new Response { Status = "Success", Message = "this email not valid" };
                return new Response { Status = "Error", Message = "invalid password"};
            }
        }





  














        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

      
    }
}

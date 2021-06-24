using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.ViewModels
{
    public class RegisterModel
    {

        [Required(ErrorMessage = "First Name is required")]
        public string Fname { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string Lname { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }


       

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

      

    }
}

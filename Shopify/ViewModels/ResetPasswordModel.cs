using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.ViewModels
{
    public class ResetPasswordModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string CPassword { get; set; }

        [Required]
        public string Token { get; set; }
    }
}

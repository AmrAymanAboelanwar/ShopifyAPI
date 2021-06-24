using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.ViewModels
{
    public class ForgetPasswordModel
    {
        [Required]
        public string Email { get; set; }
    }
}

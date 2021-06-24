using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.ViewModels
{
    public class ChangePasswordViewModel
    {



        [Required]
        [StringLength(25,MinimumLength =8)]
        public string NewPassword { get; set; }


        [Required]
        [Compare("NewPassword")]
        public string CPassword { get; set; }


    }
}

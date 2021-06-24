using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Models
{
    public class ApplicationUser: IdentityUser
    {


        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Fname { get; set; }



        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Lname { get; set; }



        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Address { get; set; }


        [DefaultValue(true)]
        public bool AdminLocked { get; set; }


        [StringLength(maximumLength:6,MinimumLength =1)]
        public string Gender { get; set; }


        [Range(18,100,ErrorMessage ="you should more the or equal 18")]
        public byte Age { get; set; }


        

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.ViewModels
{
    public class RegisterSellerModel:RegisterModel
    {
        [Required]
        [RegularExpression("^01[0125][0-9]{8}$")]
        public string  PhoneNumber { get; set; }


        [Required]
        [StringLength(50,MinimumLength =5)]
        public string StoreName { get; set; }



    }
}

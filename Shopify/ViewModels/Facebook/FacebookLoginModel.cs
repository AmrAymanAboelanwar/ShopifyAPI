using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.ViewModels.Facebook
{
    public class FacebookLoginModel
    {
        [Required]
        [StringLength(500,MinimumLength =50)]
        public string accessToken { get; set; }
    }
}

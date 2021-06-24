using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.ViewModels
{
    public class EditAddress
    {
        [Required]
        public string Address { get; set; }
    }
}

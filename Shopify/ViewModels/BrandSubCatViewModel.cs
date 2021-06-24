using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.ViewModels
{
    public class BrandSubCatViewModel
    {
        [Required]
        public int BrandId { get; set; }
        [Required]
        public int SubCatId { get; set; }
    }
}

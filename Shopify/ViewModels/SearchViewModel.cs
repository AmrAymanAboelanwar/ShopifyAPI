using Shopify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.ViewModels
{
    public class SearchViewModel
    {

        public List<Product> products { get; set; }
        public List<Brand> Brands { get; set; }
        public List<SubCategory>  subCategories { get; set; }
    }
}

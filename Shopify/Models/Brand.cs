using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Models
{
    public class Brand
    {
        public int BrandId { get; set; }

        [Required]
        [StringLength(50,MinimumLength =2)]
        public string BrandName { get; set; }
        [DefaultValue(false)]
        public bool Isdeleted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }


        public  virtual List<Product> Products { get; set; }
        public  virtual List<SubCategory> SubCategories { get; set; }
       

    }
}

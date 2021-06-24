using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Models
{
    public class SubCategory
    {

        public int SubCategoryId { get; set; }

        [Required]
        [StringLength(100,MinimumLength =3)]
        public string Name { get; set; }
        public string Image { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("Category")]
        public int ?CategoryId { get; set; }

        [DefaultValue(false)]
        public bool Isdeleted { get; set; }

        public virtual Category Category { get; set; }
        public virtual List<Product> Products { get; set; }
        public virtual List<Brand> Brands { get; set; }
        
    }
}

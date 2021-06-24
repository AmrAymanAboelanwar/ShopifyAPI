using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Models
{
    public class Inventory
    {
        public int InventoryId { get; set; }
        // citty street bu
        [Required]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string BuildingNumber { get; set; }

        [ForeignKey("Seller")]
        public string sellerId { get; set; }
        [DefaultValue(false)]
        public bool Isdeleted { get; set; }
        public virtual Seller Seller { get; set; }
        public virtual List<InventoryProduct> InventoryProducts { get; set; }



    }
}

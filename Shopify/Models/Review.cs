using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Models
{
    public class Review
    {

        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [Range(minimum:0,maximum:5)]
        public double? review { get; set; }
        public string comment { get; set; }
        public int ProductId { get; set; }
        [DefaultValue(false)]
        public bool Isdeleted { get; set; }
        public virtual Product Product { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Models
{
    public class View
    {
        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }


        [DefaultValue(false)]
        public bool IsFav { get; set; }
    }
}

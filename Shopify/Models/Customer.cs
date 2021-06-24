using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Models
{
    public class Customer
    {

        [Key, ForeignKey("ApplicationUser")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string CustomerId { get; set; }
        [DefaultValue(false)]
        public bool Isdeleted { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public  virtual List<View>  Views{ get; set; }
        public  virtual List<Review> Reviews{ get; set; }
        public  virtual List<Cart> Carts{ get; set; }





    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Models
{
    public class Payment
    {

        public int PaymentId { get; set; }
        public string type { get; set; }
        public double amount { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Cart Cart { get; set; }
    }
}

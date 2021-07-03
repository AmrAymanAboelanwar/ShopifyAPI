using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.ViewModels
{
    public class SalaryModelView
    {
        [Required]
        [Range(minimum:10,maximum:100000)]
        public float Salary { get; set; }
    }
}

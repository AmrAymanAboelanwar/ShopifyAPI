using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.ViewModels
{
    public class RegisterEmployeeViewModel:RegisterModel
    {
        public DateTime hireDate { get; set; } = DateTime.Now;

        [Required]
        public float Salary { get; set; }
    }
}

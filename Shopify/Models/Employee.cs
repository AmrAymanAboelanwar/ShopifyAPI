using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Models
{
    public class Employee
    {


        [Key, ForeignKey("ApplicationUser")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string EmployeeId { get; set; }
        [DefaultValue(false)]
        public bool Isdeleted { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public DateTime hireDate { get; set; } = DateTime.Now;

        [Required]
        public float Salary { get; set; }

    }
}

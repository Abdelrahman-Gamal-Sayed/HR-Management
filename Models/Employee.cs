using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Temp.Models
{
    public class Employee
    {
   // [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exeed 50 char")]
        public string Name { get; set; }

        public string Email { get; set; }

        public string PhotoPath { get; set; }

        public Dept? Department { get; set; }

    }
}

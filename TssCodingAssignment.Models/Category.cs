using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TssCodingAssignment.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name="Category Name")]
        [MaxLength(50), MinLength(5)]
        public string Name { get; set; }
    }
}

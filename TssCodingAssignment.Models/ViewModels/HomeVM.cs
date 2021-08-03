using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TssCodingAssignment.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Product> ProductList { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }

        [FromQuery]
        public string OrderBy { get; set; }
        [FromQuery]
        public string CategoryId { get; set; }
    }
}

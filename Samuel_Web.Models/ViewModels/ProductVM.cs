using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace Samuel_Web.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; } = null!;
        [ValidateNever]
        public IEnumerable<SelectListItem>   CategoryList { get; set; } = null!;
        [ValidateNever]
        [Display(Name = "Poduct Image")]
        public IFormFile ? Image { get; set; } = null!;       

    }
}

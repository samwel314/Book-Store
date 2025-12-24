using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Samuel_Web.Models  
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public string ISBN { get; set; } = null!;
        [Required]
        public string Author { get; set; } = null!;
        [Display(Name = "List Price")]
        [Required]
        [Range (1 , 1000)]
        public double ListPrice { get; set; } // the price of one book
        [Display(Name = "Price for 1-50")]
        [Required]
        [Range(1, 1000)]
        public double Price { get; set; }
        [Display(Name = "Price for 50+")]
        [Required]
        [Range(1, 1000)]
        public double Price50 { get; set; }
        [Display(Name = "Price for 100+")]
        [Required]
        [Range(1, 1000)]
        public double Price100 { get; set; } 

        public string ? ImageUrl { get; set; }
        [Display(Name = "Chose Category")]
        public int categoryId { get; set; } // foreign key  
        [ForeignKey("categoryId")]
        [ValidateNever]

        public Category  Category { get; set; } // navigation property
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Samuel_Web.Models
{
    public class ShoppingCart
    {
        [Key]
        [BindNever]
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ValidateNever]
        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;
        public string ApplicationUserId { get; set; } = null!;
        [ValidateNever]
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; } = null!;
        [Range(1, 1000, ErrorMessage = "Count Muste Be Between 1 - 1000")]
        public int Count { get; set; }      // means count of units from product 

        [NotMapped]
        public double Price { get; set; }
    }

}

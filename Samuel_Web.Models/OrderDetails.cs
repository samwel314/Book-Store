using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Samuel_Web.Models  
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; } 

        public int OrderHeaderId { get; set; }
        [ValidateNever]
        [ForeignKey ("OrderHeaderId")]
        public OrderHeader OrderHeader { get; set; } = null!;

        public int ProductId { get; set; }
        [ValidateNever]
        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        public int Count { get; set; }
        public double Price { get; set; } // the price of this product in this time 
    }
}

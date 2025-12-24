using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Samuel_Web.Models  
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = null!;
        [ValidateNever]
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; } = null!; 
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public double OrderTotal { get; set; }
        public string ? OrderStatus { get; set; }   
        public string? PaymentStatus { get;set; }
        public string ? TrackingNumebr { get; set; }
        public string ? Carrier { get; set; }  

        public DateTime PaymentDate { get; set; }   

        public DateOnly PaymentDueDate { get; set; } 
        public string? SessionId { get; set; }
        public string ? PaymentIntendId {  get; set; }

        [Required]
        public string Name { get; set; } = null!;
        [Required]

        public string StreetAddress { get; set; } = null!;
        [Required]
        public string City { get; set; } = null!; 
        [Required]
        public string State { get; set; } = null!;
        [Required]
        public string PostalCode { get; set; } = null!;
        [Required]
        public string PhoneNumber { get; set; } = null!;

    }
}

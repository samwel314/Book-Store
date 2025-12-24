using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Samuel_Web.Models  
{
    // her we extend IdentityUser whiche means we add more props in main table 
    public class ApplicationUser : IdentityUser
    {
        [Required]

        // ef core will create this column is null because it is not based on IdentityUser
        public string Name { get; set; } = null!;
        // means this props not Required in register process 
        public string ? StreetAddress { get; set; }
        public string ? City { get; set; }
        public string ? State { get; set; }
        public string ? PostalCode { get; set; }

        public int ? CompanyId { get; set; }
        [ValidateNever]
        [ForeignKey("CompanyId")]
        public Company Company { get; set; } = null!;  
        
    }
}

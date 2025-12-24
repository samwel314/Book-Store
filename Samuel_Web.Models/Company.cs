using System.ComponentModel.DataAnnotations;

namespace Samuel_Web.Models  
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        // means this props not Required in register process 
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        public string ? PhoneNumber { get; set; }   
    }
}

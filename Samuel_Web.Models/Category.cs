using System.ComponentModel.DataAnnotations;

namespace Samuel_Web.Models  
{
    public class Category
    {
        //we not need [key] her because ef core by default consider Id or <ClassName>Id as primary key
        [Key]  // ef core will consider this as primary key or can be CategoryId
        public int Id { get; set; }
        [MaxLength(8, ErrorMessage = "Category Name Must Be less Than Or Equel 8")]
        [MinLength(1)]
        [Display (Name = "Category Name")]
        [Required (ErrorMessage = "The Category Name Is Mandatory")] // makes the field mandatory  can not be null
        public string Name { get; set; } = null!;

        [Display(Name = "Display Order")]
        [Required(ErrorMessage = "The Display Order Is Mandatory")] // makes the field mandatory  can not be null
        [Range(1 , 100  , ErrorMessage = "Display Order Range Between 1 : 100 ")]
        public int DisplayOrder { get; set; } 

    }
}

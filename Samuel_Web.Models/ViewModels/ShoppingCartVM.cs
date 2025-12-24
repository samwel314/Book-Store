using Microsoft.AspNetCore.Identity;

namespace Samuel_Web.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable <ShoppingCart> ShoppingCartList { get; set; } = null!;
        public OrderHeader OrderHeader { get; set; } = null!;        

    }
}

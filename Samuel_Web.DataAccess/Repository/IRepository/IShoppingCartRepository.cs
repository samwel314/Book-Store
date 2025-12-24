using Samuel_Web.Models;

namespace Samuel_Web.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        void Update(ShoppingCart ShoppingCart);

    }


}

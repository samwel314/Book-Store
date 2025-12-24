using Samuel_Web.Models;

namespace Samuel_Web.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product category);

    }


}

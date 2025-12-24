using Samuel_Web.DataAccess.Data;
using Samuel_Web.Models;
using System.Linq.Expressions;

namespace Samuel_Web.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>    
    {
        void Update(Category category);

    }


}

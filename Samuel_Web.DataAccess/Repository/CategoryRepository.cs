using Samuel_Web.DataAccess.Data;
using Samuel_Web.DataAccess.Repository.IRepository;
using Samuel_Web.Models;
using System.Runtime.InteropServices;
namespace Samuel_Web.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _db;
        public CategoryRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
  
       public void Update(Category category)
        {
            _db.Categories.Update(category);
        }
    }
}

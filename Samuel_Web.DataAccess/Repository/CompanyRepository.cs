using Samuel_Web.DataAccess.Data;
using Samuel_Web.DataAccess.Repository.IRepository;
using Samuel_Web.Models;
namespace Samuel_Web.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly AppDbContext _db;
        public CompanyRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Company company )
        {
            _db.Companies.Update(company);
        }
    }
}

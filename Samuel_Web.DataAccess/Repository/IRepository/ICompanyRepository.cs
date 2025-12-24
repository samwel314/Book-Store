using Samuel_Web.Models;

namespace Samuel_Web.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company company);

    }

}

using Samuel_Web.Models;

namespace Samuel_Web.DataAccess.Repository.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail orderDetail);

    }

}

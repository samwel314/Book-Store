using Samuel_Web.DataAccess.Data;
using Samuel_Web.DataAccess.Repository.IRepository;
using Samuel_Web.Models;
namespace Samuel_Web.DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly AppDbContext _db;
        public OrderDetailRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderDetail orderDetail)
        {
            _db.OrderDetails.Update(orderDetail);
        }
    }
}

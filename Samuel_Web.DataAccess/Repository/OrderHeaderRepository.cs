using Samuel_Web.DataAccess.Data;
using Samuel_Web.DataAccess.Repository.IRepository;
using Samuel_Web.Models;
namespace Samuel_Web.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>  ,  IOrderHeaderRepository
    {
        private readonly AppDbContext _db;
        public OrderHeaderRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader  order)
        {
            _db.OrderHeaders.Update(order);
        }

        public void UpdateStatus(int id, string orderStatus, string paymentStatus = null)
        {
           var orderFromDB =   _db.OrderHeaders.FirstOrDefault(x => x.Id == id);

            if (orderFromDB != null)
            {
                orderFromDB.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDB.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string PaymentIntendId)
        {
            var orderFromDB = _db.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderFromDB!.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(PaymentIntendId))
            {
                orderFromDB!.PaymentIntendId = PaymentIntendId;
                orderFromDB!.PaymentDate = DateTime.Now;    
            }

        }
    }
}

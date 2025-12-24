using Samuel_Web.Models;

namespace Samuel_Web.DataAccess.Repository.IRepository
{
    public interface    IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader order);

        void UpdateStatus (int id , string orderStatus , string paymentStatus = null!);
        void UpdateStripePaymentId (int id , string sessionId , string PaymentIntendId);

    }

}

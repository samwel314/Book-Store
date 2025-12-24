using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Samuel_Web.DataAccess.Repository.IRepository;
using Samuel_Web.Models;
using Samuel_Web.Models.ViewModels;
using Samuel_Web.Utility;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Threading.Tasks;

namespace Samuel_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _db;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderController(IUnitOfWork db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        [Authorize]

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {

            OrderVM orderVM = new OrderVM()
            {
                OrderDetails = _db.OrderDetail.GetAll(includepro: "Product", filter: Dt => Dt.OrderHeaderId == id),
                OrderHeader = _db.OrderHeader.GetFirst(Oh => Oh.Id == id, "ApplicationUser")
            };

            return View(orderVM);
        }
        public async Task<IActionResult> GetAll(string status)
        {
            IEnumerable<OrderHeader> orderheaderList; 

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderheaderList = _db.OrderHeader.GetAll(includepro: "ApplicationUser" );
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                orderheaderList = _db.OrderHeader
                    .GetAll(includepro: "ApplicationUser" , or => or.ApplicationUserId == user!.Id);
            }



            switch (status)
            {
                case "pending":
                    orderheaderList = orderheaderList.Where(x => x.PaymentStatus == SD.PaymentStatusPending);
                    break;
                case "approved":
                    orderheaderList = orderheaderList.Where(x => x.OrderStatus == SD.StatusApproved);
                    break;
                case "inprocess":
                    orderheaderList = orderheaderList.Where(x => x.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderheaderList = orderheaderList.Where(x => x.OrderStatus == SD.StatusShipped);
                    break;
                default:
                    break;
            }
            return Json(new
            {
                data = orderheaderList,

            });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrder(OrderVM order)
        {

            var OrderHeader = _db.OrderHeader.GetFirst(Oh => Oh.Id == order.OrderHeader.Id);

            OrderHeader.Name = order.OrderHeader.Name;      
            OrderHeader.PhoneNumber = order.OrderHeader.PhoneNumber;    
            OrderHeader.State = order.OrderHeader.State;
            OrderHeader.PostalCode = order.OrderHeader.PostalCode;
            OrderHeader.City = order.OrderHeader.City;
            OrderHeader.StreetAddress = order.OrderHeader.StreetAddress;

            if (!string.IsNullOrEmpty(order.OrderHeader.Carrier))
                OrderHeader.Carrier = order.OrderHeader.Carrier;

            if (!string.IsNullOrEmpty(order.OrderHeader.TrackingNumebr))
                OrderHeader.TrackingNumebr = order.OrderHeader.TrackingNumebr;

            _db.OrderHeader.Update(OrderHeader);
            _db.Save();
            TempData["message"] = "Order Details Updated successfully";

            return RedirectToAction("Details" , new
            {
                id  = OrderHeader.Id
            });
        }
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

        public IActionResult StartProcessing (int id )
        {
            _db.OrderHeader.UpdateStatus(id, SD.StatusInProcess);
         


            _db.Save();

            TempData["message"] = "Order Details Updated successfully";

            return RedirectToAction("Details", new
            {
                id = id
            });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

        public IActionResult ShippingOrder(OrderVM order)
        {

            var orderfromDB = _db.OrderHeader.GetFirst(or => or.Id == order.OrderHeader.Id);
            orderfromDB.Carrier = order.OrderHeader.Carrier;
            orderfromDB.TrackingNumebr = order.OrderHeader.TrackingNumebr; 
            orderfromDB.ShippingDate = DateTime.Now;    
            orderfromDB.OrderStatus = SD.StatusShipped;

            if (orderfromDB.PaymentStatus == SD.PaymentStatusDelayedPayment)
                orderfromDB.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
        
            
            _db.OrderHeader.Update(orderfromDB);
            _db.Save(); 

            TempData["message"] = "Order Shipped  successfully";

            return RedirectToAction("Details", new
            {
                id = order.OrderHeader.Id
            });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder(OrderVM order)
        {

            var orderfromDB = _db.OrderHeader.GetFirst(or => or.Id == order.OrderHeader.Id);
  
            if (orderfromDB.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer , 
                    PaymentIntent = orderfromDB.PaymentIntendId
                };
                var service = new RefundService();
                service.Create(options);

                _db.OrderHeader.UpdateStatus(orderfromDB.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                _db.OrderHeader.UpdateStatus(orderfromDB.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            _db.Save(); 

            TempData["message"] = "Order Cancelled successfully";

            return RedirectToAction("Details", new
            {
                id = orderfromDB.Id
            });
        }

        [HttpPost]
        public IActionResult PayNow(OrderVM order)
        {

            order.OrderDetails = _db.OrderDetail.GetAll(includepro: "Product", filter: Dt => Dt.OrderHeaderId == order.OrderHeader.Id);
            order.OrderHeader = _db.OrderHeader.GetFirst(Oh => Oh.Id == order.OrderHeader.Id, "ApplicationUser");

                var Domain = "https://localhost:7162";
                /// stripe logic 
                var options = new Stripe.Checkout.SessionCreateOptions
                {

                    SuccessUrl = Domain + $"/Admin/Order/OrderConfirmation?id={order.OrderHeader.Id}",
                    CancelUrl = Domain + $"/Admin/Order/details?id={order.OrderHeader.Id}",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>()
                   ,
                    Mode = "payment",
                };


            foreach (var item in order.OrderDetails)
            {
                options.LineItems.Add(new Stripe.Checkout.SessionLineItemOptions()
                {
                    PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = item.Product.Title
                        }
                    }
                    ,
                    Quantity = item.Count
                });
            }

                var service = new Stripe.Checkout.SessionService();

                Stripe.Checkout.Session session = service.Create(options);                     // PaymentIntentId = null because the payment not done yet
                _db.OrderHeader.UpdateStripePaymentId(order.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _db.Save();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            
        }

        public IActionResult OrderConfirmation(int id)
        {
            var orderHeader = _db.OrderHeader.
                GetFirst(u => u.Id == id);
         
                var sessionService = new SessionService();
                Session session = sessionService.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _db.OrderHeader.UpdateStripePaymentId(orderHeader.Id, session.Id, session.PaymentIntentId);
                    _db.OrderHeader.UpdateStatus(id, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                _db.Save();    
            }

    
            return View(id);
        }


    }
}

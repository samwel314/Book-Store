using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Samuel_Web.DataAccess.Repository.IRepository;
using Samuel_Web.Models;
using Samuel_Web.Models.ViewModels;
using Samuel_Web.Utility;
using Stripe;
using Stripe.Checkout;
// to handel format ctrl + a   : ctrl + k + d
namespace Samuel_Web.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class CartController : Controller
    {

        private readonly IUnitOfWork _db;
        private readonly UserManager<IdentityUser> _userManager;
        public CartController(IUnitOfWork db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();
            // we need to git Cart items for this user 
            ShoppingCartVM CartFromDB = new ShoppingCartVM()
            {
                ShoppingCartList = _db.ShoppingCart.GetAll
                (includepro: "Product", c => c.ApplicationUserId == user.Id),
                OrderHeader = new OrderHeader()
            };
            foreach (var item in CartFromDB.ShoppingCartList)
            {
                item.Price = GetTotalForItemCart(item);
                CartFromDB.OrderHeader.OrderTotal += item.Count * item.Price;
            }
            return View(CartFromDB);
        }

        public IActionResult Plus(int id)
        {
            var CartItemFromdDB = _db.ShoppingCart.GetFirst(cat => cat.Id == id, tracked: true);
            if (CartItemFromdDB == null)
                return NotFound();
            CartItemFromdDB.Count += 1;
            _db.Save();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Mines(int id)
        {
            var CartItemFromdDB = _db.ShoppingCart.GetFirst(cat => cat.Id == id, tracked: true);
            if (CartItemFromdDB == null)
                return NotFound();
            if (CartItemFromdDB.Count == 1)
            {
                // means he not need this item
                // 
                var user = await _userManager.GetUserAsync(User);

                _db.ShoppingCart.Remove(CartItemFromdDB);


            }
            else
            {
                CartItemFromdDB.Count -= 1;
            }
            _db.Save();
            return RedirectToAction("Index");
        }
        public async Task< IActionResult> Delete(int id)
        {
            var CartItemFromdDB = _db.ShoppingCart.GetFirst(cat => cat.Id == id);
            if (CartItemFromdDB == null)
                return NotFound();
            var user = await _userManager.GetUserAsync(User);

            _db.ShoppingCart.Remove(CartItemFromdDB);
            _db.Save();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Summary()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();
            // we need to git Cart items for this user 
            ShoppingCartVM CartFromDB = new ShoppingCartVM()
            {
                ShoppingCartList = _db.ShoppingCart.GetAll
                (includepro: "Product", c => c.ApplicationUserId == user.Id),
                OrderHeader = new OrderHeader(),
            };
            if (CartFromDB.ShoppingCartList.Count() == 0)
            {
                TempData["message"] = "Add Items To Cart ";
                return RedirectToAction("Index", "Home");

            }
            CartFromDB.OrderHeader.ApplicationUser =
                _db.ApplicationUser.GetFirst(u => u.Id == user.Id);
            CartFromDB.OrderHeader.State = CartFromDB.OrderHeader.ApplicationUser.State!;
            CartFromDB.OrderHeader.PostalCode = CartFromDB.OrderHeader.ApplicationUser.PostalCode!;
            CartFromDB.OrderHeader.Name = CartFromDB.OrderHeader.ApplicationUser.Name!; CartFromDB.OrderHeader.State = CartFromDB.OrderHeader.ApplicationUser.State!;
            CartFromDB.OrderHeader.PhoneNumber = CartFromDB.OrderHeader.ApplicationUser.PhoneNumber!; CartFromDB.OrderHeader.PhoneNumber = CartFromDB.OrderHeader.ApplicationUser.PhoneNumber!;
            CartFromDB.OrderHeader.City = CartFromDB.OrderHeader.ApplicationUser.City!;
            CartFromDB.OrderHeader.StreetAddress = CartFromDB.OrderHeader.ApplicationUser.StreetAddress!;

            foreach (var item in CartFromDB.ShoppingCartList)
            {
                item.Price = GetTotalForItemCart(item);
                CartFromDB.OrderHeader.OrderTotal += item.Count * item.Price;
            }
            return View(CartFromDB);
        }
        [HttpPost]
        public async Task<IActionResult> Summary(ShoppingCartVM cartVM)
        {
            // get user 
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();
            // work with order header 

            cartVM.OrderHeader.ApplicationUserId = user.Id;
            cartVM.OrderHeader.OrderDate = DateTime.Now;

            var cartFromDB =
               _db.ShoppingCart.
               GetAll(includepro: "Product", c => c.ApplicationUserId == user.Id);
            
            foreach (var item in cartFromDB)
            {
                item.Price = GetTotalForItemCart(item);
                cartVM.OrderHeader.OrderTotal += item.Count * item.Price;
            }

            var userFromDB = _db.ApplicationUser.GetFirst(u => u.Id == user.Id);
            if (userFromDB.CompanyId.GetValueOrDefault() == 0)
            {
                // this customer 
                cartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                cartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                // this company 
                cartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                cartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            //*-------------------*-----------------*------------9***
            // add order to db 

            _db.OrderHeader.Add(cartVM.OrderHeader);
            _db.Save(); // why we call save() her to get orderheader id

            var OrderDetails = cartFromDB.Select(ord => new OrderDetail()
            {
                OrderHeaderId = cartVM.OrderHeader.Id,
                Price = ord.Price,
                Count = ord.Count,
                ProductId = ord.ProductId,
                Product = ord.Product,  
            });

            _db.OrderDetail.AddRange(OrderDetails);
            _db.Save();
            if (userFromDB.CompanyId.GetValueOrDefault() == 0)
            {
                var Domain = "https://localhost:7162"; 
                /// stripe logic 
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    
                    SuccessUrl = Domain + $"/Customer/Cart/OrderConfirmation?id={cartVM.OrderHeader.Id}",
                    CancelUrl = Domain + "/Customer/cart/index",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>()
                   ,
                    Mode = "payment",
                };

               
                foreach (var item in OrderDetails)
                {
                    options.LineItems.Add(new Stripe.Checkout.SessionLineItemOptions()
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long) (item.Price *100 ) , 
                            Currency ="usd" , 
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = item.Product.Title
                            }
                        }
                        ,Quantity = item.Count
                    });
                }
                var service = new Stripe.Checkout.SessionService();
               
                Stripe.Checkout.Session session = service.Create(options);                     // PaymentIntentId = null because the payment not done yet
                _db.OrderHeader.UpdateStripePaymentId(cartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _db.Save();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303); 
            }
            


            return RedirectToAction("OrderConfirmation", new { id = cartVM.OrderHeader.Id });

        }
        public IActionResult OrderConfirmation(int id)
        {
            var orderHeader = _db.OrderHeader.
                GetFirst(u=> u.Id == id);
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var sessionService = new SessionService();
                Session session = sessionService.Get(orderHeader.SessionId); 
           
                if (session.PaymentStatus.ToLower() == "paid" )
                {
                    _db.OrderHeader.UpdateStripePaymentId(orderHeader.Id, session.Id, session.PaymentIntentId);
                    _db.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
              
                    HttpContext.Session.Clear(); 
                }
                // THE CUSTOMER NOT NEED THIS ITEMS IN HIS CART 
                // BECAUSE HE MAY PAID OR MAKE SOME THING WRONG WHEN HE DO THE PAYMENT 
  
            }

            var ShoppingCart = _db.ShoppingCart.GetAll(filter: shv => shv.ApplicationUserId == orderHeader.ApplicationUserId);
            _db.ShoppingCart.RemoveRange(ShoppingCart);
            _db.Save();

            return View(id);
        }
        private double GetTotalForItemCart(ShoppingCart cart)
        {

            if (cart.Count <= 50)
                return cart.Product.Price;
            else if (cart.Count <= 100)
                return cart.Product.Price50;
            else
                return cart.Product.Price100;

        }
    }
}

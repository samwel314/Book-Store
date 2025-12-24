using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Samuel_Web.DataAccess.Repository.IRepository;
using Samuel_Web.Models;
using Samuel_Web.Utility;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Samuel_Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _db;
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(IUnitOfWork db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
          
            var productList = _db.Product.GetAll(includepro: "Category");
            return View(productList);
        }

        public IActionResult Details(int id)
        {
            // why using Shopping Cart Model not Product ? 
            // because i need it on post when user need to add this product in cart 
            ShoppingCart cart = new ShoppingCart()
            {
                Id = 0,
                Product = _db.Product.GetFirst(prod => prod.Id == id, includepro: "Category"),
                Count = 1,
                ProductId = id
            };

            if (cart.Product == null)
                return NotFound();
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart cart)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();
            // check if the user have this product in his cart and need to update count 
            var cartFromDB = _db.ShoppingCart.GetFirst(ca => ca.ProductId == cart.ProductId && ca.ApplicationUserId == user.Id, tracked: true);

            if (cartFromDB == null)
            {
                cartFromDB = new ShoppingCart()
                {
                    Id = 0,
                    Count = cart.Count,
                    ProductId = cart.ProductId,
                    ApplicationUserId = user.Id
                };
                _db.ShoppingCart.Add(cartFromDB);
                _db.Save();
          
            }
            else
            {
                cartFromDB.Count += cart.Count;
                _db.Save();
            }

            TempData["message"] = "Cart updated  successfully";

            return RedirectToAction("Index");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

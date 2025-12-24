using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Samuel_Web.DataAccess.Repository.IRepository;
using Samuel_Web.Utility;

namespace Samuel_Web.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _db;
        private readonly UserManager<IdentityUser> _userManager;

        public ShoppingCartViewComponent(IUnitOfWork db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var user = await _userManager.GetUserAsync(UserClaimsPrincipal);
            if (user != null)   
            {
                HttpContext.
                      Session.SetInt32(SD.SessionCart, _db.ShoppingCart.GetAll(includepro: null!, ca => ca.ApplicationUserId == user.Id).Count());
            
               return View(HttpContext.
                      Session.GetInt32(SD.SessionCart));   
            }
            else
            {
                
                HttpContext.Session.Clear();
                return View(0);
            }

        }
    }
}

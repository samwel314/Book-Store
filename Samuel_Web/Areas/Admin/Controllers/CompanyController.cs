using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Samuel_Web.DataAccess.Data;
using Samuel_Web.DataAccess.Repository.IRepository;
using Samuel_Web.Models;
using Samuel_Web.Utility;

namespace Samuel_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        // wy readonly ? to ensure that the _db field is only assigned once, in the constructor, and cannot be modified afterwards. This helps to maintain the integrity of the database context throughout the lifetime of the controller instance.

        // before using repository pattern
        // private readonly AppDbContext _db;  
        // after using repository pattern
        // private readonly ICategoryRepository _categoryRepository;

        // after adding uintofwork 
        private readonly IUnitOfWork _db;
        public CompanyController(IUnitOfWork db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var companiesList = _db.Company.GetAll();    
            return View(companiesList);
        }

        public IActionResult Create ()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Company company )
        {
                
            if (!ModelState.IsValid )
            {
                return View();
            }
            _db.Company.Add(company);
            _db.Save();
            TempData["message"] = "Company created successfully";

            return RedirectToAction("Index");
        }

        public IActionResult Edit (int id)
        {
            var ComapnyFromDb = _db.Company.GetFirst(cat=> cat.Id ==  id);  

            if (ComapnyFromDb == null)
            {
                return NotFound();    
            }
            return View(ComapnyFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Company company)
        {
            
            if (!ModelState.IsValid)
            {
                return View(company);
            }
            _db.Company.Update(company);
            _db.Save();
            TempData["message"] = "Company updated successfully";

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var ComapnyFromDb = _db.Company.GetFirst(cat => cat.Id == id);

            if (ComapnyFromDb == null)
            {
                return NotFound();
            }
            return View(ComapnyFromDb);
        }
        [HttpPost , ActionName("Delete")]
        public IActionResult DeletePost(int id )
        {
            var ComapnyFromDb = _db.Company.GetFirst(cat => cat.Id == id);

            if (ComapnyFromDb == null )
            {
                return NotFound();
            }
            _db.Company.Remove(ComapnyFromDb);
            _db.Save();

            TempData["message"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }


    }
}

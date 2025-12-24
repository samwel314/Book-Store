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
    public class CategoryController : Controller
    {
        // wy readonly ? to ensure that the _db field is only assigned once, in the constructor, and cannot be modified afterwards. This helps to maintain the integrity of the database context throughout the lifetime of the controller instance.

        // before using repository pattern
        // private readonly AppDbContext _db;  
        // after using repository pattern
        // private readonly ICategoryRepository _categoryRepository;

        // after adding uintofwork 
        private readonly IUnitOfWork _db;
        public CategoryController(IUnitOfWork db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var CategoriesList = _db.Category.GetAll();    
            return View(CategoriesList);
        }

        public IActionResult Create ()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            
            if (category.Name == category.DisplayOrder.ToString() || category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("DisplayOrder", "The DisplayOrder cannot exactly match the Category Name.");    
            }
                if (!ModelState.IsValid )
            {
                return View();
            }
            _db.Category.Add(category);
            _db.Save();
            TempData["message"] = "Category created successfully";

            return RedirectToAction("Index");
        }

        public IActionResult Edit (int id)
        {
            var categoryFromDb = _db.Category.GetFirst(cat=> cat.Id ==  id);  

            if (categoryFromDb == null)
            {
                return NotFound();    
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {

            if (category.Name == category.DisplayOrder.ToString() || category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("DisplayOrder", "The DisplayOrder cannot exactly match the Category Name.");
            }
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            _db.Category.Update(category);
            _db.Save();
            TempData["message"] = "Category updated successfully";

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var categoryFromDb = _db.Category.GetFirst(cat => cat.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost , ActionName("Delete")]
        public IActionResult DeletePost(int id )
        {
            var categoryFromDb = _db.Category.GetFirst(cat => cat.Id == id);

            if (categoryFromDb == null )
            {
                return NotFound();
            }
            _db.Category.Remove(categoryFromDb);
            _db.Save();

            TempData["message"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }


    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using NuGet.Protocol.Plugins;
using Samuel_Web.DataAccess.Repository;
using Samuel_Web.DataAccess.Repository.IRepository;
using Samuel_Web.Models;
using Samuel_Web.Models.ViewModels;
using Samuel_Web.Utility;
using Product = Samuel_Web.Models.Product;

namespace Samuel_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(IUnitOfWork db , IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            var productList = _db.Product.GetAll(includepro : "Category");
            return View(productList);
        }
        // this create method can be used for both create and edit based on the id parameter

        public IActionResult Upsert(int ? id )
        {
            var ObjToview = new ProductVM(); 
            // create new product
            if (id == 0 || id == null)
            {
                ObjToview.Product = new Product();  
            }
            else
            {
                // update product 
                var productFromDb = _db.Product.GetFirst(prod => prod.Id == id);
                if (productFromDb == null)
                    return NotFound();
                ObjToview.Product = productFromDb;
            }
            ObjToview.CategoryList = _db.Category.GetAll().Select(cat => new SelectListItem
            {
                Text = cat.Name,
                Value = cat.Id.ToString()
            });

            return View(ObjToview);
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM BindObj)
        {
            // validate model state and image upload for new product
            if (BindObj.Image == null && BindObj.Product.Id == 0)
            {
                ModelState.AddModelError("Image", "Please upload an image.");
            }
            if (!ModelState.IsValid )
            {
                BindObj.CategoryList = _db.Category.GetAll().Select(cat => new SelectListItem
                {
                    Text = cat.Name,
                    Value = cat.Id.ToString()
                }); 
                return View(BindObj);
            }

            // work on image upload  if creating new product or updating image for existing product
            Product product; 
            if (BindObj.Product.Id != 0)
            { // update existing product 
                product = _db.Product.GetFirst(prod => prod.Id == BindObj.Product.Id);
            }
            else
            { // create new product
                product = new Product();
            }
            // her user uploads a new image may update the image or create new product
            if (BindObj.Image != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(BindObj.Image.FileName);
                var uploads = Path.Combine(wwwRootPath, @"images/products");
                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    BindObj.Image.CopyTo(fileStream);
                }

                // Delete old image file if exists
                if (product.Id != 0)
                {
                    // delete the old image file
                    var oldImagePath = Path.Combine(wwwRootPath, product.ImageUrl!.TrimStart('/').ToString());
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                product.ImageUrl = @"/images/products/" + fileName;
            }
            // map data from BindObj to product entity
            product.ISBN = BindObj.Product.ISBN;    
            product.Author = BindObj.Product.Author;
            product.ListPrice = BindObj.Product.ListPrice;
            product.Price = BindObj.Product.Price;
            product.Price50 = BindObj.Product.Price50;
            product.Price100 = BindObj.Product.Price100;
            product.Title = BindObj.Product.Title;
            product.Description = BindObj.Product.Description;
            product.categoryId = BindObj.Product.categoryId;
            if (BindObj.Product.Id != 0 )
            {
                // update product 
                _db.Product.Update(product);
                TempData["message"] = "Product updated successfully";
            }
            else
            {
                // create product
                _db.Product.Add(product);
                TempData["message"] = "Product created successfully";
            }

            _db.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Delete (int Id )
        {
            var productFromDb = _db.Product.GetFirst(prod => prod.Id == Id);
            if (productFromDb == null)
                return NotFound();

            return View(productFromDb);

        }
        [HttpPost  , ActionName( "Delete")]
        public IActionResult DeletePost(int Id)
        {
            var productFromDb = _db.Product.GetFirst(prod => prod.Id == Id);
            if (productFromDb == null)
                return NotFound();
            _db.Product.Remove(productFromDb);
            _db.Save();
            string wwwRootPath = _hostEnvironment.WebRootPath;  
            var oldImagePath = Path.Combine(wwwRootPath, productFromDb.ImageUrl!.TrimStart('/').ToString());
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            TempData["message"] = "Product deleted successfully";
            return RedirectToAction("Index");

        }



        public IActionResult GetAll()
        {
            var productList = _db.Product.GetAll(includepro: "Category");
            return Json(new
            {
                data = productList , 

            });
        }


        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product = _db.
                         Product.GetFirst(c => c.Id == id);
            if (product == null)
            {
                return
              Json(new { success = false, message = "Error While Deleting" });
            }

            string finalpath = Path.Combine(_hostEnvironment.WebRootPath, product.ImageUrl!.TrimStart('/').ToString());
            if (System.IO.File.Exists(finalpath))
            {
                System.IO.File.Delete(finalpath);
            }
        
            _db.Product.Remove(product);
            _db.Save();
            return
                Json(new { success = true, message = "Product was deleted" });

        }
    }


}

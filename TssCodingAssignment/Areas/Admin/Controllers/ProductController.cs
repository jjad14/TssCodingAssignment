using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TssCodingAssignment.DataAccess.Repository;
using TssCodingAssignment.DataAccess.Repository.IRepository;
using TssCodingAssignment.Models;
using TssCodingAssignment.Models.ViewModels;

namespace TssCodingAssignment.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        // uploading images on server inside wwwroot
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            // viewmodel containing a product and a list of selectlistItems (categories)
            ProductVM productVM = new ProductVM() 
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };

            // no id means its creating a product
            if (id == null)
            {
                return View(productVM);
            }

            // if id is found, then its an update
            productVM.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());

            if (productVM.Product == null)
            {
                return NotFound();
            }

            return View(productVM);
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            // get all products
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category");

            return Json(new { data = products });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product product)
        {
            // if model validation is valid
            if (ModelState.IsValid)
            {
                // no id means action was a create
                if (product.Id == 0)
                {
                    _unitOfWork.Product.Add(product);
                }
                else
                {
                    // id found, action was an update
                    _unitOfWork.Product.Update(product);
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            // get category by id
            var objectFromDb = _unitOfWork.Product.Get(id);

            // no category was found (id was not valid)
            if (objectFromDb == null)
            {
                return Json(new { success = false, message = "Unknown Product, Cannot Delete" });
            }

            // remove category and save changes
            _unitOfWork.Product.Remove(objectFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Product Deleted" });
        }

        #endregion
    }
}

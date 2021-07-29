using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
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
        public IActionResult Upsert(ProductVM productVM)
        {
            // if model validation is valid
            if (ModelState.IsValid)
            {
                // path to wwwroot folder
                string webRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                // if file count is greater than 0, then a file has been uploaded
                if (files.Count > 0) 
                {
                    string fileName = Guid.NewGuid().ToString();
                    // path to wwwroot/images/products
                    var uploads = Path.Combine(webRootPath, @"images\products");

                    var extension = Path.GetExtension(files[0].FileName);

                    // if imageUrl is not null, then its an edit action
                    if (productVM.Product.ImageUrl != null) 
                    {
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(imagePath)) 
                        {
                            // remove old image
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    // After the file is deleted we will have to upload the new file
                    using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    // update the product view model
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                else
                {
                    // update when the image is not changed
                    if (productVM.Product.Id != 0)
                    {
                        Product objFromDb = _unitOfWork.Product.Get(productVM.Product.Id);
                        productVM.Product.ImageUrl = objFromDb.ImageUrl;
                    }
                }

                // no id means action was a create
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    // id found, action was an update
                    _unitOfWork.Product.Update(productVM.Product);
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                // if model state is invalid, return fresh version of viewmodel to view
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

                if (productVM.Product.Id != 0) 
                {
                    productVM.Product = _unitOfWork.Product.Get(productVM.Product.Id);
                }
            }

            return View(productVM);
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

            // get the wwwroot path
            string webRootPath = _webHostEnvironment.WebRootPath;

            // find image path
            var imagePath = Path.Combine(webRootPath, objectFromDb.ImageUrl.TrimStart('\\'));

            // if image path exists then we have to remove that.
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            // remove category and save changes
            _unitOfWork.Product.Remove(objectFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Product Deleted" });
        }

        #endregion
    }
}

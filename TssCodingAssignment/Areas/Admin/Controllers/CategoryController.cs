using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TssCodingAssignment.DataAccess.Repository;
using TssCodingAssignment.DataAccess.Repository.IRepository;
using TssCodingAssignment.Models;

namespace TssCodingAssignment.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id) 
        {
            Category category = new Category();

            // no id means its creating a category
            if (id == null) 
            {
                return View(category);
            }

            // if id is found, then its an update
            category = _unitOfWork.Category.Get(id.GetValueOrDefault());

            if (category == null) 
            {
                return NotFound();
            }

            return View(category);
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll() 
        {
            // get all categories
            var categories = _unitOfWork.Category.GetAll();

            return Json(new { data = categories });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            // if model validation is valid
            if (ModelState.IsValid)
            {
                // no id means action was a create
                if (category.Id == 0)
                {
                    _unitOfWork.Category.Add(category);
                }
                else
                {
                    // id found, action was an update
                    _unitOfWork.Category.Update(category);
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        [HttpDelete]
        public IActionResult Delete(int id) 
        {
            // get category by id
            var objectFromDb = _unitOfWork.Category.Get(id);

            // no category was found (id was not valid)
            if (objectFromDb == null) 
            {
                return Json(new { success = false, message = "Unknown Category, Cannot Delete" });
            }

            // remove category and save changes
            _unitOfWork.Category.Remove(objectFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Category Deleted" });
        }

        #endregion
    }
}

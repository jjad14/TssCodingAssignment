using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TssCodingAssignment.DataAccess.Repository.IRepository;
using TssCodingAssignment.Models;
using TssCodingAssignment.Models.ViewModels;
using TssCodingAssignment.Utility;

namespace TssCodingAssignment.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        // action method
        public IActionResult Index()
        {
            // TODO: Needs to be a view model, with categories as selectlistitems, a search value and orderBy, then we bind the vm to the view
            // IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");

            HomeVM homeVM = new HomeVM()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                Search = "",
                OrderBy = "",
                ProductList = _unitOfWork.Product.GetAll(includeProperties: "Category")
            };

            // get the id of the currently logged in user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // user not authenticated
            if (claim != null) 
            {
                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == claim.Value)
                    .ToList().Count();

                // Add cart count to session
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);
            }

            return View(homeVM);
        }

        public IActionResult Details(int id) 
        {
            var productFromDb = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id, includeProperties: "Category");

            ShoppingCart cart = new ShoppingCart()
            {
                   Product = productFromDb,
                   ProductId = productFromDb.Id
            };

            return View(cart);

        }

        // action method
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region API Calls

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart CartObj)
        {
            // So that EF treats this as a new record and it creates it in database
            CartObj.Id = 0;

            // Add product to cart
            if (ModelState.IsValid)
            {
                // get the id of the currently logged in user
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                CartObj.ApplicationUserId = claim.Value;

                // retrieve the shopping cart from the database based on the user ID and product Id
                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == CartObj.ApplicationUserId &&
                    u.ProductId == CartObj.ProductId,
                    includeProperties: "Product"
                 );

                // no record exists in the database for that product, for this user
                if (cartFromDb == null)
                {
                    _unitOfWork.ShoppingCart.Add(CartObj);
                }
                else 
                {
                    // record exists, update quantity in Db
                    cartFromDb.Count += CartObj.Count;
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                }

                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == CartObj.ApplicationUserId)
                    .ToList().Count();

                // Add cart count to session
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);
                // HttpContext.Session.SetObject(SD.ssShoppingCart, count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                // invalid ModelState, return view
                var productFromDb = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == CartObj.Id, includeProperties: "Category");

                ShoppingCart cart = new ShoppingCart()
                {
                    Product = productFromDb,
                    ProductId = productFromDb.Id
                };

                return View(cart);
            }

        }

        #endregion
    }
}



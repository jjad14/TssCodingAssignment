using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TssCodingAssignment.DataAccess.Repository.IRepository;
using TssCodingAssignment.Models.ViewModels;
using TssCodingAssignment.Utility;

namespace TssCodingAssignment.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            // get the id of the currently logged in user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == claim.Value, includeProperties: "Product")
            };

            // set initial order total to 0
            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
                .GetFirstOrDefault(u => u.Id == claim.Value);

            // calculate price based on quantity
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = list.Product.Price;
                ShoppingCartVM.OrderHeader.OrderTotal += list.Product.Price * list.Count;

                // convert description to html format
                list.Product.Description = SD.ConvertToRawHtml(list.Product.Description);

                // show only 100 characters of the description
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
                }
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Plus(int cartId)
        {
            // get cart record
            var cart = _unitOfWork.ShoppingCart
                    .GetFirstOrDefault(c => c.Id == cartId, includeProperties: "Product");

            // increment quantity
            cart.Count += 1;

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            // get shopping cart
            var cart = _unitOfWork.ShoppingCart
                    .GetFirstOrDefault(c => c.Id == cartId, includeProperties: "Product");

            // if cart quantity is 1, then we have to remove it from the shopping cart
            if (cart.Count == 1)
            {
                // get shopping cart length
                var cnt = _unitOfWork.ShoppingCart
                    .GetAll(u => u.ApplicationUserId == cart.ApplicationUserId)
                    .ToList().Count();

                // remove cart record from shopping cart
                _unitOfWork.ShoppingCart.Remove(cart);
                _unitOfWork.Save();

                // update session
                HttpContext.Session.SetInt32(SD.ssShoppingCart, cnt - 1);
            }
            else
            {
                // decrement quantity
                cart.Count -= 1;

                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            // get shopping cart
            var cart = _unitOfWork.ShoppingCart
                    .GetFirstOrDefault(c => c.Id == cartId, includeProperties: "Product");

            // get shopping cart length
            var count = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == cart.ApplicationUserId)
                .ToList().Count();

            // remove cart record from shopping cart
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();

            // update session
            HttpContext.Session.SetInt32(SD.ssShoppingCart, count - 1);

            return RedirectToAction(nameof(Index));
        }
    }
}

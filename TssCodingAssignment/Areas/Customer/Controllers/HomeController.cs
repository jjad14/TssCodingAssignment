﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TssCodingAssignment.DataAccess.Repository.IRepository;
using TssCodingAssignment.Models;
using TssCodingAssignment.Models.ViewModels;

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
            
            return View(homeVM);
        }

        // action method
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

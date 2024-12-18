﻿using Microsoft.AspNetCore.Mvc;
using ValueTechNZ_Final.Repository.IRepository;

namespace ValueTechNZ_Final.Controllers
{
    public class StoreController : Controller
    {

        private readonly ILogger<StoreController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private const int pageSize = 12;

        public StoreController(ILogger<StoreController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        // GET : Items store
        public async Task<IActionResult> Index(int pageNumber = 1, string search = null,
                                               string brand = null, string category = null,
                                               string sort = null)
        {
            try
            {
                // Store current filter values in ViewData to maintain state
                ViewData["CurrentSearch"] = search;
                ViewBag.CurrentBrand = brand;
                ViewBag.CurrentCategory = category;
                ViewBag.CurrentSort = sort;

                ViewBag.Categories = await _unitOfWork.Categories.GetCategoriesAsync();

                var products = await _unitOfWork.Store.GetStoreProductsAsync(
                    pageNumber,
                    pageSize,
                    search,
                    brand,
                    category,
                    sort);

                return View(products);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving product list.");
                ViewBag["ErrorMessage"] = "An error occurred while retrieving product list.";
                return View();
            }
        }


        public async Task<IActionResult> GetProductDetails(int id)
        {
            try
            {
                _logger.LogInformation($"Request to fetch details of product with id {id}");
                var products = await _unitOfWork.Store.GetProductDetailsAsync(id);

                return View(products);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogError($"Product with id {id} not found.");
                TempData["KeyNotFound"] = "Product not found.";
                return RedirectToAction("GetProducts", "Products");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving product details.");
                TempData["ErrorMessage"] = "An error occurred while retrieving product details.";
                return RedirectToAction("GetProducts", "Products");
            }
        }
    }
}

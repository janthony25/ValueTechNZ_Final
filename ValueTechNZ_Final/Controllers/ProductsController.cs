using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using ValueTechNZ_Final.Helpers;
using ValueTechNZ_Final.Models;
using ValueTechNZ_Final.Models.Dto;
using ValueTechNZ_Final.Repository.IRepository;

namespace ValueTechNZ_Final.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private const int pageSize = 10;

        public ProductsController(ILogger<ProductsController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        // GET : Products list
        public async Task<IActionResult> GetProducts(int pageNumber = 1)
        {
            try
            {
                _logger.LogInformation("Request to retrieve product list.");
                var products = await _unitOfWork.Products.GetPaginatedProductsAsync(pageNumber, pageSize);
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving product list.");
                TempData["ErrorMessage"] = "An error occurred while retrieving product list.";
                return View();
            }
        }

        // GET : Add Product
        public async Task<IActionResult> AddProduct()
        {
            ViewBag.CategoryList = await _unitOfWork.Categories.GetCategoriesAsync();
            return View();
        }

        // POST : Add Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(AddUpdateProductDto addDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.CategoryList = await _unitOfWork.Categories.GetCategoriesAsync();
                    return View(addDto);
                }

                await _unitOfWork.Products.AddProductAsync(addDto);
                TempData["SuccessMessage"] = "Product added successfuly!";
                return RedirectToAction("GetProducts");
            }
            catch(Exception ex)
            {
                _logger.LogError("An error occurred while adding new product.");
                TempData["ErrorMessage"] = "An error occurred while adding new product.";
                return RedirectToAction("GetProducts");
            }
        }



    }
}

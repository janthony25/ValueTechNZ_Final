using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using ValueTechNZ_Final.Helpers;
using ValueTechNZ_Final.Models;
using ValueTechNZ_Final.Models.Dto;
using ValueTechNZ_Final.Repository.IRepository;

namespace ValueTechNZ_Final.Controllers
{
    [Route("/Admin/[controller]/{action=GetProducts}/{id?}")]
    [Authorize(Roles ="Admin")]
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
        public async Task<IActionResult> GetProducts(int pageNumber = 1,
                                                     string search = null,
                                                     string sortColumn = "dateadded",
                                                     string sortOrder = "desc")
        {
            try
            {
                _logger.LogInformation("Request to retrieve product list.");
                var products = await _unitOfWork.Products.GetPaginatedProductsAsync(pageNumber, pageSize,
                                                                                    search, sortColumn, sortOrder);
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


        // GET : Edit Product
        public async Task<IActionResult> EditProduct(int id)
        {
            try
            {
                _logger.LogInformation($"Request to retrieve details of product with id {id}");

                var productDetails = await _unitOfWork.Products.GetProductDetailsAsync(id);
                ViewBag.CategoryList = await _unitOfWork.Categories.GetCategoriesAsync();
                return View(productDetails);

            }
            catch (KeyNotFoundException)
            {
                _logger.LogError($"Product with id {id} not found.");
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("GetProducts");
            }
            catch(Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving product details of product with id {id}");
                TempData["ErrorMessage"] = "An error occurred while retrieving product details.";
                return RedirectToAction("GetProducts");
            }
        }

        // POST : Update Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(AddUpdateProductDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.CategoryList = await _unitOfWork.Categories.GetCategoriesAsync();
                    return View(updateDto);
                }

                await _unitOfWork.Products.UpdateProductAsync(updateDto);
                TempData["SuccessMessage"] = "Product successfully updated!";
                return RedirectToAction("GetProducts");
            }
            catch (KeyNotFoundException)
            {
                _logger.LogError($"Product not found.");
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("GetProducts");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating product.");
                TempData["ErrorMessage"] = "An error occurred while updating product.";
                return RedirectToAction("GetProducts");
            }
        }

        // Delete Product
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                _logger.LogInformation($"Request to delete product with id {id}");

                await _unitOfWork.Products.DeleteProductAsync(id);
                TempData["SuccessMessage"] = "Product deleted successfully.";
                return RedirectToAction("GetProducts");
            }
            catch (KeyNotFoundException)
            {
                _logger.LogError($"Product not found.");
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("GetProducts");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting product.");
                TempData["ErrorMessage"] = "An error occurred while deleting product.";
                return RedirectToAction("GetProducts");
            }
        }


    }
}

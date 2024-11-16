using Microsoft.AspNetCore.Mvc;
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
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving product list.");
                return View();
            }
        }
    }
}

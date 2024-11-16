using Microsoft.AspNetCore.Mvc;
using ValueTechNZ_Final.Models;
using ValueTechNZ_Final.Repository.IRepository;

namespace ValueTechNZ_Final.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(ILogger<ProductsController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        // GET : Products list
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                _logger.LogInformation("Request to retrieve product list.");
                var products = await _unitOfWork.Products.GetProductsAsync();
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

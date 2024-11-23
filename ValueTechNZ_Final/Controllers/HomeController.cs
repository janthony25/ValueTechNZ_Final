using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ValueTechNZ_Final.Models;
using ValueTechNZ_Final.Repository.IRepository;

namespace ValueTechNZ_Final.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Request to retrive latest products.");

                var latestProducts = await _unitOfWork.Products.GetLatestProductsAsync();
                ViewBag.CheapestProducts = await _unitOfWork.Products.GetCheapestProductsAsync();
                return View(latestProducts);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving latest products.");
                TempData["ErrorMessage"] = "An error occurred while retrieving latest products.";
                return View("Index");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

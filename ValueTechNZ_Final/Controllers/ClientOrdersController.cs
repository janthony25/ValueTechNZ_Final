using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using System.Drawing.Printing;
using ValueTechNZ_Final.Models;
using ValueTechNZ_Final.Repository;
using ValueTechNZ_Final.Repository.IRepository;

namespace ValueTechNZ_Final.Controllers
{
    [Authorize(Roles = "Client")]
    [Route("/CLient/Orders/{action=Index}/{id?}")]
    public class ClientOrdersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ClientOrdersController> _logger;
        private const int pageSize = 10;

        public ClientOrdersController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,
                                      ILogger<ClientOrdersController> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
        }


        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var orders = await _unitOfWork.ClientOrders.GetPaginatedClientOrdersAsync(User, pageNumber, 10);
            return View(orders);
            
        }

        // GET : Order details
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetOrderDetailsAsync(id);

                return View(order);
            }
            catch (ArgumentException)
            {
                _logger.LogError("User not found.");
                return RedirectToAction("Index", "Home");
            }
            catch (KeyNotFoundException)
            {
                _logger.LogError("Order details not found.");
                TempData["ErrorMessage"] = "Order details not found.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving order details.");
                TempData["ErrorMessage"] = "An error occurred while retrieving order details.";
                return RedirectToAction("Index");
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ValueTechNZ_Final.Data;
using ValueTechNZ_Final.Helpers;
using ValueTechNZ_Final.Models;
using ValueTechNZ_Final.Repository.IRepository;

namespace ValueTechNZ_Final.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("/Admin/Orders/{action=Index}/{id?}")]
    public class AdminOrdersController : Controller
    {
        private readonly ApplicationDbContext _data;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AdminOrdersController> _logger;
        private const int pageSize = 10;

        public AdminOrdersController(IUnitOfWork unitOfWork, ILogger<AdminOrdersController> logger,
                                     ApplicationDbContext data)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _data = data;
        }

        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            try
            {
                // Following the same pattern as your ProductsController
                var orders = await _unitOfWork.Orders.GetPaginatedOrdersAsync(
                    pageNumber,
                    pageSize
                );

                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the order list.");
                TempData["ErrorMessage"] = "An error occurred while retrieving order list.";
                return View(new PaginatedList<Order>(new List<Order>(), 0, pageNumber, pageSize));
            }
        }

        // GET : Order details
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetOrderDetailsAsync(id);
                ViewBag.NumOrders = _data.Orders
                                    .Where(o => o.ClientId == order.ClientId).Count(); 
                
                return View(order);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogError("Order details not found.");
                TempData["ErrorMessage"] = "Order details not found.";
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving order details.");
                TempData["ErrorMessage"] = "An error occurred while retrieving order details.";
                return RedirectToAction("Index");
            }
        }
    }
}

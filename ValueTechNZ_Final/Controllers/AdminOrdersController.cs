using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ValueTechNZ_Final.Data;

namespace ValueTechNZ_Final.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("/Admin/Orders/{action=Index}/{id?}")]
    public class AdminOrdersController : Controller
    {
        private readonly ApplicationDbContext _data;

        public AdminOrdersController(ApplicationDbContext data)
        {
            _data = data;
        }
        public IActionResult Index()
        {

            var orders = _data.Orders
                    .Include(o => o.Client)
                    .Include(o => o.Items)
                    .OrderByDescending(o => o.Id)
                    .ToList();

            ViewBag.Orders = orders;
            return View();
        }
    }
}

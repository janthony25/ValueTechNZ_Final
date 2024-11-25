using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ValueTechNZ_Final.Data;
using ValueTechNZ_Final.Models;

namespace ValueTechNZ_Final.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _data;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly decimal shippingFee;

        public CartController(ApplicationDbContext data, IConfiguration configuration,
                              UserManager<ApplicationUser> userManager)
        {
            _data = data;
            _userManager = userManager;
            shippingFee = configuration.GetValue<decimal>("CartSettings:ShippingFee");
        }
        public IActionResult Index()
        {
            //List<OrderItem> cartItems = 
            return View();
        }
    }
}

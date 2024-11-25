using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ValueTechNZ_Final.Data;
using ValueTechNZ_Final.Models;
using ValueTechNZ_Final.Models.Dto;
using ValueTechNZ_Final.Services;

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
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, _data);
            decimal subtotal = CartHelper.GetSubTotal(cartItems);

            ViewBag.CartItems = cartItems;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Subtotal = subtotal;
            ViewBag.Total = subtotal + shippingFee;

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Index(CheckoutDto model)
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, _data);
            decimal subtotal = CartHelper.GetSubTotal(cartItems);

            ViewBag.CartItems = cartItems;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Subtotal = subtotal;
            ViewBag.Total = subtotal + shippingFee;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if shopping cart is empty or not
            if (cartItems.Count == 0)
            {
                TempData["ErrorMessage"] = "Your cart is empty";
                return View(model);
            }

            TempData["DeliveryAddress"] = model.DeliveryAddress;
            TempData["PaymentMethod"] = model.PaymentMethod;

            return RedirectToAction("Confirm");
        }
    }
}

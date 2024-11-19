using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using SQLitePCL;
using System.CodeDom;
using System.Drawing.Printing;
using ValueTechNZ_Final.Models;
using ValueTechNZ_Final.Repository.IRepository;

namespace ValueTechNZ_Final.Controllers
{

    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UsersController> _logger;
        public UsersController(IUnitOfWork unitOfWork, ILogger<UsersController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

        }
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Request to retrieve user list.");
                var users = await _unitOfWork.Users.GetUsersAsnyc();
                return View(users);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users.");
                TempData["ErrorMessage"] = "An error occurred while retrieving users.";
                return View();
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private const int pageSize = 20;
        public UsersController(IUnitOfWork unitOfWork, ILogger<UsersController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            try
            {
                _logger.LogInformation("Request to retrieve user list.");
                var users = await _unitOfWork.Users.GetUsersAsync(pageNumber, pageSize);
                return View(users);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users.");
                TempData["ErrorMessage"] = "An error occurred while retrieving users.";
                return View();
            }
        }

        public async Task<IActionResult> UserDetails(string id)
        {
            try
            {
                var appUser = await _unitOfWork.Users.GetUserDetailsAsync(id);
                var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

                ViewBag.Roles = await _userManager.GetRolesAsync(appUser);
                ViewBag.AllRoles = allRoles;
                return View(appUser);
            }
            catch(KeyNotFoundException)
            {
                _logger.LogError($"User with id {id} not found.");
                TempData["ErrorMessage"] = "Cannot read user id.";
                return RedirectToAction("Index");
            }
            catch (NullReferenceException)
            {
                _logger.LogError($"No users found.");
                TempData["ErrorMessage"] = "No users found.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user details.");
                TempData["ErrorMessage"] = "An error occurred while retrieving user details.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> EditRole(string id, string newRole)
        {
            if (id == null || newRole == null)
            {
                _logger.LogInformation($"id: {id}, role: {newRole}");
                return RedirectToAction("Index");
            }

            var roleExists = await _roleManager.RoleExistsAsync(newRole);
            var appUser = await _userManager.FindByIdAsync(id);

            if(appUser == null || !roleExists)
            {
                return RedirectToAction("Index");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if(currentUser!.Id == appUser.Id)
            {
                return RedirectToAction("UserDetails", "Users", new { id });
            }

            // Update user role
            var userRoles = await _userManager.GetRolesAsync(appUser);
            await _userManager.RemoveFromRolesAsync(appUser, userRoles); // Delete current user role
            await _userManager.AddToRoleAsync(appUser, newRole); // Create new user role

            TempData["SuccessMessage"] = "Successfully changed user role.";
            return RedirectToAction("UserDetails", "Users", new { id });
        }
    }
}

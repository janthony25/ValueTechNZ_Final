using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using ValueTechNZ_Final.Data;
using ValueTechNZ_Final.Helpers;
using ValueTechNZ_Final.Models;
using ValueTechNZ_Final.Repository.IRepository;

namespace ValueTechNZ_Final.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = loggerFactory.CreateLogger<UserRepository>();
        }

        public async Task<ApplicationUser> EditUserRoleAsync(string id, string newRole)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(newRole))
                {
                    _logger.LogError($"Invalid input: id = {id}, role = {newRole}");
                    throw new ArgumentException("User ID and role are required.");
                }

                // Check if role exists
                var roleExists = await _roleManager.RoleExistsAsync(newRole);
                if (!roleExists)
                {
                    _logger.LogError($"Role '{newRole}' does not exist.");
                    throw new ArgumentException($"The role '{newRole}' does not exist.", nameof(newRole));
                }

                // Get the user
                var appUser = await _userManager.FindByIdAsync(id);
                if (appUser == null)
                {
                    _logger.LogError($"User with ID '{id}' not found.");
                    throw new KeyNotFoundException($"The user with ID '{id}' was not found.");
                }

                // Update roles
                var currentRoles = await _userManager.GetRolesAsync(appUser);
                await _userManager.RemoveFromRolesAsync(appUser, currentRoles); // Remove current roles
                await _userManager.AddToRoleAsync(appUser, newRole);           // Add new role

                // Return the updated user
                return appUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user role.");
                throw;
            }
        }


        public async Task<ApplicationUser> GetUserDetailsAsync(string id)
        {
            try
            {
                if(id == null || id == "")
                {
                    throw new KeyNotFoundException("User not found.");
                }

                var user = await _userManager.FindByIdAsync(id);

                if(user == null)
                {
                    _logger.LogError($"user with id {id} not found.");
                    throw new NullReferenceException("No users found.");
                }

                return user;


            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx.Message);
                throw; // Re-throwing to handle it at a higher level, if necessary.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while retrieving user details.");
                throw;
            }
        }

        public async Task<PaginatedList<ApplicationUser>> GetUsersAsync(int pageNumber, int pageSize)
        {
            try
            {
                var query = _userManager.Users
                            .OrderByDescending(u => u.CreatedAt)
                            .AsQueryable();

                

                return await PaginatedList<ApplicationUser>.CreateAsync(query, pageNumber, pageSize);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user list.");
                throw;
            }
        }
    }
}

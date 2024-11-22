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

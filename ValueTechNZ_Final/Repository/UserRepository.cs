using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ValueTechNZ_Final.Data;
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
        public async Task<List<ApplicationUser>> GetUsersAsnyc()
        {
            try
            {
                var users = await _userManager.Users
                            .OrderByDescending(u => u.CreatedAt)
                            .ToListAsync();

                return users;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occured while retrieving user roles.");
                throw;
            }
        }
    }
}

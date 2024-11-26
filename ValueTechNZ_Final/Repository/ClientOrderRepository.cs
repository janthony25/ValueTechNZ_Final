using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ValueTechNZ_Final.Data;
using ValueTechNZ_Final.Helpers;
using ValueTechNZ_Final.Models;
using ValueTechNZ_Final.Repository.IRepository;

namespace ValueTechNZ_Final.Repository
{
    public class ClientOrderRepository : IClientOrderRepository
    {

        private readonly ApplicationDbContext _data;
        private readonly ILogger<ClientOrderRepository> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientOrderRepository(ApplicationDbContext data, ILoggerFactory loggerFactory,
                                     UserManager<ApplicationUser> userManager)
        {
            _data = data;
            _logger = loggerFactory.CreateLogger<ClientOrderRepository>();
            _userManager = userManager;
        }

        public async Task<PaginatedList<Order>> GetPaginatedClientOrdersAsync(ClaimsPrincipal user,int pageNumber, int pageSize)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(user);
                if (currentUser == null)
                {
                    throw new ArgumentException("User not found.");
                }

                var query = _data.Orders
                  .Include(o => o.Items)
                  .OrderByDescending(o => o.CreatedAt)
                  .Where(o => o.ClientId == currentUser.Id)
                  .AsQueryable();

                return await PaginatedList<Order>.CreateAsync(query, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch order list.");
                throw;
            }
        }
    }
}

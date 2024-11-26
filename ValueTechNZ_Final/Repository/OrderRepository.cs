using Microsoft.EntityFrameworkCore;
using ValueTechNZ_Final.Data;
using ValueTechNZ_Final.Helpers;
using ValueTechNZ_Final.Models;
using ValueTechNZ_Final.Repository.IRepository;

namespace ValueTechNZ_Final.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _data;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(ApplicationDbContext data, ILoggerFactory loggerFactory)
        {
            _data = data;
            _logger = loggerFactory.CreateLogger<OrderRepository>();
        }

        public async Task<PaginatedList<Order>> GetPaginatedOrdersAsync(int pageNumber, int pageSize)
        {
            try
            {
                var query = _data.Orders
                    .Include(o => o.Client)
                    .Include(o => o.Items)
                    .OrderByDescending(o => o.CreatedAt)
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

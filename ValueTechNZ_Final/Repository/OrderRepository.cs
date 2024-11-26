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

        public async Task<Order> GetOrderDetailsAsync(int id)
        {
            try
            {
                var order = await _data.Orders
                        .Include(o => o.Client)
                        .Include(o => o.Items)
                            .ThenInclude(oi => oi.Product)
                        .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    _logger.LogError($"order details with id {order.Id} not found.");
                    throw new KeyNotFoundException("Order details not found.");
                }

                _logger.LogInformation($"Successfully retrieved order details for order with id {order.Id}");
                return order;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving order details.");
                throw;
            }
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

        public async Task UpdateStatusAsync(int id, string? payment_status, string? order_status)
        {
            try
            {
                var order = await _data.Orders.FindAsync(id);

                if(order == null)
                {
                    _logger.LogError($"Order wil id {id} not found.");
                    throw new KeyNotFoundException("Order not found.");
                }

                if (payment_status == null && order_status == null)
                {
                    _logger.LogError("Order and Payment status not found.");
                    throw new ArgumentException("Order and Payment status not found.");
                }

                if (payment_status != null)
                {
                    order.PaymentStatus = payment_status;
                }

                if (order_status != null)
                {
                    order.OrderStatus = order_status;
                }

                await _data.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating order.");
                throw;
            }
        }
    }
}

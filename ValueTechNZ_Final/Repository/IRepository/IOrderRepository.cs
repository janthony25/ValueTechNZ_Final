using ValueTechNZ_Final.Helpers;
using ValueTechNZ_Final.Models;

namespace ValueTechNZ_Final.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<PaginatedList<Order>> GetPaginatedOrdersAsync(int pageNumber, int pageSize);

    }
}

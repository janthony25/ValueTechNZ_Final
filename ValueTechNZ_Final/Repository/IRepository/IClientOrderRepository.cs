using System.Security.Claims;
using ValueTechNZ_Final.Helpers;
using ValueTechNZ_Final.Models;

namespace ValueTechNZ_Final.Repository.IRepository
{
    public interface IClientOrderRepository
    {
        Task<PaginatedList<Order>> GetPaginatedClientOrdersAsync(ClaimsPrincipal user,int pageNumber, int pageSize);
        Task<Order> GetClientOrderDetailsAsync(ClaimsPrincipal user,int id);
    }
}   

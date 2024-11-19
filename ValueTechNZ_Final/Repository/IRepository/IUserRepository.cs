using Microsoft.AspNetCore.Identity;
using ValueTechNZ_Final.Helpers;
using ValueTechNZ_Final.Models;

namespace ValueTechNZ_Final.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<PaginatedList<ApplicationUser>> GetUsersAsync(int pageNumber, int pageSize);
    }
}

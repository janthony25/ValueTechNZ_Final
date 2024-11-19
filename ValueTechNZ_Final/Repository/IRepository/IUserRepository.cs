using Microsoft.AspNetCore.Identity;
using ValueTechNZ_Final.Models;

namespace ValueTechNZ_Final.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<List<ApplicationUser>> GetUsersAsnyc();
    }
}

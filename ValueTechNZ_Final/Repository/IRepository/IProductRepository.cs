using ValueTechNZ_Final.Helpers;
using ValueTechNZ_Final.Models;
using ValueTechNZ_Final.Models.Dto;

namespace ValueTechNZ_Final.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<List<GetProductsDto>> GetProductsAsync();
        Task<PaginatedList<GetProductsDto>> GetPaginatedProductsAsync(int pageNumber,
                                                                      int pagSize);
    }
}

using ValueTechNZ_Final.Helpers;
using ValueTechNZ_Final.Models.Dto;

namespace ValueTechNZ_Final.Repository.IRepository
{
    public interface IStoreRepository
    {
        Task<PaginatedList<GetProductsDto>> GetStoreProductsAsync(int pageNumber,
                                                                  int pageSize,
                                                                  string? searchTerm,
                                                                  string? brand,
                                                                  string? category,
                                                                  string? sort);

        Task<GetProductsDto> GetProductDetailsAsync(int id);
    }
}

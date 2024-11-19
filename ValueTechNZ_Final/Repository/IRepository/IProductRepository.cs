using ValueTechNZ_Final.Helpers;
using ValueTechNZ_Final.Models;
using ValueTechNZ_Final.Models.Dto;

namespace ValueTechNZ_Final.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<PaginatedList<GetProductsDto>> GetPaginatedProductsAsync(int pageNumber,
                                                                      int pagSize,
                                                                      string? searchTerm,
                                                                      string sortColumn = "DateAdded",
                                                                      string sortOrder = "desc");
        Task AddProductAsync(AddUpdateProductDto addDto);
        Task<AddUpdateProductDto> GetProductDetailsAsync(int id);
        Task UpdateProductAsync(AddUpdateProductDto updateDto);
        Task<List<GetProductsDto>> GetLatestProductsAsync();
        Task DeleteProductAsync(int id);
    }
}

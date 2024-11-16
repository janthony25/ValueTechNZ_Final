using ValueTechNZ_Final.Models.Dto;

namespace ValueTechNZ_Final.Repository.IRepository
{
    public interface ICategoryRepository
    {
        Task<List<CategoryListDto>> GetCategoriesAsync();
    }
}

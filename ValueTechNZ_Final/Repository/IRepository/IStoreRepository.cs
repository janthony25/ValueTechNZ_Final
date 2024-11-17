using ValueTechNZ_Final.Models.Dto;

namespace ValueTechNZ_Final.Repository.IRepository
{
    public interface IStoreRepository
    {
        Task<List<GetProductsDto>> GetLatestProductsAsycn();
    }
}

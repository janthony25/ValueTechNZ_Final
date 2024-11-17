using ValueTechNZ_Final.Data;
using ValueTechNZ_Final.Models.Dto;
using ValueTechNZ_Final.Repository.IRepository;

namespace ValueTechNZ_Final.Repository
{
    public class StoreRepository : IStoreRepository
    {
        private readonly ApplicationDbContext _data;
        private readonly ILogger<StoreRepository> _logger;
        public StoreRepository(ApplicationDbContext data, ILoggerFactory loggerFactory)
        {
            _data = data;
            _logger = loggerFactory.CreateLogger<StoreRepository>();
        }
        public Task<List<GetProductsDto>> GetLatestProductsAsycn()
        {
            throw new NotImplementedException();
        }
    }
}

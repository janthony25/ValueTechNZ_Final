using Microsoft.EntityFrameworkCore;
using ValueTechNZ_Final.Data;
using ValueTechNZ_Final.Models.Dto;
using ValueTechNZ_Final.Repository.IRepository;

namespace ValueTechNZ_Final.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _data;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(ApplicationDbContext data, ILoggerFactory loggerFactory)
        {
            _data = data;
            _logger = loggerFactory.CreateLogger<CategoryRepository>();
        }
        public async Task<List<CategoryListDto>> GetCategoriesAsync()
        {
            try
            {
                var categories = await _data.Categories
                        .Select(c => new CategoryListDto
                        {
                            CategoryId = c.CategoryId,
                            CategoryName = c.CategoryName
                        }).ToListAsync();

                _logger.LogInformation($"Category list retrieved successfully. There are {categories.Count} categories in the list.");
                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving category list.");
                throw;
            }
        }
    }
}

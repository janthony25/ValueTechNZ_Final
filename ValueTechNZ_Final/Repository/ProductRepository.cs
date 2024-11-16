using Microsoft.EntityFrameworkCore;
using ValueTechNZ_Final.Data;
using ValueTechNZ_Final.Models;
using ValueTechNZ_Final.Models.Dto;
using ValueTechNZ_Final.Repository.IRepository;

namespace ValueTechNZ_Final.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _data;
        private readonly ILogger<ProductRepository> _logger;
        private readonly IWebHostEnvironment _environment;

        public ProductRepository(ApplicationDbContext data, ILoggerFactory loggerFactory, IWebHostEnvironment environment)
        {
            _data = data;
            _logger = loggerFactory.CreateLogger<ProductRepository>();
            _environment = environment;
        }
        public async Task<List<GetProductsDto>> GetProductsAsync()
        {
            try
            {
                var products = await _data.Products
                            .Include(p => p.ProductCategory)
                                .ThenInclude(pc => pc.Category)
                            .Select(p => new GetProductsDto
                            {
                                ProductId = p.ProductId,
                                ProductName = p.ProductName,
                                Brand = p.Brand,
                                Price = p.Price,
                                Description = p.Description,
                                ImageFileName = p.ImageFileName,
                                CategoryName = p.ProductCategory.Select(pc => pc.Category.CategoryName).FirstOrDefault()
                            }).ToListAsync();

                _logger.LogInformation($"Retrieved {products.Count} products");
                return products;
            }
            catch(Exception ex)
            {
                _logger.LogError("An error occurred while retrieving product list.");
                throw;
            }
        }
    }
}

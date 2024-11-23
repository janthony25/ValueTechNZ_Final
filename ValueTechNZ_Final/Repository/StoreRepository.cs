using Microsoft.EntityFrameworkCore;
using ValueTechNZ_Final.Data;
using ValueTechNZ_Final.Helpers;
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

       
        public async Task<GetProductsDto> GetProductDetailsAsync(int id)
        {
            try
            {
                // Find product by id
                var product = await _data.Products
                        .Include(p => p.ProductCategory)
                            .ThenInclude(pc => pc.Category)
                        .Where(p => p.ProductId == id)
                        .Select(p => new GetProductsDto
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            Brand = p.Brand,
                            Price = p.Price,
                            CategoryName = p.ProductCategory.Select(pc => pc.Category.CategoryName).FirstOrDefault(),
                            Description = p.Description,
                            ImageFileName = p.ImageFileName,
                            DateAdded = p.DateAdded,
                            DateUpdated = p.DateUpdated
                        }).FirstOrDefaultAsync();

                if(product == null || product.ProductId == 0)
                {
                    _logger.LogError($"Product with id {product.ProductId} not found.");
                    throw new KeyNotFoundException("Product not found.");
                }

                _logger.LogInformation($"Product with id {product.ProductId} retrieved successfully.");
                return product;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving product details.");
                throw;
            }
        }

        public async Task<PaginatedList<GetProductsDto>> GetStoreProductsAsync(int pageNumber, int pageSize, string? searchTerm, string? brand, string? category, string? sort)
        {
            try
            {
                var query = _data.Products
                        .Include(p => p.ProductCategory)
                            .ThenInclude(pc => pc.Category)
                        .AsQueryable();

                // Apply search filter if search term is provided
                if(!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(p =>
                        p.ProductName.ToLower().Contains(searchTerm) ||
                        p.Brand.ToLower().Contains(searchTerm) ||
                        p.ProductCategory.Any(pc => pc.Category.CategoryName.ToLower().Contains(searchTerm))
                    );
                }

                // Apply brand filter
                if (!string.IsNullOrWhiteSpace(brand))
                {
                    query = query.Where(p => p.Brand.ToLower() == brand.ToLower());
                }

                // Apply category filter
                if (!string.IsNullOrWhiteSpace(category))
                {
                    query = query.Where(p => p.ProductCategory.Any(pc => pc.Category.CategoryName.ToLower() == category.ToLower()));
                }

                // Apply sorting by date
                query = sort?.ToLower() switch
                {
                    "price_asc" => query.OrderBy(p => p.Price),
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    "newest" => query.OrderByDescending(p => p.DateAdded),
                    _ => query.OrderByDescending(p => p.DateAdded)
                };

                var finalQuery = query.Select(p => new GetProductsDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Brand = p.Brand,
                    Price = p.Price,
                    CategoryName = p.ProductCategory
                            .Select(pc => pc.Category.CategoryName)
                            .FirstOrDefault(),
                    Description = p.Description,
                    ImageFileName = p.ImageFileName,
                    DateAdded = p.DateAdded
                });

                return await PaginatedList<GetProductsDto>.CreateAsync(finalQuery, pageNumber, pageSize);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to retrieve product list.");
                throw;
            }
        }
    }
}

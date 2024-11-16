using Microsoft.EntityFrameworkCore;
using ValueTechNZ_Final.Data;
using ValueTechNZ_Final.Helpers;
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

        public async Task AddProductAsync(AddUpdateProductDto addDto)
        {
            try
            {
                string newFileName = null;

                if (addDto.ImageFile != null && addDto.ImageFile.Length > 0)
                {
                    newFileName = DateTime.Now.ToString("yyyyMMddhhssfff");
                    newFileName += Path.GetExtension(addDto.ImageFile!.FileName);

                    string imageFullPath = _environment.WebRootPath + "/images/" + newFileName;
                    using (var stream = System.IO.File.Create(imageFullPath))
                    {
                        addDto.ImageFile.CopyTo(stream);
                    }
                }
                else
                {
                    // Assign default image
                    newFileName = "No image.png";
                }

                // Add new product
                var product = new Product
                {
                    ProductName = addDto.ProductName,
                    Brand = addDto.Brand,
                    Price = addDto.Price,
                    Description = addDto.Description,
                    ImageFileName = newFileName
                };

                // Add new product to DB
                _data.Products.Add(product);
                await _data.SaveChangesAsync();

                // Create new Product Category entity
                var productCategory = new ProductCategory
                {
                    ProductId = product.ProductId,
                    CategoryId = addDto.CategoryId
                };

                // Save product category to DB
                _data.ProductCategories.Add(productCategory);
                await _data.SaveChangesAsync();

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding new product.");
                throw;
            }
        }

        public async Task<PaginatedList<GetProductsDto>> GetPaginatedProductsAsync(int pageNumber, int pagSize)
        {
            try
            {
                var query = _data.Products
                    .Include(p => p.ProductCategory)
                        .ThenInclude(pc => pc.Category)
                    .AsQueryable();

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

                return await PaginatedList<GetProductsDto>.CreateAsync(finalQuery, pageNumber, pagSize);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving product list.");
                throw;
            }            
        }

    }
}

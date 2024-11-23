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

        public async Task DeleteProductAsync(int id)
        {
            try
            {
                var product = await _data.Products.FindAsync(id);

                if(product == null)
                {
                    throw new KeyNotFoundException($"Product with id {id} not found");
                }

                string imageFullPath = Path.Combine(_environment.WebRootPath, "images", product.ImageFileName);
                
                // Check if the image is not the default image
                if(!product.ImageFileName.Equals("No image.png", StringComparison.OrdinalIgnoreCase))
                {
                    if (File.Exists(imageFullPath))
                    {
                        File.Delete(imageFullPath);
                    }
                }
                

                _data.Products.Remove(product);
                await _data.SaveChangesAsync();

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting product.");
                throw;
            }
        }

        public async Task<List<GetProductsDto>> GetLatestProductsAsync()
        {
            try
            {
                // Take 4 latest products
                var latestProducts = await _data.Products
                            .Include(p => p.ProductCategory)
                                .ThenInclude(pc => pc.Category)
                            .Take(4)
                            .Select(p => new GetProductsDto
                            {
                                ProductId = p.ProductId,
                                ProductName = p.ProductName,
                                Brand = p.Brand,
                                CategoryName = p.ProductCategory.Select(pc => pc.Category.CategoryName)
                                        .FirstOrDefault(),
                                Price = p.Price,
                                ImageFileName = p.ImageFileName
                            }).ToListAsync();

                _logger.LogInformation($"Latest products retrieved successfully!");
                return latestProducts;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving latest products.");
                throw;
            }
        }

        public async Task<PaginatedList<GetProductsDto>> GetPaginatedProductsAsync(int pageNumber, int pagSize,
                                                                                   string? searchTerm,
                                                                                   string sortColumn = "DateAdded",
                                                                                   string sortOrder = "desc")   
        {
            try
            {
                var query = _data.Products
                    .Include(p => p.ProductCategory)
                        .ThenInclude(pc => pc.Category)
                    .AsQueryable();

                // Apply search filter if the searchTerm is provided
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(p =>
                        p.ProductName.ToLower().Contains(searchTerm) || 
                        p.Brand.ToLower().Contains(searchTerm) || 
                        p.ProductCategory.Any(pc => pc.Category.CategoryName.ToLower().Contains(searchTerm)
                        ));
                }

                // Apply sorting based on column
                query = sortColumn.ToLower() switch
                {
                    "id" => sortOrder == "desc" ? query.OrderByDescending(p => p.ProductId) : query.OrderBy(p => p.ProductId),
                    "name" => sortOrder == "desc" ? query.OrderByDescending(p => p.ProductName) : query.OrderBy(p => p.ProductName),
                    "brand" => sortOrder == "desc" ? query.OrderByDescending(p => p.Brand) : query.OrderBy(p => p.Brand),
                    "category" => sortOrder == "desc" ?
                        query.OrderByDescending(p => p.ProductCategory.FirstOrDefault().Category.CategoryName) :
                        query.OrderBy(p => p.ProductCategory.FirstOrDefault().Category.CategoryName),
                    "price" => sortOrder == "desc" ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                    "dateadded" => sortOrder == "desc" ? query.OrderByDescending(p => p.DateAdded) : query.OrderBy(p => p.DateAdded),
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

                return await PaginatedList<GetProductsDto>.CreateAsync(finalQuery, pageNumber, pagSize);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving product list.");
                throw;
            }            
        }

        public async Task<AddUpdateProductDto> GetProductDetailsAsync(int id)
        {
            try
            {
                var productDetails = await _data.Products
                        .Include(p => p.ProductCategory)
                            .ThenInclude(pc => pc.Category)
                        .Where(p => p.ProductId == id)
                        .Select(p => new AddUpdateProductDto
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            Brand = p.Brand,
                            Price = p.Price,
                            CategoryId = p.ProductCategory.Select(p => p.Category.CategoryId).FirstOrDefault(),
                            Description = p.Description
                        }).FirstOrDefaultAsync();

                if(productDetails == null || productDetails.ProductId == 0)
                {
                    _logger.LogWarning($"Product with id {productDetails.ProductId} not found.");
                    throw new KeyNotFoundException("Product not found.");
                }

                _logger.LogInformation($"Successfully fetched details from product with id {productDetails.ProductId}.");
                return productDetails;
            }
            catch(Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving product details of product with id {id}");
                throw;
            }
        }

        public async Task UpdateProductAsync(AddUpdateProductDto updateDto)
        {
            try
            {
                // GET Product by id
                var product = await _data.Products.FindAsync(updateDto.ProductId);

                if (product == null || product.ProductId == 0)
                {
                    _logger.LogError($"product with id {product.ProductId} not found.");
                    throw new KeyNotFoundException("Product not found.");
                }

                // Update the image file if we want a new one
                string newFileName = product.ImageFileName;
                if (updateDto.ImageFile != null && updateDto.ImageFile.Length > 1)
                {
                    newFileName = DateTime.Now.ToString("yyyyMMddHHssfff");
                    newFileName += Path.GetExtension(updateDto.ImageFile.FileName);

                    string imageFullPath = _environment.WebRootPath + "/images/" + newFileName;
                    using (var stream = System.IO.File.Create(imageFullPath))
                    {
                        updateDto.ImageFile.CopyTo(stream);
                    }

                    // delete the old image
                    string oldImageFullPath = _environment.WebRootPath + "/images/" + product.ImageFileName;
                    System.IO.File.Delete(oldImageFullPath);
                }

                // Update products
                product.ProductName = updateDto.ProductName;
                product.Brand = updateDto.Brand;
                product.Price = updateDto.Price;
                product.Description = updateDto.Description;
                product.ImageFileName = newFileName;
                product.DateUpdated = DateTime.Now;

                // Find Product-Category Relationship
                var existingProductCategory = await _data.ProductCategories
                                .FirstOrDefaultAsync(pc => pc.ProductId == product.ProductId);

                // Update <Product Category> if the category has been changed
                if(existingProductCategory.CategoryId != updateDto.CategoryId)
                {
                    // Remove existing relationship
                    _data.ProductCategories.Remove(existingProductCategory);

                    // Create new relationship
                    var newProductCategory = new ProductCategory
                    {
                        ProductId = product.ProductId,
                        CategoryId = updateDto.CategoryId
                    };

                    await _data.ProductCategories.AddAsync(newProductCategory);
                }

                await _data.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product.");
                throw;
            }
        }
    }
}

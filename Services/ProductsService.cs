using ApplicationToSellThings.APIs.Data;
using ApplicationToSellThings.APIs.Models;
using ApplicationToSellThings.APIs.Services.Interface;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace ApplicationToSellThings.APIs.Services
{
    public class ProductsService : IProductsService
    {
        private readonly ApplicationToSellThingsAPIsContext _dbContext;

        public ProductsService(ApplicationToSellThingsAPIsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            var products = _dbContext.Products.ToList();
            return products;
        }

        public async Task<ProductViewResponseModel> CreateProduct(Product product)
        {
            if (product != null)
            {
                var productRequest = new Product()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = product.ProductName,
                    BrandName = product.BrandName,
                    Price = product.Price,
                    Discount = product.Discount,
                    Description = product.Description,
                    Category = product.Category,
                    QuantityInStock = product.QuantityInStock,
                    CreatedAt = DateTime.Now,
                    ProductImage = product.ProductImage,
                };
                
                _dbContext.Products.Add(productRequest);
                await _dbContext.SaveChangesAsync();

                var productResponse = new ProductViewResponseModel()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = product.ProductName,
                    BrandName = product.BrandName,
                    Price = product.Price,
                    Discount = product.Discount,
                    Description = product.Description,
                    Category = product.Category,
                    QuantityInStock = product.QuantityInStock,
                    CreatedAt = DateTime.Now,
                    ProductImage = product.ProductImage,
                };

                return productResponse;
            }

            return null;
        }

        public async Task<Product> GetProductById(Guid id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            return product;
        }

        public async Task<ResponseModel<Product>> UpdateProduct(Guid productId, Product productModel)
        {
            try
            {
                var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (product == null)
                {
                    return new ResponseModel<Product>
                    {
                        StatusCode = 404,
                        Status = "Not Found",
                        Message = "Product not found"
                    };
                }

                product.ProductName = productModel.ProductName;
                product.BrandName = productModel.BrandName;
                product.Price = productModel.Price;
                product.Description = productModel.Description;
                product.Category = productModel.Category;
                product.QuantityInStock = productModel.QuantityInStock;
                product.Discount = productModel.Discount;
                product.CreatedAt = productModel.CreatedAt;
                product.ProductImage = productModel.ProductImage;

                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();

                return new ResponseModel<Product>
                {
                    StatusCode = 200,
                    Status = "Success",
                    Data = product
                };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new ResponseModel<Product>
                {
                    StatusCode = 409,
                    Status = "Concurrency Error",
                    Message = "The record you attempted to edit was modified by another user after you got the original value. The edit operation was canceled."
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<Product>
                {
                    StatusCode = 500,
                    Status = "Error",
                    Message = ex.Message
                };
            }
        }
        
        public async Task DeleteProduct(Guid id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
        }
    }
}

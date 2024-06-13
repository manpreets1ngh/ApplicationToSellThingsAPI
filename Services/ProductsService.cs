using ApplicationToSellThings.APIs.Data;
using ApplicationToSellThings.APIs.Models;
using ApplicationToSellThings.APIs.Services.Interface;

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

        public async Task<Product> CreateProduct(Product product)
        {
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            return product;
        }

        public async Task<Product> GetProductById(Guid id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            return product;
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            var result = _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task DeleteProduct(Guid id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
        }
    }
}

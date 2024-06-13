using ApplicationToSellThings.APIs.Models;

namespace ApplicationToSellThings.APIs.Services.Interface
{
    public interface IProductsService
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> CreateProduct(Product product);
        Task<Product> GetProductById(Guid id);
        Task<Product> UpdateProduct(Product product);
        Task DeleteProduct(Guid id);
    }
}

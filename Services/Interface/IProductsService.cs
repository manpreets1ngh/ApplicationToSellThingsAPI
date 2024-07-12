using ApplicationToSellThings.APIs.Models;

namespace ApplicationToSellThings.APIs.Services.Interface
{
    public interface IProductsService
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<ProductViewResponseModel> CreateProduct(Product product);
        Task<Product> GetProductById(Guid id);
        Task<ResponseModel<Product>> UpdateProduct(Guid productId, Product productModel);
        Task DeleteProduct(Guid id);
    }
}

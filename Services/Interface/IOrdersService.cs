using ApplicationToSellThings.APIs.Models;

namespace ApplicationToSellThings.APIs.Services.Interface
{
    public interface IOrdersService
    {
        Task<ResponseModel<OrderApiResponseModel>> CreateOrder(OrderApiRequestModel orderRequestModel);
        Task<ResponseModel<Order>> GetOrdersByUserId(Guid userId);
    }
}

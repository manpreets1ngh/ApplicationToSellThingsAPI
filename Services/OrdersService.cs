using ApplicationToSellThings.APIs.Data;
using ApplicationToSellThings.APIs.Models;
using ApplicationToSellThings.APIs.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ApplicationToSellThings.APIs.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly ApplicationToSellThingsAPIsContext _dbContext;

        public OrdersService(ApplicationToSellThingsAPIsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseModel<OrderApiResponseModel>> CreateOrder(OrderApiRequestModel orderRequestModel)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var orderData = new Order
                {
                    OrderId = Guid.NewGuid(),
                    UserId = orderRequestModel.UserId,
                    PaymentMethod = orderRequestModel.PaymentMethod,
                    CardId = orderRequestModel.CardId,
                    TotalAmount = 0, // This will be calculated later
                    Tax = 0,
                    OrderStatus = "Pending",
                    OrderCreatedAt = DateTime.UtcNow,
                    OrderDetails = new List<OrderDetail>()
                };

                decimal totalAmount = 0;

                foreach (var productModel in orderRequestModel.Products)
                {
                    var product = await _dbContext.Products.FindAsync(productModel.ProductId);
                    if (product == null)
                    {
                        return new ResponseModel<OrderApiResponseModel>
                        {
                            StatusCode = 404,
                            Status = "Error",
                            Message = $"Product with ID {productModel.ProductId} not found",
                        };
                    }

                    var orderDetail = new OrderDetail
                    {
                        OrderDetailId = Guid.NewGuid(),
                        OrderId = orderData.OrderId,
                        ProductId = product.ProductId,
                        AddressId = orderRequestModel.ShippingAddressId,
                        Quantity = productModel.Quantity,
                        Total = (decimal)product.Price * productModel.Quantity,
                        Product = product
                    };

                    totalAmount += orderDetail.Total;

                    orderData.OrderDetails.Add(orderDetail);
                }

                orderData.TotalAmount = totalAmount;

                _dbContext.Orders.Add(orderData);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = new ResponseModel<OrderApiResponseModel>()
                {
                    StatusCode = 200,
                    Status = "Success",
                    Message = "Order Created Successfully",
                    Data = new OrderApiResponseModel
                    {
                        PaymentMethod = orderData.PaymentMethod,
                        TotalAmount = orderData.TotalAmount,
                        Tax = orderData.Tax,
                        OrderStatus = orderData.OrderStatus,
                        OrderCreatedAt = orderData.OrderCreatedAt,
                        OrderDetails = orderData.OrderDetails.Select(od => new OrderDetailApiResponseModel
                        {
                            ProductId = od.ProductId,
                            Quantity = od.Quantity,
                            Total = od.Total,
                            ProductName = od.Product.ProductName
                        }).ToList()
                    },
                };

                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ResponseModel<OrderApiResponseModel>
                {
                    StatusCode = 500,
                    Status = "Error",
                    Message = ex.Message.ToString(),
                };
            }
        }

        public async Task<ResponseModel<Order>> GetOrdersByUserId(Guid userId)
        {
            try
            {
                List<Order> addressResponseViewModel = new List<Order>();
                string id = userId.ToString();
                var orders = await _dbContext.Orders
                    .Include(o => o.OrderDetails)                                    
                    .ThenInclude(od => od.Product) 
                    .Where(o => o.UserId == userId)
                    .ToListAsync();

                if (orders != null)
                {
                    var response = new ResponseModel<Order>()
                    {
                        StatusCode = 200,
                        Status = "Success",
                        Message = "List of orders found",
                        Items = orders,
                    };

                    return response;
                }
                else
                {
                    return new ResponseModel<Order>
                    {
                        StatusCode = 404,
                        Status = "Not Found",
                        Message = "Orders Not Found!",
                        Items = null,
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<Order>
                {
                    StatusCode = 500,
                    Status = "Error",
                    Message = ex.Message.ToString(),
                };
            }
        }
    }
}

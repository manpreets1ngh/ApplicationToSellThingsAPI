using ApplicationToSellThings.APIs.Models;
using ApplicationToSellThings.APIs.Services;
using ApplicationToSellThings.APIs.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationToSellThings.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _orderService;
        private readonly IConfiguration _config;
        public OrdersController(IOrdersService orderService, IConfiguration config)
        {
            _orderService = orderService;
            _config = config;
        }

        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> CreateOrder(OrderApiRequestModel orderRequestModel)
        {
            try
            {
                var result = await _orderService.CreateOrder(orderRequestModel);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetOrdersByUser(Guid userId)
        {
            var result = await _orderService.GetOrdersByUserId(userId);
            return Ok(result);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return Ok(result);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(Order order)
        {
            try
            {
                var result = await _orderService.UpdateOrderById(order.OrderId, order);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<string>
                {
                    StatusCode = 500,
                    Status = "Error",
                    Message = ex.Message,
                };

                return StatusCode(500, new { status = response.Status, message = response.Message });
            }
        }
    }
}

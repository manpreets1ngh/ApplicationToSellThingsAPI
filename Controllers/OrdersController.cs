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
        [Authorize(Policy = "UserOnly")]
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
    }
}

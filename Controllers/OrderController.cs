using AutoMapper;
using E_CommerceSystem.Models;
using E_CommerceSystem.Models.DTOs;
using E_CommerceSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace E_CommerceSystem.Controllers
{
    [Authorize] // 🔒 requires valid JWT
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        // ✅ Place Order
        [HttpPost("PlaceOrder")]
        public IActionResult PlaceOrder([FromBody] List<OrderItemDTO> items)
        {
            try
            {
                if (items == null || !items.Any())
                    return BadRequest("Order items cannot be empty.");

                var uid = GetUserIdFromToken();
                _orderService.PlaceOrder(items, uid);

                return Ok("Order placed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while placing order. {ex.Message}");
            }
        }

        // ✅ Get All Orders for logged-in user
        [HttpGet("GetAllOrders")]
        public IActionResult GetAllOrders()
        {
            try
            {
                var uid = GetUserIdFromToken();
                var orders = _orderService.GetAllOrders(uid);

                var ordersDto = _mapper.Map<IEnumerable<OrdersOutputDTO>>(orders);
                return Ok(ordersDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving orders. {ex.Message}");
            }
        }

        // ✅ Get Order by ID (must belong to logged-in user)
        [HttpGet("GetOrderById/{orderId}")]
        public IActionResult GetOrderById(int orderId)
        {
            try
            {
                var uid = GetUserIdFromToken();
                var order = _orderService.GetOrderById(orderId, uid);

                if (order == null)
                    return NotFound($"No order found with ID {orderId} for this user.");

                var orderDto = _mapper.Map<IEnumerable<OrdersOutputDTO>>(order);
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving order. {ex.Message}");
            }
        }

        // ✅ Order Summaries
        [HttpGet("GetOrderSummaries")]
        public IActionResult GetOrderSummaries()
        {
            try
            {
                var uid = GetUserIdFromToken();
                var summaries = _orderService.GetOrderSummaries(uid);
                return Ok(summaries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving order summaries. {ex.Message}");
            }
        }

        [HttpPut("CancelOrder/{orderId}")]
        public IActionResult CancelOrder(int orderId)
        {
            try
            {
                var userId = GetUserIdFromToken(); // no parse needed

                _orderService.CancelOrder(orderId, userId);

                return Ok($"Order {orderId} has been cancelled and stock restored.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPut("UpdateStatus/{orderId}")]
        public IActionResult UpdateOrderStatus(int orderId, [FromQuery] OrderStatus status)
        {
            try
            {
                var userId = GetUserIdFromToken();
                _orderService.UpdateOrderStatus(orderId, userId, status);

                return Ok($"Order {orderId} status updated to {status}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }



        // 🔑 Extract UserId from JWT
        private int GetUserIdFromToken()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "nameid");

                if (subClaim != null && int.TryParse(subClaim.Value, out int uid))
                    return uid;
            }

            throw new UnauthorizedAccessException("Invalid or unreadable token.");
        }
    }
}

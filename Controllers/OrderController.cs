using E_CommerceSystem.Models;
using E_CommerceSystem.Models.DTOs;
using E_CommerceSystem.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace E_CommerceSystem.Controllers
{
    [Authorize]
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

        // POST: api/order/PlaceOrder
        [HttpPost("PlaceOrder")]
        public IActionResult PlaceOrder([FromBody] List<OrderItemDTO> items)
        {
            try
            {
                if (items == null || !items.Any())
                {
                    return BadRequest("Order items cannot be empty.");
                }

                // Extract user ID from JWT
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userId = GetUserIdFromToken(token);
                int uid = int.Parse(userId);

                _orderService.PlaceOrder(items, uid);

                return Ok("Order placed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while placing order. {ex.Message}");
            }
        }

        // GET: api/order/GetAllOrders
        [HttpGet("GetAllOrders")]
        public IActionResult GetAllOrders()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userId = GetUserIdFromToken(token);
                int uid = int.Parse(userId);

                var orders = _orderService.GetAllOrders(uid);

                // Map to DTOs before returning
                var ordersDto = _mapper.Map<IEnumerable<OrdersOutputDTO>>(orders);

                return Ok(ordersDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving orders. {ex.Message}");
            }
        }

        // GET: api/order/GetOrderById/{OrderId}
        [HttpGet("GetOrderById/{OrderId}")]
        public IActionResult GetOrderById(int OrderId)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userId = GetUserIdFromToken(token);
                int uid = int.Parse(userId);

                var order = _orderService.GetOrderById(OrderId, uid);
                if (order == null) return NotFound();

                var orderDto = _mapper.Map<OrdersOutputDTO>(order);
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving order. {ex.Message}");
            }
        }

        // Helper: decode JWT token to extract user ID
        private string? GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");

                return subClaim?.Value;
            }

            throw new UnauthorizedAccessException("Invalid or unreadable token.");
        }
    }
}

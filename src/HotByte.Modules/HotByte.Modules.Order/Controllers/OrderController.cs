using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotByte.Modules.Order.Application.DTOs;
using HotByte.Modules.Order.Application.Interfaces;

namespace HotByte.Modules.Order.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service) { _service = service; }

        private int GetUserId() => int.Parse(User.FindFirst("userId")!.Value);
        private string GetRole() => User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        // Customer places order from their cart
        [HttpPost]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _service.PlaceOrderAsync(GetUserId(), dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var data = await _service.GetOrderByIdAsync(id, GetUserId(), GetRole());
                if (data == null) return NotFound(new { message = "Order not found." });
                return Ok(data);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        [HttpGet("my-orders")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetMyOrders()
            => Ok(await _service.GetOrdersByUserAsync(GetUserId()));

        // Requirement 7: owner can query orders for a specific restaurant they own.
        // Admin can query any restaurant. Ownership check lives inside the service.
        [HttpGet("restaurant/{restaurantId}")]
        [Authorize(Policy = "AdminOrRestaurantOwner")]
        public async Task<IActionResult> GetByRestaurant(int restaurantId)
        {
            try
            {
                var data = await _service.GetOrdersByRestaurantAsync(restaurantId, GetUserId(), GetRole());
                return Ok(data);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        // Requirement 7: owner sees ALL orders across ALL restaurants they own (derived from token)
        [HttpGet("my-restaurants-orders")]
        [Authorize(Policy = "RestaurantOwnerOnly")]
        public async Task<IActionResult> GetMyRestaurantsOrders()
            => Ok(await _service.GetMyRestaurantsOrdersAsync(GetUserId()));

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllOrdersAsync());

        // Requirement 6 + 1: owner/admin updates status; email fires on every change
        [HttpPut("{id}/status")]
        [Authorize(Policy = "AdminOrRestaurantOwner")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _service.UpdateOrderStatusAsync(id, dto, GetUserId(), GetRole());
                if (!result) return NotFound(new { message = "Order not found." });
                return Ok(new { message = "Order status updated." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        [HttpPost("{id}/cancel")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var result = await _service.CancelOrderAsync(id, GetUserId());
            if (!result) return BadRequest(new { message = "Cannot cancel this order." });
            return Ok(new { message = "Order cancelled successfully." });
        }
    }
}

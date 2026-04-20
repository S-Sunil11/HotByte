using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotByte.Modules.Order.Application.DTOs;
using HotByte.Modules.Order.Application.Interfaces;

namespace HotByte.Modules.Order.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize(Policy = "CustomerOnly")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service) { _service = service; }

        private int GetUserId() => int.Parse(User.FindFirst("userId")!.Value);

        [HttpGet]
        public async Task<IActionResult> GetCart()
            => Ok(await _service.GetCartAsync(GetUserId()));

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _service.AddToCartAsync(GetUserId(), dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("items/{cartItemId}")]
        public async Task<IActionResult> UpdateItem(int cartItemId, [FromBody] UpdateCartItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _service.UpdateCartItemAsync(GetUserId(), cartItemId, dto);
                if (!result) return NotFound(new { message = "Cart item not found." });
                return Ok(new { message = "Cart item updated." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            try
            {
                var result = await _service.RemoveCartItemAsync(GetUserId(), cartItemId);
                if (!result) return NotFound(new { message = "Cart item not found." });
                return Ok(new { message = "Cart item removed." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _service.ClearCartAsync(GetUserId());
            if (!result) return NotFound(new { message = "Cart not found." });
            return Ok(new { message = "Cart cleared." });
        }
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotByte.Modules.Menu.Application.DTOs;
using HotByte.Modules.Menu.Application.Interfaces;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Menu.Controllers
{
    [ApiController]
    [Route("api/menu-items")]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemService _service;

        public MenuItemController(IMenuItemService service) { _service = service; }

        private int GetUserId() => int.Parse(User.FindFirst("userId")!.Value);
        private string GetRole() => User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllMenuItemsAsync());

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetMenuItemByIdAsync(id);
            if (data == null) return NotFound(new { message = "Menu item not found." });
            return Ok(data);
        }

        [HttpGet("restaurant/{restaurantId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByRestaurant(int restaurantId)
            => Ok(await _service.GetMenuItemsByRestaurantAsync(restaurantId));

        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCategory(int categoryId)
            => Ok(await _service.GetMenuItemsByCategoryAsync(categoryId));

        // Requirement 2: search endpoint for menu items
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string keyword)
            => Ok(await _service.SearchMenuItemsAsync(keyword ?? string.Empty));

        [HttpGet("filter")]
        [AllowAnonymous]
        public async Task<IActionResult> Filter(
            [FromQuery] int? restaurantId,
            [FromQuery] int? categoryId,
            [FromQuery] DietaryType? dietaryType,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
            => Ok(await _service.FilterMenuItemsAsync(restaurantId, categoryId, dietaryType, minPrice, maxPrice));

        [HttpPost]
        [Authorize(Policy = "AdminOrRestaurantOwner")]
        public async Task<IActionResult> Create([FromBody] CreateMenuItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _service.CreateMenuItemAsync(dto, GetUserId(), GetRole());
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrRestaurantOwner")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuItemDto dto)
        {
            try
            {
                var result = await _service.UpdateMenuItemAsync(id, dto, GetUserId(), GetRole());
                if (!result) return NotFound(new { message = "Menu item not found." });
                return Ok(new { message = "Menu item updated successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/toggle-stock")]
        [Authorize(Policy = "AdminOrRestaurantOwner")]
        public async Task<IActionResult> ToggleOutOfStock(int id)
        {
            try
            {
                var result = await _service.ToggleOutOfStockAsync(id, GetUserId(), GetRole());
                if (!result) return NotFound(new { message = "Menu item not found." });
                return Ok(new { message = "Stock status toggled successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrRestaurantOwner")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteMenuItemAsync(id, GetUserId(), GetRole());
                if (!result) return NotFound(new { message = "Menu item not found." });
                return Ok(new { message = "Menu item deleted successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }
    }
}

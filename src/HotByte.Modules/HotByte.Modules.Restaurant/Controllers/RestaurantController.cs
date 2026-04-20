using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotByte.Modules.Restaurant.Application.DTOs;
using HotByte.Modules.Restaurant.Application.Interfaces;

namespace HotByte.Modules.Restaurant.Controllers
{
    [ApiController]
    [Route("api/restaurants")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _service;

        public RestaurantController(IRestaurantService service)
        {
            _service = service;
        }

        private int GetUserId() => int.Parse(User.FindFirst("userId")!.Value);
        private string GetRole() => User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllRestaurantsAsync());

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetRestaurantByIdAsync(id);
            if (data == null) return NotFound(new { message = "Restaurant not found." });
            return Ok(data);
        }

        // Requirement 2: search by name and/or category
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search(
            [FromQuery] string? name,
            [FromQuery] string? category)
        {
            var data = await _service.SearchRestaurantsAsync(name, category);
            return Ok(data);
        }

        // Requirement 7: owner sees restaurants they own, derived from their token
        [HttpGet("my-restaurants")]
        [Authorize(Policy = "RestaurantOwnerOnly")]
        public async Task<IActionResult> GetMyRestaurants()
        {
            var ownerId = GetUserId();
            var data = await _service.GetRestaurantsByOwnerAsync(ownerId);
            return Ok(data);
        }

        // Admin-only list by any ownerId
        [HttpGet("owner/{ownerUserId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetByOwner(int ownerUserId)
        {
            var data = await _service.GetRestaurantsByOwnerAsync(ownerUserId);
            return Ok(data);
        }

        // Requirement 8: Admin creates restaurant AND owner account atomically
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateWithOwner([FromBody] CreateRestaurantWithOwnerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.CreateRestaurantWithOwnerAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Requirement 6: owner restricted to own restaurants; admin can update any
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrRestaurantOwner")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRestaurantDto dto)
        {
            try
            {
                var result = await _service.UpdateRestaurantAsync(id, dto, GetUserId(), GetRole());
                if (!result) return NotFound(new { message = "Restaurant not found." });
                return Ok(new { message = "Restaurant updated successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteRestaurantAsync(id);
            if (!result) return NotFound(new { message = "Restaurant not found." });
            return Ok(new { message = "Restaurant deleted successfully." });
        }
    }
}

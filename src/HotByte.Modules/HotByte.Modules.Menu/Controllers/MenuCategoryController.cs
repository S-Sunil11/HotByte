using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotByte.Modules.Menu.Application.DTOs;
using HotByte.Modules.Menu.Application.Interfaces;

namespace HotByte.Modules.Menu.Controllers
{
    [ApiController]
    [Route("api/menu-categories")]
    public class MenuCategoryController : ControllerBase
    {
        private readonly IMenuCategoryService _service;

        public MenuCategoryController(IMenuCategoryService service) { _service = service; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllCategoriesAsync());

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetCategoryByIdAsync(id);
            if (data == null) return NotFound(new { message = "Category not found." });
            return Ok(data);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] CreateMenuCategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuCategoryDto dto)
        {
            var result = await _service.UpdateCategoryAsync(id, dto);
            if (!result) return NotFound(new { message = "Category not found." });
            return Ok(new { message = "Category updated successfully." });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteCategoryAsync(id);
            if (!result) return NotFound(new { message = "Category not found." });
            return Ok(new { message = "Category deleted successfully." });
        }
    }
}

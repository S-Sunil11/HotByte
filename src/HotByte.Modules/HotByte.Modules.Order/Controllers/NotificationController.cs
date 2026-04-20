using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotByte.Modules.Order.Application.Interfaces;

namespace HotByte.Modules.Order.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(INotificationService service) { _service = service; }

        private int GetUserId() => int.Parse(User.FindFirst("userId")!.Value);

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
            => Ok(await _service.GetUserNotificationsAsync(GetUserId()));

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var result = await _service.MarkAsReadAsync(id, GetUserId());
                if (!result) return NotFound(new { message = "Notification not found." });
                return Ok(new { message = "Notification marked as read." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }
    }
}

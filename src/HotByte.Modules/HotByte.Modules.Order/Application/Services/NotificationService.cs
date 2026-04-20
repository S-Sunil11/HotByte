using HotByte.Modules.Order.Application.DTOs;
using HotByte.Modules.Order.Application.Interfaces;
using HotByte.Modules.Order.Application.Mappings;
using HotByte.Modules.Order.Domain.Entities;
using HotByte.SharedKernel.Interfaces;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Order.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly IEmailService _emailService;
        private readonly IUserPublicService _userPublicService;

        public NotificationService(
            INotificationRepository repository,
            IEmailService emailService,
            IUserPublicService userPublicService)
        {
            _repository = repository;
            _emailService = emailService;
            _userPublicService = userPublicService;
        }

        // Requirement 1: send actual email on order placement AND every status change
        public async Task SendNotificationAsync(int userId, int orderId, string type, string message)
        {
            var notifType = System.Enum.TryParse<NotificationType>(type, out var nt) ? nt : NotificationType.General;

            var notification = new OrderNotification
            {
                UserId = userId,
                OrderId = orderId,
                Type = notifType,
                Message = message,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            await _repository.AddAsync(notification);

            // Send actual email
            var userEmail = await _userPublicService.GetUserEmailAsync(userId);
            var userName = await _userPublicService.GetUserNameAsync(userId);

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                var subject = $"HotByte - {notifType}";
                var htmlBody = $@"
<html>
  <body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #e74c3c;'>HotByte Order Update</h2>
    <p>Hi {userName},</p>
    <p>{message}</p>
    <p style='color: #777; font-size: 12px;'>This is an automated message from HotByte Food Ordering.</p>
  </body>
</html>";

                try
                {
                    await _emailService.SendAsync(userEmail, subject, htmlBody);
                }
                catch
                {
                    // Don't block notification flow on email failures — notification is still persisted in DB
                }
            }
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _repository.GetByUserIdAsync(userId);
            return notifications.Select(OrderMapper.ToDto).ToList();
        }

        // Requirement 6: only the owner of the notification can mark it read
        public async Task<bool> MarkAsReadAsync(int notificationId, int requesterUserId)
        {
            var notification = await _repository.GetByIdAsync(notificationId);
            if (notification == null) return false;

            if (notification.UserId != requesterUserId)
                throw new UnauthorizedAccessException("You do not own this notification.");

            return await _repository.MarkAsReadAsync(notificationId);
        }
    }
}

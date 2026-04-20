using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Order.Application.DTOs
{
    public record NotificationDto(
        int Id,
        int UserId,
        int OrderId,
        NotificationType Type,
        string Message,
        bool IsRead,
        DateTime SentAt);
}

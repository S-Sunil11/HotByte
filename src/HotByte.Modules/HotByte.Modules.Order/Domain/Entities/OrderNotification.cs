using System.ComponentModel.DataAnnotations;
using HotByte.SharedKernel;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Order.Domain.Entities
{
    public class OrderNotification : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int OrderId { get; set; }

        public NotificationType Type { get; set; }

        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}

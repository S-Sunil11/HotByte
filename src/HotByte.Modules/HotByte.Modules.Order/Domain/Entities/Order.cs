using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotByte.SharedKernel;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Order.Domain.Entities
{
    public class Order : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int RestaurantId { get; set; }

        [MaxLength(200)]
        public string? RestaurantName { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Placed;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? DiscountAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal FinalAmount { get; set; }

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        [MaxLength(500)]
        public string? ShippingAddress { get; set; }

        [MaxLength(20)]
        public string? ContactNumber { get; set; }

        public DateTime? EstimatedDeliveryTime { get; set; }
        public DateTime? DeliveredAt { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public virtual ICollection<OrderItem>? OrderItems { get; set; }
    }
}

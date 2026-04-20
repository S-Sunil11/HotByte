using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotByte.SharedKernel;

namespace HotByte.Modules.Order.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int MenuItemId { get; set; }

        [MaxLength(200)]
        public string MenuItemName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        public virtual Order? Order { get; set; }
    }
}

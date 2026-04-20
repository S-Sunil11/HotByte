using System.ComponentModel.DataAnnotations;
using HotByte.SharedKernel;

namespace HotByte.Modules.Order.Domain.Entities
{
    public class Cart : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        public virtual ICollection<CartItem>? CartItems { get; set; }
    }
}

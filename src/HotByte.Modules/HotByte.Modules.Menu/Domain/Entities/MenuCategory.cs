using System.ComponentModel.DataAnnotations;
using HotByte.SharedKernel;

namespace HotByte.Modules.Menu.Domain.Entities
{
    public class MenuCategory : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<MenuItem>? MenuItems { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class SizedItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Item")]
        public int ItemId { get; set; }

        [ForeignKey("ItemId")]
        public virtual Item? Item { get; set; }

        [Required]
        [Display(Name = "Size")]
        public int SizeId { get; set; }

        [ForeignKey("SizeId")]
        public virtual Size? Size { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Amount can not be negative")]
        public int Amount { get; set; }
    }
}

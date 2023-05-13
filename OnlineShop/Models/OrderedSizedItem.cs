using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class OrderedSizedItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Sized Item")]
        public int SizedItemId { get; set; }

        [ForeignKey("SizedItemId")]
        public virtual SizedItem? SizedItem { get; set; }

        [Required]
        [Display(Name = "Order")]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Ordered Amount can not be negative")]
        [Display(Name = "Ordered amount")]
        public int OrderedAmount { get; set; }

    }
}

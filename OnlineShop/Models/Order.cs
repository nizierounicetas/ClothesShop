using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual OnlineShop.Models.User User { get; set; }

        [Required]
        [Display(Name = "Order status")]
        public OrderStatus OrderStatus { get; set; }

        [Display(Name = "Order time")]
        public DateTime OrderTime { get; set; }

        [Display(Name = "Response time")]
        public DateTime? ResponseTime { get; set; }

        [Required]
        [Display(Name = "Total sum")]
        [Range(0, int.MaxValue, ErrorMessage = "Total sum can not be negative")]
        public double TotalSum { get; set; }

        [Display(Name = "Ordered Items")]
        public IList<OrderedSizedItem>? OrderedSizedItems { get; set; }

        [MaxLength(300, ErrorMessage = "Max length is 300 symbols")]
        [Display(Name = "User's notes")]
        public string? Notes { get; set; }

        [MaxLength(300, ErrorMessage = "Max length is 300 symbols")]
        [Display(Name = "Admin's comments")]
        public string? Commentaries { get; set; }

        [Required]
        [MaxLength(300, ErrorMessage = "Max length is 300 symbols")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
        [Phone]
        public string PhoneNumber { get; set; }

        public Order()
        {
            OrderedSizedItems = new List<OrderedSizedItem>();
        }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Category
    {
        [Key]
        public int Code { get; set; }
        [Required]
        public string Name { get; set; }
        [DisplayName("Display order")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Display order for category must be greater than 0")]
        public int DisplayOrder { get; set; }
    }
}

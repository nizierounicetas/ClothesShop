using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Item
    {
        public Item()
        {
            SizedItems = new List<SizedItem>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [MaxLength(600, ErrorMessage = "Max length is 300 symbols")]
        public string? Description { get; set; }

        public string? Image { get; set; }

        [Required]
        public Sex Sex { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(0, int.MaxValue)]
        public double Price { get; set; }

        [Required]
        [Display(Name= "Category type")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [Display(Name = "Sizes")]
        public IEnumerable<SizedItem>? SizedItems { get; set; }
    }
}

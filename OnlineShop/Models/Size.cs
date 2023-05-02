using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Size
    {
        [Key]
        public int Code { get; set; }
        [Required]
        [MaxLength(50,ErrorMessage = "Too long")]
        public string Name { get; set; }
    }
}

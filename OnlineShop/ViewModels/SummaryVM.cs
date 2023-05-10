using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using OnlineShop.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.ViewModels
{
    public class SummaryVM
    {

        public SummaryVM()
        {
            ItemsDictionary = new Dictionary<SizedItem, int>();
        }

        [Required]
        public User User { get; set; }

        [Display(Name = "Items")]
        public IDictionary<SizedItem, int> ItemsDictionary { get; set; }

        [Required]
        [Display(Name = "Phone")]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        public string? Notes { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShop.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.ViewModels
{
    public class DetailsVM
    {
        public Item Item { get; set; } = new Item();
        public bool IsInCart { get; set; } = false;

        [Required(ErrorMessage = "Size is not chosen")]
        public int ChosenSizedItemId { get; set; }

        [Display(Name="Size")]
        public IEnumerable<SelectListItem> ExistingSizesSelectList { get; set; } = new List<SelectListItem>();
    }
}

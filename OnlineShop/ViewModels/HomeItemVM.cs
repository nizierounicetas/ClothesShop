using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShop.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.ViewModels
{
    public class HomeItemVM
    {
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<Category> Categories { get; set; }

        public IEnumerable<Sex> Sexes { get; set; } = new List<Sex>() { Sex.Male, Sex.Female, Sex.Unisex };

        public IEnumerable<SelectListItem> PriceOrders { get; set; } = new List<SelectListItem>() {
            new SelectListItem(){ Value = PriceOrder.Ascending.ToString(), Text= "Price ascending"},
            new SelectListItem(){ Value = PriceOrder.Descending.ToString(), Text= "Price descending"}
        };

        public int? CategoryId { get; set; }
        public Sex? SexProp { get; set; }

        [Required(ErrorMessage = "Sorting is not specified")]
        public PriceOrder PriceOrderProp { get; set; }
    }
}

using OnlineShop.Models;

namespace OnlineShop.ViewModels
{
    public class HomeItemVM
    {
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Size> Sizes { get; set; }
        public IEnumerable<Sex> Sexes { get; set; } = new List<Sex>() { Sex.Male, Sex.Female, Sex.Unisex };
    }
}

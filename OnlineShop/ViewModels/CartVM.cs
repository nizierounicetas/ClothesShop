using OnlineShop.Models;

namespace OnlineShop.ViewModels
{
    public class CartVM
    {
        public IDictionary<SizedItem, int> ItemsDictionary { get; set; }

        public CartVM()
        {
            ItemsDictionary = new Dictionary<SizedItem, int>();
        }
    }
}

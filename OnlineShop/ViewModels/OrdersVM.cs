using OnlineShop.Models;

namespace OnlineShop.ViewModels
{
    public class OrdersVM
    {
        public IEnumerable<Order> ConfirmedOrders { get; set; }
        public IEnumerable<Order> NotConsideredOrders { get; set; }
        public IEnumerable<Order> DeniedOrders { get; set; }
    }
}

using OnlineShop.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.ViewModels
{
    public class SizeVM
    {
        public Size Size { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Amount can not be negative")]
        public int Amount { get; set; }

        public bool Checked { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShop.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.ViewModels
{
    public class ItemVM
    {
        public Item Item { get; set; }
        public IEnumerable<SelectListItem>? CategorySelectList { get; set; }
        public IEnumerable<SelectListItem>? SexSelectList { get; set; }
        [Display(Name = "Sizes")]
        public IList<SizeVM> SizeVMList { get; set; }
    }
}


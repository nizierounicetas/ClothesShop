using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.Utility;
using OnlineShop.ViewModels;
using System.Diagnostics;

namespace OnlineShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            this._dbContext = dbContext;
        }

        public IActionResult Index(HomeItemVM homeItemVM)
        {
            if (homeItemVM == null)
            {
                homeItemVM = new HomeItemVM();
            }

            homeItemVM.Categories = _dbContext.Categories.OrderBy(c => c.Name);

            if (homeItemVM.CategoryId == null)
            {
                homeItemVM.Items = _dbContext.Items.Include(i => i.Category).Include(i => i.SizedItems);
            }
            else
            {
                homeItemVM.Items = _dbContext.Items.Include(i => i.Category).Include(i => i.SizedItems)
                    .Where(i => i.CategoryId == homeItemVM.CategoryId);
            }

            if (homeItemVM.SexProp != null)
            {
                homeItemVM.Items = homeItemVM.Items.Where(i => i.Sex == homeItemVM.SexProp);
            }

            if (homeItemVM.PriceOrderProp != null)
            {
                if (homeItemVM.PriceOrderProp == PriceOrder.Ascending)
                {
                    homeItemVM.Items = homeItemVM.Items.OrderBy(i => i.Price);
                }
                else if (homeItemVM.PriceOrderProp == PriceOrder.Descending)
                {
                    homeItemVM.Items = homeItemVM.Items.OrderByDescending(i => i.Price);
                }
            }

            if (TempData[WC.MessageAlertName] != null)
            {
                ViewData[WC.MessageAlertName] = TempData[WC.MessageAlertName];
            }

            if (TempData[WC.ErrorMessageAlertName] != null)
            {
                ViewData[WC.ErrorMessageAlertName] = TempData[WC.ErrorMessageAlertName];
            }

            return View(homeItemVM);
        }

        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Details(int? id)
        {
            Item item = await _dbContext.Items.Include(i => i.Category).Include(i => i.SizedItems)
                .ThenInclude(si => si.Size).Where(i => i.Id == id).FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound();
            }

            var detailsVM = await GetDetailsVMAsync(id);

            if (detailsVM == null)
                return NotFound();

            if (TempData[WC.ErrorMessageAlertName] != null)
                ViewData[WC.ErrorMessageAlertName] = TempData[WC.ErrorMessageAlertName];

            if (TempData[WC.MessageAlertName] != null)
                ViewData[WC.MessageAlertName] = TempData[WC.MessageAlertName];

            return View(detailsVM);
        }

        public async Task<DetailsVM> GetDetailsVMAsync(int? id)
        {
            Item item = await _dbContext.Items.Include(i => i.Category).Include(i => i.SizedItems)
                .ThenInclude(si => si.Size).Where(i => i.Id == id).FirstOrDefaultAsync();

            if (item == null)
                return null;

            return new DetailsVM()
            {
                Item = item,
                ExistingSizesSelectList = this.ExtractExistingSizes(item),
            };
        }

        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int? chosenSizedItemId)
        {
            if (chosenSizedItemId == null)
                return NotFound();
            
            List<ShoppingCartItem> shoppingCartList = HttpContext.Session.Get<List<ShoppingCartItem>>(WC.SessionCart);

            if (shoppingCartList == null)
                shoppingCartList = new List<ShoppingCartItem>();

            var sizedItem = await _dbContext.SizedItems.FindAsync(chosenSizedItemId);

            if (sizedItem == null)
                return NotFound();

            var shoppingCartItem = shoppingCartList.Find(i => i.SizedItemId == chosenSizedItemId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem() { SizedItemId = chosenSizedItemId.Value, Amount = 0 };
            }

            if (shoppingCartItem.Amount + 1 <= sizedItem.Amount)
            {
                shoppingCartItem.Amount += 1;

                if (shoppingCartItem.Amount == 1)
                {
                    shoppingCartList.Add(shoppingCartItem);
                }

                TempData[WC.MessageAlertName] = "Item added to cart";

                HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            }
            else
            {
                TempData[WC.ErrorMessageAlertName] = "Quantity of available items exceeded";
            }

            return RedirectToAction(nameof(Details));
        }

        private IEnumerable<SelectListItem> ExtractExistingSizes(Item item)
        {
            if (item.SizedItems == null)
                return null;

            List<SelectListItem> sizes = new List<SelectListItem>();
            foreach(var sizedItem in item.SizedItems)
            {
                if (sizedItem.Size != null && sizedItem.Amount > 0)
                {
                    sizes.Add(new SelectListItem() { Value = sizedItem.Id.ToString(), Text = sizedItem.Size.Name });
                }
            }

            return sizes;
        }

        private IEnumerable<Sex> Sexes
        {
            get
            {
                return new List<Sex>() { Sex.Male, Sex.Female, Sex.Unisex };
            }
        }

        private IEnumerable<SelectListItem> PriceOrders
        {
            get
            {
                return new List<SelectListItem>() {
                    new SelectListItem(){ Value = PriceOrder.Ascending.ToString(), Text= "Price ascending"},
                    new SelectListItem(){ Value = PriceOrder.Descending.ToString(), Text= "Price descending"}
                    };
            }
        }
    }
}